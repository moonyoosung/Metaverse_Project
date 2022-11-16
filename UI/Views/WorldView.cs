using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MindPlus.Contexts.Master.Menus;
using System;
using MindPlus;

public class WorldView : UIFlatView
{
    public UIMaskSlider maskSlider;
    private WorldViewContext context;
    private Sprite worldIcon;
    private Sprite backIcon;
    private LocalPlayerData playerData;
    private RoomDataBaseManager roomDataManager;
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        ImageContainer imageContainer = persistent.ResourceManager.ImageContainer;
        worldIcon = imageContainer.Get("menuworld");
        backIcon = imageContainer.Get("menubackarrow");

        UIManager uiManager = uIManager as UIManager;
        this.context = new WorldViewContext();
        ContextHolder.Context = context;
        uiManager.MasterContext.MenuViewContext.WorldViewContext = context;
        this.roomDataManager = persistent.RoomDataBaseManager;
        this.playerData = persistent.AccountManager.PlayerData;

        context.onClickBack += OnClickBack;

        context.onClickCreate += OnClickCreate;
        context.onClickClose += OnClickClose;
        context.onClickJoin += OnClickJoin;
        maskSlider.onStart += OnStartSlide;
        context.onClickSlider += maskSlider.OnMove;
        context.SetValue("IsAlbe", false);
        Set(false);
    }

    private void OnClickClose()
    {
        context.onClickClose -= OnClickClose;
        UIManager.Pop("", false, false, true, null, () => { context.onClickClose += OnClickClose; });
    }

    private void OnClickCreate()
    {
        context.onClickCreate -= OnClickCreate;

        Pop(false, false, true, null, () => { context.onClickCreate += OnClickCreate; });
        Set(false);
    }

    private void OnClickBack()
    {
        context.onClickBack -= OnClickBack;

        Pop(false, false, true, () =>
        {
            if (navigation.Current is ContentListView)
            {
                Set(true);
            }
            else
            {
                Get<ContentListView>().ClearQueue();
                Set(false);
            }
        }, () =>
        {
            context.onClickBack += OnClickBack;
        });
    }

    public override void OnStartShow()
    {
        if ((bool)context.GetValue("IsActiveBalance"))
        {
            context.SetValue("CoinText", playerData.coin == 0 ? "0" : string.Format(Format.Money, playerData.coin));
            context.SetValue("HeartText", playerData.heart == 0 ? "0" : string.Format(Format.Money, playerData.heart));
        }
        base.OnStartShow();
        if (navigation.history.Count == 0)
        {
            context.onClickSlider.Invoke(0);
        }
        else
        {
            maskSlider.OnMove(0);
        }
    }
    public override void OnFinishHide()
    {
        if (context.IsTitleInteract)
        {
            Set(false);
        }
        base.OnFinishHide();
        if (navigation.history.Count > 0)
        {
            navigation.PopToRoot(0);
        }
        maskSlider.current = int.MaxValue;
    }
    private void OnStartSlide(int Current, int next)
    {
        if (Current == next)
        {
            return;
        }

        UIView[] views = new UIView[] { Get<RoomsView>(), Get<PlayView>(), Get<EventView>() };

        Push("", false, true, null, views[next]);
    }

    public void Set(bool isInteractable, bool isActiveJoin = false, bool isActiveCreate = false, bool isActiveBalance = true, string titleText = "World")
    {
        if (isInteractable)
        {
            context.SetValue("TitleIcon", backIcon);
            context.SetValue("IsActiveDivider", true);
        }
        else
        {
            context.SetValue("TitleIcon", worldIcon);
            context.SetValue("IsActiveDivider", false);
        }

        context.SetValue("IsActiveJoin", isActiveJoin);
        context.SetValue("IsActiveStarIcon", isActiveJoin);
        context.SetValue("IsActiveBalance", isActiveBalance);
        context.SetValue("IsActiveCreate", isActiveCreate);
        context.SetValue("SliderActive", !isInteractable);
        context.SetValue("IsTitleInteract", isInteractable);
        context.SetValue("TitleText", titleText);
    }

    private void OnClickJoin()
    {
        context.onClickJoin -= OnClickJoin;

        Set(false);

        ContentData data = (navigation.Current as AboutView).data;

        if (data.roomType.Contains(ContentTypes.Event))
        {
            roomDataManager.RoomAPIHandler.JoinRoom(data.roomId, "event#public", SuccessJoinRoom, FailedJoinRoom);
            return;
        }

        roomDataManager.RoomAPIHandler.JoinRoomAuto(data, SuccessJoinRoom, FailedJoinRoom);
    }
    private void SuccessJoinRoom()
    {
        context.onClickJoin += OnClickJoin;
    }
    private void FailedJoinRoom(string message)
    {
        context.onClickJoin += OnClickJoin;
        (UIManager as UIManager).PushNotify(message, 3f);
    }
}

