using MindPlus.Contexts.Master.Menus.WorldView;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PeopleListView : UIView, GetFriendListCommand.IEventHandler
{
    public ScrollRect scroll;
    private ContentListContext context;
    private UIPool groupPool;
    private UIPool peoplePool;
    private Persistent persistent;
    private List<UIPeople> uIPeoples = new List<UIPeople>();
    private List<UIPeopleGroup> uIPeopleGroups = new List<UIPeopleGroup>();
    private VerticalLayoutGroup verticalLayout;
    private Queue<string> queue = new Queue<string>();

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.context = new ContentListContext();
        ContextHolder.Context = context;
        UIManager UIManager = (uIManager as UIManager);
        this.groupPool = UIManager.GetPool(StringTable.UIPeopleGroupPool);
        this.peoplePool = UIManager.GetPool(StringTable.UIPeoplePool);
        this.persistent = persistent;
        this.verticalLayout = scroll.content.GetComponent<VerticalLayoutGroup>();
    }
    public override void OnStartShow()
    {
        persistent.APIManager.ResisterEvent(this);
        CallAPI(queue.Peek());
        base.OnStartShow();
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();
        if (uIPeoples.Count > 0)
        {
            foreach (var uIPeople in uIPeoples)
            {
                uIPeople.InActivePool();
            }

            foreach (var uIPeopleGroup in uIPeopleGroups)
            {
                uIPeopleGroup.InActivePool();
            }

            uIPeoples.Clear();
            uIPeopleGroups.Clear();
        }
    }

    public void Set(string titleCategory)
    {
        queue.Enqueue(titleCategory);
        context.SetValue("Title", ConvertCateogry(titleCategory));
    }
    public void ClearQueue()
    {
        queue.Clear();
    }
    public override void OnStartHide()
    {
        persistent.APIManager.UnResisterEvent(this);
        base.OnStartHide();
    }
    private string ConvertCateogry(string category)
    {
        switch (category)
        {
            case "following":
                return "Following";
            case "follower":
                return "Follower";
        }

        return "Not Found";
    }

    //테스트용
    private void CallAPI(string category)
    {
        switch (category)
        {
            case "following":
                persistent.PeopleManager.GetFriendList();
                //persistent.RoomDataBaseManager.RoomAPIHandler.GetContentsList("live", ContentTypes.Event);
                break;
            case "follower":
                persistent.PeopleManager.GetFriendList();
                //persistent.RoomDataBaseManager.RoomAPIHandler.GetContentsList("myevent", ContentTypes.Event);
                break;
        }

    }

    //테스트용 추후 API 개발되면 수정
    public void OnGetFriendListSuccess(NetworkMessage message)
    {
        JObject jObject = JObject.Parse(message.body);

        List<PeopleData> peopleDatas = persistent.PeopleManager.GetList(jObject);

        int groupCnt = 0;
        int maxCnt = 7;

        groupCnt = peopleDatas.Count / maxCnt;
        if (peopleDatas.Count % maxCnt != 0)
        {
            groupCnt++;
        }


        for (int i = 0; i < groupCnt; i++)
        {
            UIPeopleGroup peopleGroup = groupPool.Get<UIPeopleGroup>(scroll.content.transform);
            peopleGroup.Set();
            uIPeopleGroups.Add(peopleGroup);
        }

        int idx = 0;
        foreach (var group in uIPeopleGroups)
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
        if (uIPeopleGroups.Count > 0)
        {
            float height = uIPeopleGroups[0].rectTransform.sizeDelta.y * groupCnt + verticalLayout.spacing * (groupCnt - 1) + 86f;
            scroll.content.sizeDelta = new Vector2(scroll.content.sizeDelta.x, height);
            scroll.content.localPosition = Vector3.zero;
        }
    }

    public void OnGetFriendListFailed(NetworkMessage message)
    {
    }
}
