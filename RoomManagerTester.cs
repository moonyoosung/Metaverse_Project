using MindPlus;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomManagerTester : MonoBehaviour
{
    public RoomManager roomManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddPlayer();
        }
    }

    public void AddPlayer()
    {
        MindPlusPlayer localPlayer = NetworkManager.Instance.SpawnPlayer(Vector3.zero);

        //NetworkManager.Instance.currentRoomManager.SetPlayerProperties(PhotonNetwork.LocalPlayer.ActorNumber.ToString(), NetworkManager.Instance.GetAccountManager().PlayerData.userId);

        foreach (var eventHandler in NetworkManager.Instance.GetEventHandlers())
        {
            eventHandler.OnJoinedRoom(localPlayer);
        }
    }

    public void RemovePlayer()
    {

    }
}
