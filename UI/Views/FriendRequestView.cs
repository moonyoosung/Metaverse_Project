using MindPlus.Contexts.Master.Menus.PeopleView;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FriendRequestView : UIView, GetRequestFriendListCommand.IEventHandler, DelRequestFriendCommand.IEventHandler, PostAcceptFriendCommand.IEventHandler
{
    public ScrollRect scroll;
    public Transform listTransform;
    private Persistent persistent;
    private FriendRequestViewContext context;
    private UIPool requestPool;
    private VerticalLayoutGroup verticalLayout;
    private List<UIFriendRequest> uIRequests = new List<UIFriendRequest>();

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.persistent = persistent;

        this.context = new FriendRequestViewContext();
        ContextHolder.Context = context;

        UIManager UIManager = (uIManager as UIManager);
        this.requestPool = UIManager.GetPool(StringTable.UIFriendRequestPool);

        this.verticalLayout = listTransform.GetComponent<VerticalLayoutGroup>();
    }

    public override void OnStartShow()
    {
        if (uIRequests.Count > 0)
        {
            foreach (var uIContent in uIRequests)
            {
                uIContent.InActivePool();
            }
            uIRequests.Clear();
        }
        persistent.APIManager.ResisterEvent(this);
        persistent.PeopleManager.GetRequestFriendList();
        base.OnStartShow();
    }
    public override void OnStartHide()
    {
        persistent.APIManager.UnResisterEvent(this);
        base.OnStartHide();
    }

    public void OnGetRequestFriendListSuccess(NetworkMessage message)
    {
        JObject jObject = JObject.Parse(message.body);

        List<PeopleData> contentDatas = persistent.PeopleManager.GetList(jObject);

        if (uIRequests.Count > 0)
        {
            foreach (var uIContent in uIRequests)
            {
                uIContent.InActivePool();
            }
            uIRequests.Clear();
        }

        context.SetValue("RequestFriendCountText", "Request (" + contentDatas.Count()  +")");

        for (int i = 0; i < contentDatas.Count; i++)
        {
            UIFriendRequest uIRequest = requestPool.Get<UIFriendRequest>(listTransform);
            uIRequest.Initialize(persistent, contentDatas[i]);
            uIRequests.Add(uIRequest);
        }

        if (uIRequests.Count > 0)
        {
            float height = uIRequests[0].rectTransform.sizeDelta.y * contentDatas.Count + verticalLayout.spacing * (contentDatas.Count - 1) + 210f;
            scroll.content.sizeDelta = new Vector2(scroll.content.sizeDelta.x, height);
            scroll.content.localPosition = Vector3.zero;
        }
    }

    public void OnGetRequestFriendListFailed(NetworkMessage message)
    {

    }

    public void OnDelRequestFriendSuccess(NetworkMessage message)
    {
        persistent.PeopleManager.GetRequestFriendList();
    }

    public void OnDelRequestFriendFailed(NetworkMessage message)
    {
    }

    public void OnPostAcceptFriendSuccess(NetworkMessage message)
    {
        persistent.PeopleManager.GetRequestFriendList();
    }

    public void OnPostAcceptFriendFailed(NetworkMessage message)
    {
    }
}

