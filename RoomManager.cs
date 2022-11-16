using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace MindPlus
{
    [System.Serializable]
    public class ArrayIntClass
    {
        [SerializeField]
        public int[] array;

        public ArrayIntClass(int max)
        {
            array = new int[max];
            Init();
        }

        public void Init()
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = -1;
        }

        public void SetArray(int[] array)
        {
            if (array == null)
                return;
            this.array = array;
        }

        public void Add(int value)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == value)
                    return;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < 0)
                {
                    array[i] = value;
                    break;
                }
            }
        }

        public void ChangeAt(int index, int value)
        {
            if (index < 0)
                return;
            array[index] = value;
        }

        public void Remove(int value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    array[i] = -1;
                    break;
                }
            }
        }

        public void RemoveAt(int index)
        {
            array[index] = -1;
        }

        public int GetIndex(int value)
        {
            int index = -1;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }

    [System.Serializable]
    public class ArrayStringClass
    {
        [SerializeField]
        public string[] array;

        public ArrayStringClass(int max)
        {
            array = new string[max];
            Init();
        }

        public void Init()
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = "";
        }

        public void SetArray(string[] array)
        {
            if (array == null)
                return;
            this.array = array;
        }

        public void Add(string value)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == value)
                    return;
            for (int i = 0; i < array.Length; i++)
            {
                if (string.IsNullOrEmpty(array[i]))
                {
                    array[i] = value;
                    break;
                }
            }
        }

        public void ChangeAt(int index, string value)
        {
            if (index < 0)
                return;
            array[index] = value;
        }

        public void Remove(string value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    array[i] = "";
                    break;
                }
            }
        }

        public void RemoveAt(int index)
        {
            array[index] = "";
        }

        public int GetIndex(string value)
        {
            int index = -1;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }


    [System.Serializable]
    public class RoomPlayers
    {
        public ArrayIntClass playerActorNumbers;
        public ArrayIntClass playerReadyCheck;
        
        public RoomPlayers(int max)
        {
            playerActorNumbers = new ArrayIntClass(max);
            playerReadyCheck = new ArrayIntClass(max);
        }
    }

    public enum PlayerProperties { PlayerActorNumbers, PlayerReadyCheck, PlayerNick, PlayerID, GameNameArray }
    public abstract class RoomManager : MonoBehaviourPun, NetworkManager.IEventHandler
    {
        public float spawnScale = 1f;
        public Vector3 spawnOffset = Vector3.up * 1.315f;
        public MindPlusPlayer player;
        public RoomPlayers roomPlayers;
        public int maxPlayer = 4;

        public bool hasReadyScene = false;
        public bool isAutoResistInputAction = true;
        public ReadySceneManager readySceneManager;

        public Action<MindPlusPlayer> onJoinedRoom;
        public Action<Player, bool> onPlayerEnteredRoom;
        public Action<Player, bool> onPlayerLeftRoom;


        protected UIManager uIManager;
        protected RoomDataBaseManager roomDataManager;
        protected LoadSceneManager loadSceneManager;
        protected AccountManager accountManager;
        public Recorder recorder;

        public virtual IEnumerator Initialize(GameManager gameManager)
        {
            yield return null;
            this.uIManager = gameManager.Persistent.UIManager;
            this.roomDataManager = gameManager.Persistent.RoomDataBaseManager;
            this.loadSceneManager = gameManager.LoadSceneManager;
            this.accountManager = gameManager.Persistent.AccountManager;

            //uIManager.Push("", true, false, false, UIView.Get<MobileControllerView>());
        }

        public virtual void Awake()
        {
            //Debug.Log("Test Awake");
            roomPlayers = new RoomPlayers(maxPlayer);
#if UNITY_EDITOR
            AddRoomManagerTester();
#endif
        }

        public virtual void OnPlayerSpawn(MindPlusPlayer mindPlusPlayer)
        {

        }

        public virtual void CrashPlayer(MindPlusPlayer mindPlusPlayer)
        {
            RemovePlayerID(mindPlusPlayer.GetPhotonID());
        }

        public virtual void AddRoomManagerTester()
        {
            RoomManagerTester roomManagerTester = gameObject.AddComponent<RoomManagerTester>();
            roomManagerTester.roomManager = this;
        }



        public AccountManager GetAccountManager()
        {
            return accountManager;
        }

        public virtual IEnumerator GameRoomReady()
        {
            yield return null;
        }

        public string GetPropertiesName(int actorNum, string type)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(type);
            sb.Append("_");
            sb.Append(roomPlayers.playerActorNumbers.GetIndex(actorNum));
            return sb.ToString();
        }

        public virtual void AddPlayerID(int actorNum)
        {
            RenewRoomPlayers();
            if (roomPlayers.playerActorNumbers.array.Contains(actorNum))
                return;
            roomPlayers.playerActorNumbers.Add(actorNum);


            SetPlayerProperties(PlayerProperties.PlayerActorNumbers.ToString(), roomPlayers.playerActorNumbers.array);
            SetPlayerProperties(GetPropertiesName(actorNum, "userID"), "");
            SetPlayerProperties(GetPropertiesName(actorNum, "nick"), "");
            SetPlayerReady(actorNum, 0);
        }

        public virtual void RemovePlayerID(int actorNum)
        {
            RenewRoomPlayers();
            int index = roomPlayers.playerActorNumbers.GetIndex(actorNum);
            if (index < 0)
                return;
            SetPlayerProperties(GetPropertiesName(actorNum, "userID"), "");
            SetPlayerProperties(GetPropertiesName(actorNum, "nick"), "");
            roomPlayers.playerActorNumbers.RemoveAt(index);
            roomPlayers.playerReadyCheck.RemoveAt(index);

            SetPlayerProperties(PlayerProperties.PlayerReadyCheck.ToString(), roomPlayers.playerReadyCheck.array);
            SetPlayerProperties(PlayerProperties.PlayerActorNumbers.ToString(), roomPlayers.playerActorNumbers.array);
        }

        public void RenewRoomPlayers()
        {
            var ht = PhotonNetwork.CurrentRoom.CustomProperties;
            roomPlayers.playerActorNumbers.SetArray((int[])GetPlayerProperties(ht, PlayerProperties.PlayerActorNumbers.ToString()));
            roomPlayers.playerReadyCheck.SetArray((int[])GetPlayerProperties(ht, PlayerProperties.PlayerReadyCheck.ToString()));
        }

        public virtual void SetPlayerReady(int id, int value)
        {
            RenewRoomPlayers();
            int index = roomPlayers.playerActorNumbers.GetIndex(id);
            roomPlayers.playerReadyCheck.ChangeAt(index, value);
            SetPlayerProperties(PlayerProperties.PlayerReadyCheck.ToString(), roomPlayers.playerReadyCheck.array);
        }

        public virtual void  OnConnected()
        {
        }

        public virtual void  OnConnectedToMaster()
        {
        }

        public virtual void  OnCreatedRoom(bool isMasterClient)
        {
        }

        public virtual void  OnCreateRoomFailed(short returnCode, string message)
        {
        }

        public virtual void  OnDisconnected(DisconnectCause cause)
        {
        }

        public virtual void  OnJoinedLobby()
        {
        }

        public virtual void  OnJoinedRoom(MindPlusPlayer localPlayer)
        {
            player = localPlayer;

            if (onJoinedRoom != null)
                onJoinedRoom.Invoke(localPlayer);
        }

        public virtual void  OnJoinRandomFailed(short returnCode, string message)
        {
        }

        public virtual void  OnJoinRoomFailed(short returnCode, string message)
        {
        }

        public virtual void  OnLeftLobby()
        {
        }

        public virtual void  OnLeftRoom(bool isMasterClient)
        {
        }

        public virtual void  OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
        }

        public virtual void  OnMasterClientSwitched(Player newMasterClient)
        {
        }

        public virtual void  OnPlayerEnteredRoom(Player newPlayer, bool isMasterClient)
        {
            //�濡 ���� ��, ���ο� �÷��̾ ����
            if(onPlayerEnteredRoom != null)
                onPlayerEnteredRoom.Invoke(newPlayer, isMasterClient);
        }

        public virtual void  OnPlayerLeftRoom(Player otherPlayer, bool isMasterClient)
        {
            if (isMasterClient)
            {
                foreach (var item in GameObject.FindObjectsOfType<InteractionTarget>())
                {
                    if (item.targetUser.GetUser() == otherPlayer.ActorNumber)
                    {
                        item.TransferOwnership(-1);
                        break;
                    }
                }
            }
            if(onPlayerLeftRoom != null)
                onPlayerLeftRoom.Invoke(otherPlayer, isMasterClient);
        }

        public virtual void  OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
        }

        public virtual void  OnRegionListReceived(RegionHandler regionHandler)
        {
        }

        public virtual void  OnRoomListUpdate(List<RoomInfo> roomList)
        {
        }

        public virtual void  OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            //null�ΰ��?üũ�ؾ���
            roomPlayers.playerActorNumbers.SetArray((int[])GetPlayerProperties(propertiesThatChanged, PlayerProperties.PlayerActorNumbers.ToString()));
            roomPlayers.playerReadyCheck.SetArray((int[])GetPlayerProperties(propertiesThatChanged, PlayerProperties.PlayerReadyCheck.ToString()));
        }

        public void SetPlayerProperties(string key, object value)
        {
            Hashtable ht = new Hashtable { { key, value } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
        }

        public object GetPlayerProperties(Hashtable propertiesThatChanged, string key)
        {
            if (propertiesThatChanged.TryGetValue(key, out object result))
            {
                return result;
            }

            return null;
        }

        public int GetPlayerIndex(int playerActorNumber)
        {
            return roomPlayers.playerActorNumbers.GetIndex(playerActorNumber);
        }

        public void SetRoomTag(string key, string value)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { key, value } });
        }

        public string GetRoomTag(string key)
        {
            var value = PhotonNetwork.CurrentRoom.CustomProperties[key];
            return value == null ? "" : (string)value;
        }

        public string GetUserId(string key)
        {
            Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
            return properties.ContainsKey(key) ? (string)properties[key] : null;
        }

        public bool AllHasTag(string key)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.PlayerList[i].CustomProperties[key] == null)
                    return false;
            }
            return true;
        }
    }
}
