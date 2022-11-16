using AkilliMum.SRP.Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using MindPlus;


public class MyRoomManager : MindPlus.RoomManager
{
    public override IEnumerator Initialize(GameManager gameManager)
    {
        yield return null;
        this.uIManager = gameManager.Persistent.UIManager;
        /*
        joinInteractionTarget.clickEvent.RemoveAllListeners();
        joinInteractionTarget.clickEvent.AddListener(() =>
            {
                if (uIManager.GetPool(StringTable.UIContentPool).TryGetActivePool<UIContent>("commcenter", out UIContent data))
                {
                    gameManager.Persistent.RoomDataBaseManager.RoomAPIHandler.JoinRoomAuto(data.data);
                }
            });
        */
      MainView mobileControllerView = UIView.Get<MainView>();
        mobileControllerView.Reset();
    }

    public override void OnJoinedRoom(MindPlusPlayer localPlayer)
    {
        base.OnJoinedRoom(localPlayer);
        uIManager.Push("", false, false, null, /*UIView.Get<MobileControllerView>()*/UIView.Get("MainView"));
    }

}
