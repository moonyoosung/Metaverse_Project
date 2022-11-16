using MindPlus.Contexts.Master.Menus.PeopleView;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FriendView : UIView, GetFriendListCommand.IEventHandler
{
    public ScrollRect scroll;
    public GameObject target;
    private Persistent persistent;
    private FriendViewContext context;
    private UIPool groupPool;
    private UIPool peoplePool;
    private List<UIPeople> uIPeoples = new List<UIPeople>();
    private List<UIPeopleGroup> uIPeopleGroup = new List<UIPeopleGroup>();
    private VerticalLayoutGroup verticalLayout;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.persistent = persistent;
        this.context = new FriendViewContext();
        ContextHolder.Context = context;

        UIManager UIManager = (uIManager as UIManager);
        this.groupPool = UIManager.GetPool(StringTable.UIPeopleGroupPool);
        this.peoplePool = UIManager.GetPool(StringTable.UIPeoplePool);

        this.verticalLayout = target.GetComponent<VerticalLayoutGroup>();
    }

    public override void OnStartShow()
    {
        if (uIPeoples.Count > 0)
        {
            foreach (var uIPeople in uIPeoples)
            {
                uIPeople.InActivePool();
            }

            foreach (var group in uIPeopleGroup)
            {
                group.InActivePool();
            }

            uIPeoples.Clear();
            uIPeopleGroup.Clear();
        }

        persistent.APIManager.ResisterEvent(this);
        persistent.PeopleManager.GetFriendList();
        base.OnStartShow();
    }

    //public override void OnFinishHide()
    //{
    //    persistent.APIManager.ResisterEvent(this);
    //    base.OnFinishHide();
    //}

    public override void OnStartHide()
    {
        persistent.APIManager.UnResisterEvent(this);
        base.OnStartHide();
    }

    public void OnGetFriendListSuccess(NetworkMessage message)
    {
        PeopleData peopleData = JsonUtility.FromJson<PeopleData>(message.body);

        JObject jObject = JObject.Parse(message.body);

        List<PeopleData> peopleDatas = persistent.PeopleManager.GetList(jObject);
        context.SetValue("FriendCountText", "Friends (" + peopleDatas.Count() + ")");

        int groupCnt = 0;
        int maxCnt = 7;

        groupCnt = peopleDatas.Count / maxCnt;
        if (peopleDatas.Count % maxCnt != 0)
        {
            groupCnt++;
        }

        for (int i = 0; i < groupCnt; i++)
        {
            UIPeopleGroup contentGroup = groupPool.Get<UIPeopleGroup>(target.transform);
            contentGroup.Set();
            uIPeopleGroup.Add(contentGroup);
        }
      
        int idx = 0;
        foreach (var group in uIPeopleGroup)
        {
            for (int i = idx; i < peopleDatas.Count; i++)
            {

                if (!group.IsADDAvailable())
                {
                    idx = i;
                    break;
                }

                UIPeople uIPeople = peoplePool.Get<UIPeople>(group.group.transform);
                uIPeople.Set(persistent, peopleDatas[i]);
                uIPeoples.Add(uIPeople);
            }
        }
        if (uIPeopleGroup.Count > 0)
        {
            float height = uIPeopleGroup[0].rectTransform.sizeDelta.y * groupCnt + verticalLayout.spacing * (groupCnt - 1) + 150f;
            scroll.content.sizeDelta = new Vector2(scroll.content.sizeDelta.x, height);
            scroll.content.localPosition = Vector3.zero;
        }
    }

    public void OnGetFriendListFailed(NetworkMessage message)
    {
        
    }
}