using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyRoomManager : MindPlus.RoomManager
{
    public override IEnumerator Initialize(MindPlus.GameManager gameManager)
    {
        yield return StartCoroutine(base.Initialize(gameManager));
        yield return null;

        MainView mobileControllerView = UIView.Get<MainView>();
        mobileControllerView.Reset();
    }

    public override void OnJoinedRoom(MindPlusPlayer localPlayer)
    {
        base.OnJoinedRoom(localPlayer);
        uIManager.Push("", false, false, null, UIView.Get("MainView"));
        //MindPlus.GameManager.Instance.Persistent.UIManager.Push("", true, false, false, /*UIView.Get<MobileControllerView>()*/UIView.Get("MainView"));
    }

    public override void OnLeftRoom(bool isMasterClient)
    {
        base.OnLeftRoom(isMasterClient);

        if (isMasterClient)
        {

        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var audio in GameObject.FindObjectsOfType<InteractionTargetAudio>())
            {
                if (audio.isPlaying)
                    audio.BtnNext();
            } 
        }
    }
}
