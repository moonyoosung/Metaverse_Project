using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

namespace MindPlus
{
    public class RoomDataBaseManager : NetworkManager.IEventHandler
    {
        public interface IEventHandler
        {
            void OnStartJoin(ContentData next);
            void OnADDContent(ContentData roomData, string category);

        }

        public ContentData Current
        {
            set { current = value; }
            get { return current; }
        }
        public RoomAPIHandler RoomAPIHandler { private set; get; }

        private ContentData current;
        private List<IEventHandler> eventHandlers = new List<IEventHandler>();

        private LoadSceneManager loadSceneManager;

        public void Initalize(NetworkManager networkBase, AccountManager accountManager, LoadSceneManager loadSceneManager, APIManager apiManager)
        {
            this.RoomAPIHandler = new RoomAPIHandler(this, apiManager, accountManager);

            this.loadSceneManager = loadSceneManager;

            networkBase.ResistEvent(this);

        }
        public void ResistEventHandler(IEventHandler eventHandler)
        {
            eventHandlers.Add(eventHandler);
        }
        public void UnResistEventHandler(IEventHandler eventHandler)
        {
            eventHandlers.Remove(eventHandler);
        }
        public void SetLoadSceneManager(LoadSceneManager loadSceneManager)
        {
            this.loadSceneManager = loadSceneManager;
        }
        #region"RoomDataEvent"
        public void OnStartJoin(ContentData nextRoom)
        {
            GameManager.Instance.StartCoroutine(ASynchJoin(nextRoom));
        }
        private IEnumerator ASynchJoin(ContentData nextRoom)
        {
            if (Current != null)
            {
                NetworkManager.Instance.isLeft = false;
                if (GameManager.Instance.Persistent.UIManager.GetPool(StringTable.UIContentPool).TryGetActivePool<UIContent>(Current.roomName.ToLower().Replace(" ", ""), out UIContent currentUIContent))
                {
                }
                GameManager.Instance.Persistent.UIManager.PopToRoot(null, 1, true, true);
                RoomAPIHandler.LeaveRoom(Current);

                if (PhotonNetwork.CurrentRoom == null)
                {
                    NetworkManager.Instance.isLeft = true;
                }
                else
                {
                    PhotonNetwork.LeaveRoom();
                }
            }

            // 데이터베이스 나가기 됐는지 기다리기
            while (Current != null)
            {
                yield return null;
            }

            while (!NetworkManager.Instance.isLeft)
            {
                yield return null;
            }

            Current = nextRoom;

  


            loadSceneManager.Change(nextRoom.sceneName, () =>
            {
                foreach (var eventHandler in eventHandlers)
                {
                    eventHandler.OnStartJoin(nextRoom);
                }
            }, false);

        }
 
        public void OnADDContent(ContentData data, string category)
        {
            foreach (var eventHandler in eventHandlers)
            {
                eventHandler.OnADDContent(data, category);
            }
        }

        #endregion

        #region "NetworkManagerEvent"

        public void OnConnected() { }
        public void OnConnectedToMaster() { }
        public void OnRegionListReceived(RegionHandler regionHandler) { }
        public void OnDisconnected(DisconnectCause cause) { }
        public void OnJoinedLobby() { }
        public void OnLeftLobby() { }
        public void OnCreatedRoom(bool isMasterClient) { }
        public void OnCreateRoomFailed(short returnCode, string message) { }
        public void OnJoinedRoom(MindPlusPlayer localPlayer)
        {
            loadSceneManager.UnloadLoading();
        }
        public void OnJoinRandomFailed(short returnCode, string message) { }
        public void OnJoinRoomFailed(short returnCode, string message) { }
        public void OnLeftRoom(bool isMasterClient) { }
        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics) { }
        public void OnMasterClientSwitched(Player newMasterClient) { }
        public void OnRoomListUpdate(List<RoomInfo> roomList) { }
        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) { }
        public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) { }
        public void OnPlayerEnteredRoom(Player newPlayer, bool isMasterClient) { }
        public void OnPlayerLeftRoom(Player otherPlayer, bool isMasterClient) { }


        #endregion
    }
}

