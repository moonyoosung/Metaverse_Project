
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using MindPlus;
using UnityEngine.XR;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.ProBuilder.Shapes;

public class NetworkManager : MonoBehaviourPunCallbacks, IInRoomCallbacks, RoomDataBaseManager.IEventHandler
{
    public interface IEventHandler
    {
        void OnConnected();
        void OnConnectedToMaster();
        void OnRegionListReceived(RegionHandler regionHandler);
        void OnDisconnected(DisconnectCause cause);
        void OnJoinedLobby();
        void OnLeftLobby();
        void OnCreatedRoom(bool isMasterClient);
        void OnCreateRoomFailed(short returnCode, string message);
        void OnJoinedRoom(MindPlusPlayer localPlayer);
        void OnJoinRandomFailed(short returnCode, string message);
        void OnJoinRoomFailed(short returnCode, string message);
        void OnLeftRoom(bool isMasterClient);
        void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics);
        void OnMasterClientSwitched(Player newMasterClient);
        void OnRoomListUpdate(List<RoomInfo> roomList);
        void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged);
        void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps);
        void OnPlayerEnteredRoom(Player newPlayer, bool isMasterClient);
        void OnPlayerLeftRoom(Player otherPlayer, bool isMasterClient);
    }

    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<NetworkManager>();
            return instance;
        }
    }

    private PhotonInstantiation photonInstantiation;
    private List<IEventHandler> eventHandlers = new List<IEventHandler>();

    public MindPlus.RoomManager currentRoomManager;
    private AccountManager accountManager;
    private RoomDataBaseManager roomDataBaseManager;
    public void Initialize(RoomDataBaseManager firebaseManager, PhotonInstantiation photonInstantiation, AccountManager accountManager)
    {
        this.photonInstantiation = photonInstantiation;

        this.accountManager = accountManager;

        roomDataBaseManager = firebaseManager;
        firebaseManager.ResistEventHandler(this);

        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.AddCallbackTarget(this);
    }
    public void ResistEvent(IEventHandler eventHandler)
    {
        eventHandlers.Add(eventHandler);
        MindPlus.RoomManager roomManager = eventHandler as MindPlus.RoomManager;
        if (roomManager != null)
        {
            this.currentRoomManager = roomManager;
        }
    }

    public void UnResistEvent(IEventHandler eventHandler)
    {
        eventHandlers.Remove(eventHandler);
    }

    public int GeteventHandlerCount()
    {
        return eventHandlers.Count;
    }

    public List<IEventHandler> GetEventHandlers()
    {
        return eventHandlers;
    }
    
    public AccountManager GetAccountManager()
    {
        return accountManager;
    }

    public MindPlusPlayer SpawnPlayer(Vector3 pos)
    {
        MindPlusPlayer player = photonInstantiation.SpawnPlayer(new BuilderSetting(true, true, true, true));
        player.transform.localPosition = pos;
        //????????
        player.Initialize(GameManager.Instance, true, true);

        return player;
    }

    #region "PhotonNetwork"
    public override void OnConnected()
    {
        //PhotonNetwork.ConnectUsingSettings() ?? ???????? ?????? ????
        Debug.Log("------------OnConnected------------" + PhotonNetwork.CloudRegion);

        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnConnected();
        }
    }
    public override void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.Log("------------OnRegionListReceived------------");
        // NameServer?? ???????? ???? ??
        //PhotonNetwork.ConnectToRegion("us");
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnRegionListReceived(regionHandler);
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("------------OnDisconnected------------");
        //?????? ????????
        //PhotonNetwork.Disconnect();

        //?? ?????? ?????? ??????
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnDisconnected(cause);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("------------OnConnectedToMaster------------");
        //Master?? ?????? OnConnected ???? ???????? Master?? ?????????? ??????


        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnConnectedToMaster();
        }

    }

    public override void OnJoinedLobby()
    {
        Debug.Log("------------OnJoinedLobby------------");
        //?????? ???????? ?? ????
        //OnConnectedToMaster ???? PhotonNetwork.JoinLobby(); ?????? ???????? ?????? ???????? ?????? ??
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnJoinedLobby();
        }
    }

    public override void OnLeftLobby()
    {
        Debug.Log("------------OnLeftLobby------------");
        //PhotonNetwork.LeaveLobby();?? ???????? ???????? ???????? ???????? ?????? ????
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnLeftLobby();
        }
    }

    public override void OnCreatedRoom()
    {
        //?????? ?????? ???????????? ?????? ???? ???????? ?????? ?? ????
        Debug.Log("------------OnCreatedRoom------------");
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnCreatedRoom(PhotonNetwork.IsMasterClient);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //?? ?????? ???? (ex: ???? ???????? ???? ???????? ?? ??)
        Debug.Log("------------OnCreateRoomFailed------------");
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnCreateRoomFailed(returnCode, message);
        }
        OnFailed();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("------------OnJoinedRoom------------");
        //???? ???????? ??

        PhotonNetwork.LocalPlayer.NickName = accountManager.PlayerData.userName;
        currentRoomManager.SetPlayerProperties(PhotonNetwork.LocalPlayer.ActorNumber.ToString(), accountManager.PlayerData.userId);

        MindPlusPlayer localPlayer = SpawnPlayer(new Vector3(0, 1f, 0));

        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnJoinedRoom(localPlayer);
        }
        GameManager.Instance.Persistent.VoiceManager.ControlTotalVolumeSpeakr();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("------------OnJoinRandomFailed------------" + "\n" + message);
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnJoinRandomFailed(returnCode, message);
        }
        OnFailed();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("------------OnJoinRoomFailed------------" + "\n" + message);

        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnJoinRoomFailed(returnCode, message);
        }
        OnFailed();
    }

    public bool isLeft = true;

    public override void OnLeftRoom()
    {
        Debug.Log("------------OnLeftRoom------------");

        //???? ?????? ?????? ?????? ?????? ???????? ????
        //???????? ???? ?????? ?? ???????? ????

        //?????????? ????
        //PhotonNetwork.NetworkingClient.ConnectToNameServer();
        Debug.Log("eventHandlers.Count : " + eventHandlers.Count);

        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnLeftRoom(PhotonNetwork.IsMasterClient);
        }
        isLeft = true;
        //PhotonChat.Instance.Disconnect();
    }
    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        Debug.Log("------------OnLobbyStatisticsUpdate------------");

        //???? ?????? ???????? ?????? PhotonServerSettings - Lobby Statistilcs ??????
        // ?????? ???????? ???????? ?????? ?? ?? ????

        /*
         ?????? ?????? ???????? ?????? ???? ????
        PhotonNetwork.JoinLobby(new TypedLobby("dduck", LobbyType.Default));
        for (int i = 0; i < lobbyStatistics.Count; i++)
        {
            Debug.Log(lobbyStatistics[i].Name + ", " + lobbyStatistics[i].PlayerCount);
        }
         */
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnLobbyStatisticsUpdate(lobbyStatistics);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("------------OnMasterClientSwitched------------");
        //???? ???? ??, ?????? ?????? ??????
        //???? ?????? ????
        //PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[0]);
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnMasterClientSwitched(newMasterClient);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnRoomListUpdate(roomList);
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        //???? ???? ??, ?? ???? ?????? ???? ??????????????
        Debug.Log("------------OnRoomPropertiesUpdate------------");

        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnRoomPropertiesUpdate(propertiesThatChanged);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("------------OnPlayerPropertiesUpdate------------");
        //???? ???? ??, ???????? ???? ?????? ???? ?????????? ????
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("------------OnPlayerEnteredRoom------------");

        //???? ???? ??, ?????? ?????????? ??????

        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnPlayerEnteredRoom(newPlayer, PhotonNetwork.IsMasterClient);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.Persistent.VoiceManager.NewPlayerApply(newPlayer);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log("------------OnPlayerLeftRoom------------");

        //???? ???? ??, ?????????? ????
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnPlayerLeftRoom(otherPlayer, PhotonNetwork.IsMasterClient);
        }
    }
    #endregion
    public void OnStartJoin(ContentData next)
    {
        //Must Have Refactoring 
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.IsMessageQueueRunning = true;
            PhotonNetwork.ConnectUsingSettings();
            StartCoroutine(FirstConnect(next));
        }
        else
        {
            StartCoroutine(FirstConnect(next));
        }
    }
    private IEnumerator FirstConnect(ContentData next)
    {
        while (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
        {
            yield return null;
        }

        PhotonNetwork.JoinOrCreateRoom(next.roomId, new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)next.maxPlayers,
            CleanupCacheOnLeave = true
        }, null);

        GameManager.Instance.Persistent.PhotonChat.Connect(accountManager.PlayerData.userId, next.roomId);
    }

    public void OnADDContent(ContentData data, string category) { }

    public void OnRemovedContent(ContentData data) { }

    private void OnApplicationFocus(bool focus)
    {
        //if (focus)
        //{
        //    StopCoroutine(DoApplicationFocus());
        //    StartCoroutine(DoApplicationFocus());
        //}
        //else
        //{
        //}
    }

    IEnumerator DoApplicationFocus()
    {
        int count = 3;

        bool isDisconnected = false;
        while (count > 0)
        {
            if (PhotonNetwork.IsConnected == false || PhotonNetwork.NetworkClientState != ClientState.Joined)
            {
                isDisconnected = true;
                break;
            }
            yield return new WaitForSeconds(0.1f);
            count--;
        }

        if (isDisconnected)
        {
            OnFailed();
            yield break;
            //if (PhotonNetwork.Reconnect() && roomDataBaseManager != null && !string.IsNullOrEmpty(accountManager.PlayerData.userId))
            //{
            //    Debug.Log("DoApplicationFocus a");
            //    roomDataBaseManager.RoomAPIHandler.GetRoom(accountManager.PlayerData.myRoomId);
            //}
            //else
            //{
            //    PhotonNetwork.ConnectUsingSettings();
            //    float time = Time.time;
            //    bool isTimeOut = false;
            //    while (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
            //    {
            //        yield return null;

            //        if (Time.time - time > 10)
            //        {
            //            isTimeOut = true;
            //            break;
            //        }
            //    }
            //    if (isTimeOut)
            //    {
            //        Debug.Log("DoApplicationFocus b");
            //        OnFailed();
            //    }
            //    else
            //    {
            //        Debug.Log("DoApplicationFocus c");
            //        roomDataBaseManager.RoomAPIHandler.GetRoom(accountManager.PlayerData.myRoomId);
            //    }
            //}
        }
    }


    void OnFailed()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Title");
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //NetworkManager.Instance.isLeft = true;
            roomDataBaseManager.RoomAPIHandler.GetRoom(roomDataBaseManager.Current.roomId);
            // photonView.RPC(nameof(DestroyAll), RpcTarget.MasterClient);
        }
    }

    void DestroyAll()
    {
        PhotonNetwork.DestroyAll();
    }
#endif

}
