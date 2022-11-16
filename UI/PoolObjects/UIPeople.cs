using Slash.Unity.DataBind.Core.Presentation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MindPlus;
using System;
using MindPlus.Contexts.Master.Menus.PeopleView;
using System.Globalization;


public class UIPeople : UIPoolObject
{
    [HideInInspector]
    public UIPeopleContext context;
    public ContextHolder contextHolder;
    private UIManager uIManager;
    private PeopleManager peopleManager;
    public PeopleData data;

    //제거예정
    PeopleAboutView.ProfileKind kind;

    public override void Initialize(Transform parent)
    {
        base.Initialize(parent);
        this.context = new UIPeopleContext();
        contextHolder.Context = context;

        context.onClickProfile += OnClickButton;
    }

    //추후 PeopleAboutView.ProfileKind 과 관련된건 제가할 것
    public void Set(Persistent persistent, PeopleData data, PeopleAboutView.ProfileKind kind = PeopleAboutView.ProfileKind.Friend)
    {
        this.uIManager = persistent.UIManager;
        this.peopleManager = persistent.PeopleManager;
        this.data = data;
        this.kind = kind;
        SetID(data.userId);

        context.SetValue("UserName", data.userName);
        context.SetValue("ThumbnailIcon", persistent.ResourceManager.ImageContainer.Get("loadingpeople"));

        if (!string.IsNullOrEmpty(data.thumbnail))
        {
            persistent.APIManager.DownLoadTexture(data.thumbnail, (sprite) =>
            {
                context.SetValue("ThumbnailIcon", sprite);
            });
        }

        if (data.IsOnline())
        {
            context.SetValue("OnlineIcon", persistent.ResourceManager.ImageContainer.Get("peopleonline"));
        }
        else
        {
            context.SetValue("OnlineIcon", persistent.ResourceManager.ImageContainer.Get("peopleoffline"));
        }
    }

    private void OnClickButton()
    {
        context.onClickProfile -= OnClickButton;

        if (data == null)
        {
            context.onClickProfile += OnClickButton;
            return;
        }
        peopleManager.GetOhterUser(data.userId, OnGetOhterUserSuccess, OnGetOhterUserFailed);
    }

    public void OnGetOhterUserSuccess(string message)
    {
        context.onClickProfile += OnClickButton;

        LocalPlayerData playerData = JsonUtility.FromJson<LocalPlayerData>(message);

        PeopleView peopleView = UIView.Get<PeopleView>();
        PeopleAboutView aboutView = UIView.Get<PeopleAboutView>();
        ProfileAvatarView avatarView = UIView.Get<ProfileAvatarView>();

        peopleView.Set(true, true);

        //Test
        if (this.kind == PeopleAboutView.ProfileKind.NotFriend)
        {
            //aboutView.Set(playerData, this.kind);
        }
        else
        {
            aboutView.Set(playerData);
        }

        avatarView.Set(false, playerData.GetCustomAvatar().GetCustomPlusParts());

        if (!aboutView.AlreadyExist(avatarView))
        {
            aboutView.PushNavigation(avatarView);
        }

        peopleView.Push("", false, true, null, aboutView);
    }

    public void OnGetOhterUserFailed(string message)
    {
        context.onClickProfile += OnClickButton;
        uIManager.PushNotify(message, 2f);
    }
}
