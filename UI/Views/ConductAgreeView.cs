using MindPlus.Contexts.TitleView;
using MindPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConductAgreeView : UIView
{
    private ConductAgreeViewContext context;
    private UIManager mainUIManager;
    private RoomAPIHandler roomAPI;
    private LocalPlayerData playerData;
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)

    {
        base.Initialize(persistent, uIManager);
        this.mainUIManager = persistent.UIManager;
        this.roomAPI = persistent.RoomDataBaseManager.RoomAPIHandler;
        this.playerData = persistent.AccountManager.PlayerData;
        this.context = new ConductAgreeViewContext();
        ContextHolder.Context = context;

        context.onClickPlay += OnClickPlay;
        //persistent.UIManager.MasterContext.MenuViewContext.
    }
    public override void OnStartShow()
    {
        base.OnStartShow();
        GameObject.FindObjectOfType<CustomCameraContoller>().ActiveVirtualCamera(CustomCameraContoller.VirtualCamera.Body);
    }
    private void OnClickPlay()
    {
        context.onClickPlay -= OnClickPlay;

        if (!context.IsAgree)
        {
            mainUIManager.PushNotify("Please Agree to the Code of Conduct", 5f);
            context.onClickPlay += OnClickPlay;
            return;
        }

        NotifyView notify = Get<NotifyView>();
        mainUIManager.Hide(notify.name, false, null, notify);

        roomAPI.JoinRoom(playerData.myRoomId, "myroom#private");
    }
}
