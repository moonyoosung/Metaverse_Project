using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Newtonsoft.Json.Linq;
using MindPlus.Contexts.Master.Menus.PeopleView;
using UnityEngine;

public class FollowView : UIView,  GetFriendListCommand.IEventHandler
{
    private Persistent persistent;
    private UILayoutGroupContainer groupContainer;
    private UIManager uIManager;
    private FollowViewContext context;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.persistent = persistent;
        this.context = new FollowViewContext();

        this.uIManager = (uIManager as UIManager);
        UIHorizontalButtonGroup[] uIHorizontalLists = GetComponentsInChildren<UIHorizontalButtonGroup>(true);
        List<UILayoutGroup> lists = new List<UILayoutGroup>();
        foreach (var uIHorizontalList in uIHorizontalLists)
        {
            uIHorizontalList.Set();
            lists.Add(uIHorizontalList);
        }
        groupContainer = new UILayoutGroupContainer(lists);
    }
    public override void OnStartShow()
    {
        //this.persistent.APIManager.ResisterEvent(this);
        //persistent.PeopleManager.GetFriendList();
        base.OnStartShow();
    }

    public override void OnStartHide()
    {
        //this.persistent.APIManager.UnResisterEvent(this);
        base.OnStartHide();
    }

    public void OnGetFriendListFailed(NetworkMessage message)
    {
       
    }

    //추후 API 개발되면 Friend 정보 대신 Follow 정보를 가지고 뿌려준다.
    //UI 확인을 위해 테스트
    public void OnGetFriendListSuccess(NetworkMessage message)
    {
        JObject jObject = JObject.Parse(message.body);

        List<PeopleData> peopleDatas = persistent.PeopleManager.GetList(jObject);

        int FollowingCnt = 0;
        int FollowerCnt = 0;

        for (int i = 0; i < peopleDatas.Count; i++)
        {
            string temp = "following";
            if (i % 3 == 0)
            {
                FollowerCnt++;
                temp = "follower";
            }
            else
            {
                FollowingCnt++;
            }
            //현재 가지고 있는 category에 해당하는 Group을 가져오기
            if (!groupContainer.TryGetUILayourGroup<UIHorizontalButtonGroup>(temp, out UIHorizontalButtonGroup targetList))
            {
                return;
            }

            ////해당 Group이 추가가 가능한 상태인가?
            //if (!targetList.IsADDAvailable())
            //{
            //    return;
            //}

            //해당 Group이 추가가 가능한 상태인가?
            if (targetList.group.transform.childCount >= 7)
            {
                if (!targetList.more.gameObject.activeSelf) targetList.more.gameObject.SetActive(true);
                continue;
            }
            targetList.more.gameObject.SetActive(false);

            UIPeople people = uIManager.GetPool(StringTable.UIPeoplePool).Get<UIPeople>(targetList.group.transform);
            people.Set(persistent, peopleDatas[i]);
        }
        Debug.Log(FollowingCnt);

        context.SetValue("FollowingCountText", "Following (" + FollowingCnt + ")");
        context.SetValue("FollowerCountText", "Followers (" + FollowerCnt + ")");
    }
}

