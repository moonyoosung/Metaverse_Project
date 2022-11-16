using Slash.Unity.DataBind.Core.Presentation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MindPlus;
using System;
using System.Globalization;
using System.Collections.Generic;
using MindPlus.Contexts.Pool;
using Sirenix.OdinInspector;

public class UIContent : UIPoolObject
{
    [HideInInspector]
    public UIContentContext context;
    public ContextHolder contextHolder;

    [ShowInInspector]
    public ContentData data;
    private RoomDataBaseManager roomDataManager;
    private AccountManager accountManager;
    private UIManager uiManager;
    public override void Initialize(Transform parent)
    {
        base.Initialize(parent);
        this.context = new UIContentContext();
        contextHolder.Context = context;
        context.onClickAbout += OnClickButton;
    }
    public override void InActivePool()
    {
        base.InActivePool();
        data = null;        
    }
    public void Set(Persistent persistent, ContentData data, ThumbnailData thumbnailData)
    {
        this.data = data;
        this.roomDataManager = persistent.RoomDataBaseManager;
        this.accountManager = persistent.AccountManager;
        this.uiManager = persistent.UIManager;
        SetID(data.roomId);

        context.SetValue("RoomName", data.roomName);

        if (!string.IsNullOrEmpty(data.thumbnail))
        {
            context.SetValue("ThumbnailIcon", persistent.ResourceManager.ImageContainer.Get("loadingcontent"));

            persistent.APIManager.DownLoadTexture(data.thumbnail, (sprite) =>
            {
                context.SetValue("ThumbnailIcon", sprite);
            });
        }
        else
        {
            Sprite thumb = thumbnailData.Get(data.sceneName).thumbnail;
            context.SetValue("ThumbnailIcon", thumb);
        }

        //Debug.Log("Initalize ContentBoxUI : " + data.roomName + " / " + data.roomType + "생성");
        context.SetValue("InfoIconActive", true);
        context.SetValue("LiveIconActive", true);
        context.SetValue("InfoTextActive", true);

        string lowerRoomType = data.roomType.ToLower();

        if (lowerRoomType.Contains(ContentTypes.Play))
        {
            if (!lowerRoomType.Contains("private"))
            {
                //Play Info 오브젝트 설정
            }
        }
        else if (lowerRoomType.Contains(ContentTypes.Location))
        {
            if (!lowerRoomType.Contains("private"))
            {
                context.SetValue("InfoIconActive", false);
                context.SetValue("LiveIconActive", false);
                context.SetValue("InfoTextActive", false);
            }
        }
        else if (lowerRoomType.Contains(ContentTypes.Event))
        {
            if (!lowerRoomType.Contains("private"))
            {
                // Open되어 있으면 인원수가 뜨고 Close되어 있으면 시간이랑 달력표시
                EventContentData eventData = this.data as EventContentData;
                Sprite liveIcon;
                Sprite infoIcon;
                string infoText;

                if (eventData.IsOpen())
                {
                    liveIcon = persistent.ResourceManager.ImageContainer.Get("contentonline");
                    infoIcon = persistent.ResourceManager.ImageContainer.Get("contentplayer");
                    infoText = eventData.numPlayers + "/" + eventData.maxPlayers;
                }
                else
                {
                    liveIcon = persistent.ResourceManager.ImageContainer.Get("contentoffline");
                    infoIcon = persistent.ResourceManager.ImageContainer.Get("contentcalendar5");
                    DateTime openTime = eventData.GetTime();
                    infoText = string.Format("{0}:{1}, {2} {3}", openTime.ToString("HH"), openTime.ToString("mm"), openTime.Day, DateTimeFormatInfo.InvariantInfo.GetMonthName(openTime.Month));
                }

                context.SetValue("InfoIcon", infoIcon);
                context.SetValue("InfoText", infoText);
                context.SetValue("LiveIcon", liveIcon);
            }
        }

    }
    private void OnClickButton()
    {
        context.onClickAbout -= OnClickButton;

        if (data.roomType.ToLower().Contains(ContentTypes.Location))
        {
            if (data.roomId.Contains("myroom"))
            {
                roomDataManager.RoomAPIHandler.JoinRoom(accountManager.PlayerData.myRoomId, "myroom#private",
                    () =>
                    {
                        context.onClickAbout += OnClickButton;
                    },
                    (message) =>
                    {
                        uiManager.PushNotify(message, 2f);
                        context.onClickAbout += OnClickButton;
                    });
                return;
            }

            roomDataManager.RoomAPIHandler.JoinRoomAuto(data,
                () =>
                {
                    context.onClickAbout += OnClickButton;
                },
                (message) =>
                {
                    uiManager.PushNotify(message, 2f);
                    context.onClickAbout += OnClickButton;
                });
            return;
        }

        WorldView worldView = UIView.Get<WorldView>();
        AboutView aboutView = UIView.Get<AboutView>();
        worldView.Set(true, true, false, false, data.roomName);
        aboutView.Set(data);
        worldView.Push("", false, true, () => { context.onClickAbout += OnClickButton; }, aboutView);
    }
}
