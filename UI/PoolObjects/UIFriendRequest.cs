using MindPlus.Contexts.Master.Menus.PeopleView;
using Slash.Unity.DataBind.Core.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UIFriendRequest : UIPoolObject
{
    [HideInInspector]
    public UIFriendRequestContext context;
    public ContextHolder contextHolder;
    public PeopleData data;
    public RectTransform rectTransform;
    private PeopleManager peopleManager;
    private bool isClick = false;
    public Color color;
    public bool Equal(string userId)
    {
        if (data.userId == userId)
        {
            return true;
        }
        return false;
    }

    public void Initialize(Persistent persistent, PeopleData data)
    {
        isClick = false;
        this.rectTransform = GetComponent<RectTransform>();
        this.peopleManager = persistent.PeopleManager;
        this.data = data;
        SetID(data.userId);

        this.context = new UIFriendRequestContext(true);
        contextHolder.Context = context;

        context.SetValue("UserName", data.userName);
        context.SetValue("DescText", data.description); //todo
        context.SetValue("ThumbnailIcon", persistent.ResourceManager.ImageContainer.Get("loadingpeople"));

        if (!string.IsNullOrEmpty(data.thumbnail))
        {
            persistent.APIManager.DownLoadTexture(data.thumbnail, (sprite) =>
            {
                context.SetValue("ThumbnailIcon", sprite);
            });
        }

        context.onClickDelete = () =>
        {
            if (isClick) return;

            peopleManager.DelRequestFriend(data.userId);
            isClick = true;
        };

        //버튼 텍스트 색상과 아이콘 사용할지? TODO
        context.SetValue("AcceptText", color);
        context.onClickAccept = () =>
        {
            if (isClick) return;

            context.SetValue("AcceptText", Color.white);
            peopleManager.PostAcceptFriend(data.userId);
            isClick = true;
        };
    }
}
