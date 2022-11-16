using MindPlus;
using MindPlus.Contexts.Master.ProfileView;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PeopleAboutView : UIFlatView/*, GetLocationCommand.IEventHandler,*/ /*GetFriendListCommand.IEventHandler, GetRequestFriendListCommand.IEventHandler,*/ /*DelFriendCommand.IEventHandler*/
{
    private Persistent persistent;
    private PeopleAboutViewContext context;
    public ScrollRect scroll;
    public RectTransform rectTransform;
    private PeopleData data;
    private ContentData contentData;
    private bool isClick = false;
    private bool isSetting = false;
    public enum ProfileKind
    {
        NotFriend,
        RequestFriend,
        Friend,
    }

    public ProfileKind kind;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.persistent = persistent;
        this.context = new PeopleAboutViewContext();
        ContextHolder.Context = context;

        UIManager UIManager = (uIManager as UIManager);

        #region 친구 상태 버튼
        //프로필에서 더보기 버튼 눌렀을 시
        context.onClickMore = () =>
        {
            context.SetValue("IsActiveMore", !context.IsActiveMore);
        };
        //친구 끊기 버튼 눌렀을 시
        context.onClickUnfriend += OnClickUnFriend;
        #endregion

        //블록 처리 버튼 눌렀을 시
        context.onClickBlock += OnClickBlock;
        context.onClickInvite += OnClickInvite;

        //친구가 아닌 상태에서 버튼 상호작용
        context.OnFollowingToggleChanged += OnFollowingToggleChanged;
        context.OnFreindToggleChanged += OnFreindToggleChanged;

        //기본 Room Image를 Loading 상태로 초기화
        context.SetValue("ThumbnailIcon", persistent.ResourceManager.ImageContainer.Get("loadingcontent"));
        context.SetValue("RoomName", "None");

        context.SetValue("IsInteractable", false);
        context.SetValue("OnlineIcon", persistent.ResourceManager.ImageContainer.Get("peopleoffline"));
    }

    private void OnClickBlock()
    {
        context.onClickBlock -= OnClickBlock;

        //친구 상태라면 친구 리스트에서 제거
        if (kind == ProfileKind.Friend)
        {
            persistent.PeopleManager.DelFriend(data.userId,
                () =>
                {
                    context.onClickBlock += OnClickBlock;
                    persistent.PeopleManager.GetFriendList((friend) =>
                    { OnGetFriendListSuccess(friend); });
                },
                (message) =>
                {
                    (UIManager as UIManager).PushNotify(message, 2f);
                });
        }
        //추후 Block 리스트에 추가
    }

    private void OnClickInvite()
    {
        if (String.IsNullOrEmpty(data.connectionId)) return;

        if (isClick) return;
        isClick = true;

        context.SetValue("OnInviteIcon", persistent.ResourceManager.ImageContainer.Get("darkgraycheck"));
        context.SetValue("InviteText", "Sent");
        persistent.AccountManager.RequestInvite(data);
    }

    private void OnClickUnFriend()
    {
        context.onClickUnfriend -= OnClickUnFriend;

        persistent.PeopleManager.DelFriend(data.userId,
            () =>
            {
                context.onClickUnfriend += OnClickUnFriend;
                persistent.PeopleManager.GetFriendList((friend) =>
                { OnGetFriendListSuccess(friend); });
            },
            (message) =>
            {
                (UIManager as UIManager).PushNotify(message, 2f);
                context.onClickUnfriend += OnClickUnFriend;
            });
    }

    public override void OnStartShow()
    {
        //context.SetValue("RoomName", "None");
        //this.persistent.APIManager.ResisterEvent(this);
        base.OnStartShow();
    }

    public override void OnStartHide()
    {
        //Destroy(avatar.tr);
        context.SetValue("IsInteractable", false);
        context.SetValue("OnlineIcon", persistent.ResourceManager.ImageContainer.Get("peopleoffline"));
        //this.persistent.APIManager.UnResisterEvent(this);
        base.OnStartHide();
    }

    /// <summary>
    /// UI 보여주기 위한 테스트 SET fUN
    /// ExplorerView에서 접근할때만 보여주기로
    /// </summary>

    public void Set(PeopleData peopleData)
    {
        isSetting = false;
        isClick = false;

        persistent.PeopleManager.GetLocation(peopleData.userId);
        this.data = new PeopleData(peopleData.userId);
        persistent.PeopleManager.GetFriendList();
        context.SetValue("DescText", peopleData.description);
        context.SetValue("OnInviteIcon", persistent.ResourceManager.ImageContainer.Get("add"));
        context.SetValue("InviteText", "Invite");
        SetUserInfo(peopleData.userName);

        SetContentSize();
    }

    public void Set(LocalPlayerData playerData)
    {
        isSetting = false;
        isClick = false;

        this.data = new PeopleData(playerData.userId);

        //기본 Room Image를 Loading 상태로 초기화
        context.SetValue("ThumbnailIcon", persistent.ResourceManager.ImageContainer.Get("loadingcontent"));
        context.SetValue("RoomName", "None");

        persistent.PeopleManager.GetLocation(playerData.userId, (location) =>
        {
            OnGetLocationSuccess(JObject.Parse(location), () =>
            {
                persistent.PeopleManager.GetFriendList((friend) =>
                { OnGetFriendListSuccess(friend); });
            });
        });

        context.SetValue("DescText", playerData.description);
        context.SetValue("OnInviteIcon", persistent.ResourceManager.ImageContainer.Get("add"));
        context.SetValue("InviteText", "Invite");
        SetUserInfo(playerData.userName, !string.IsNullOrEmpty(playerData.connectionId));

        SetContentSize();
    }

    public void OnGetLocationSuccess(JObject jObject, Action onAction = null)
    {
        contentData = null;
        if (string.IsNullOrEmpty(jObject.ToString()) || jObject.Count <= 0)
        {            //context.SetValue("ThumbnailIcon", persistent.ResourceManager.ImageContainer.Get("loadingcontent"));
            //context.SetValue("RoomName", "None");

            onAction?.Invoke();
            return;
        }
        contentData = persistent.RoomDataBaseManager.RoomAPIHandler.CreateContentData(jObject);

        if (contentData != null && contentData.roomId != null)
        {
            context.SetValue("RoomName", contentData.roomName);
            if (!string.IsNullOrEmpty(contentData.thumbnail))
            {
                persistent.APIManager.DownLoadTexture(contentData.thumbnail, (sprite) =>
                {
                    if (sprite)
                    {
                        context.SetValue("ThumbnailIcon", sprite);
                    }
                });
            }
            else if (!string.IsNullOrEmpty(contentData.sceneName))
            {
                string sceneName = contentData.sceneName.ToLower();
                persistent.APIManager.DownLoadTexture("location/" + sceneName + ".jpg", (sprite) =>
                {
                    if (sprite)
                    {
                        context.SetValue("ThumbnailIcon", sprite);
                    }
                    else
                    {
                        context.SetValue("ThumbnailIcon", persistent.ResourceManager.ThumbnailData.Get(sceneName).thumbnail);
                    }
                });
            }
        }

        onAction?.Invoke();
    }

    public void OnFollowingToggleChanged(bool prev, bool next)
    {
        if (data == null || prev == next)
            return;

        if (!prev && next)
        {
            context.SetValue("OnFollowingIcon", persistent.ResourceManager.ImageContainer.Get("whitecheck"));
            context.SetValue("FollowColor", Color.white);
        }
        else if (prev && !next)
        {
            context.SetValue("OnFollowingIcon", persistent.ResourceManager.ImageContainer.Get("add"));
            context.SetValue("FollowColor", Color.black);
        }
        //팔로윙 API 개발 시 적용
    }

    public void OnFreindToggleChanged(bool prev, bool next)
    {
        if (!isSetting) return;
        if (data == null || prev == next)
            return;

        Debug.Log("OnFreindToggleChanged1");
        if (next)
        {
            context.SetValue("FriendText", "Send");
            context.SetValue("OnFriendIcon", persistent.ResourceManager.ImageContainer.Get("darkgraycheck"));
            persistent.PeopleManager.PostRequestFriend(data.userId);
        }
        else if (!next)
        {
            context.SetValue("FriendText", "Friend");
            context.SetValue("OnFriendIcon", persistent.ResourceManager.ImageContainer.Get("addfriend"));
            persistent.PeopleManager.DelRequestFriend(persistent.AccountManager.PlayerData.userId, data.userId);
        }
    }

    private void SetContentSize()
    {
        float height = 700 + rectTransform.sizeDelta.y;
        scroll.content.sizeDelta = new Vector2(scroll.content.sizeDelta.x, height);
        scroll.content.localPosition = Vector3.zero;
    }

    private ProfileKind GetRelationShip(JObject jObject, ProfileKind Success)
    {
        ProfileKind kind = ProfileKind.NotFriend;
        foreach (var item in jObject["Items"])
        {
            JObject obj = (JObject)item;
            if (!data.Equals(obj))
                continue;

            kind = Success;
            data.SetPeopleData(obj);
        }
        return kind;
    }

    private void SetUserInfo(string userName, bool isOnline = false)
    {
        context.SetValue("UserNameText", userName);

        //todo
        context.SetValue("FollowerCountText", 0 + " followers");
        context.SetValue("IsActiveMore", false);
        if (isOnline)
        {
            context.SetValue("OnlineIcon", persistent.ResourceManager.ImageContainer.Get("peopleonline"));
        }
        else
        {
            context.SetValue("OnlineIcon", persistent.ResourceManager.ImageContainer.Get("peopleoffline"));
        }
        float height = 700 + rectTransform.sizeDelta.y;
        scroll.content.sizeDelta = new Vector2(scroll.content.sizeDelta.x, height);
        scroll.content.localPosition = Vector3.zero;
    }

    private void SetFriendShipInfo(ProfileKind kind)
    {
        switch (kind)
        {
            case ProfileKind.Friend:
                context.SetValue("IsActiveFriend", true);
                context.SetValue("IsActiveNotFriend", false);
                context.SetValue("IsInteractable", data.IsOnline());
                if (data.IsOnline())
                {
                    context.SetValue("OnlineIcon", persistent.ResourceManager.ImageContainer.Get("peopleonline"));
                }
                else
                {
                    context.SetValue("OnlineIcon", persistent.ResourceManager.ImageContainer.Get("peopleoffline"));
                }

                context.SetValue("FreindToggle", false);
                isSetting = true;
                break;
            default:
                persistent.PeopleManager.GetRequestFriendList((request) => { OnGetRequestFriendListSuccess(request); });

                context.SetValue("IsActiveNotFriend", true);
                context.SetValue("IsActiveFriend", false);
                context.SetValue("FollowingToggle", true);
                break;
        }
    }

    private void SetRequestInfo(ProfileKind kind)
    {
        switch (kind)
        {
            case ProfileKind.RequestFriend:

                context.SetValue("FreindToggle", true);
                context.SetValue("IsInteractable", data.IsOnline());
                if (data.IsOnline())
                {
                    context.SetValue("OnlineIcon", persistent.ResourceManager.ImageContainer.Get("peopleonline"));
                }
                else
                {
                    context.SetValue("OnlineIcon", persistent.ResourceManager.ImageContainer.Get("peopleoffline"));
                }
                context.SetValue("FriendText", "Send");
                context.SetValue("OnFriendIcon", persistent.ResourceManager.ImageContainer.Get("darkgraycheck"));
                break;
            default:
                context.SetValue("FriendText", "Friend");
                context.SetValue("OnFriendIcon", persistent.ResourceManager.ImageContainer.Get("addfriend"));
                context.SetValue("FreindToggle", false);
                break;
        }
        isSetting = true;
    }

    public void OnGetFriendListSuccess(string message)
    {
        JObject jObject = JObject.Parse(message);
        this.kind = GetRelationShip(jObject, ProfileKind.Friend);

        SetFriendShipInfo(kind);
    }

    public void OnGetRequestFriendListSuccess(string message)
    {
        JObject jObject = JObject.Parse(message);

        this.kind = GetRelationShip(jObject, ProfileKind.RequestFriend);

        SetRequestInfo(kind);
    }

    //public void OnDelFriendSuccess(NetworkMessage message)
    //{
    //    persistent.PeopleManager.GetFriendList();
    //}

    #region API - Failed
    //public void OnGetFriendListFailed(NetworkMessage message)
    //{
    //    Debug.Log("OnGetFriendListFailed");
    //}

    //public void OnGetRequestFriendListFailed(NetworkMessage message)
    //{
    //}

    //public void OnDelFriendFailed(NetworkMessage message)
    //{
    //}
    #endregion
}
