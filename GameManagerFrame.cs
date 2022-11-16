using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DateTime = System.DateTime;
using TMPro;
using Pixelplacement;
using Photon.Realtime;
using Sirenix.Utilities;
using UnityEditor.EditorTools;

namespace MindPlus.Game
{
    public class GameManagerFrameBase : MonoBehaviourPun
    {
        public CountDown countDown;
        public bool useGravity = true;
        public DateTime startTime;

        public virtual void Awake()
        {
            countDown = Instantiate<CountDown>(Resources.Load<CountDown>("Prefabs/UI/Canvas CountDown"));
            countDown.gameObject.SetActive(false);
            Resources.UnloadUnusedAssets();
        }

        public virtual void OnPlayerSpawn(MindPlusPlayer mindPlusPlayer)
        {

        }

        public virtual IEnumerator WaitGameInfoPanelEnd()
        {
            yield return null;
        }

        public virtual IEnumerator DoStartGame()
        {
            yield return null;
        }

        public virtual IEnumerator DoMasterSetting()
        {
            yield return null;
        }

        public virtual void CollisionAction(int id, string tag)
        {
        }
        public virtual IEnumerator DoAddGameData(MindPlusPlayer mindPlusPlayer)
        {
            yield return null;
        }
        public virtual void ContinueGame()
        {

        }
        public virtual void CrashPlayer(MindPlusPlayer mindPlusPlayer)
        {

        }
        public virtual void SendPlayerGameState(string userID, int state)
        {

        }
    }
    public class GameManagerFrame<T, U, V> : GameManagerFrameBase where T : GameDataFrame, new () where U : GameDataUIFrame<T>, new() where V : GamePlayerFrame<T>, new()
    {
        [Header("GameManagerFrame")]
        public MindPlus.MiniGameRoomManager roomManager;
        public bool isOrthographic = false;
        public List<U> gameDataUIs = new List<U>();
        public List<V> gamePlayers = new List<V>();
        public List<V> gamePlayerRanks = new List<V>();
        public PanelResult panelResult;
        public bool isJoystick = false;
        public bool isRotate = false;
        public GameObject lightObject;
        public GameObject bgmObject;
        public SetSkyBox setSkyBox;
        public Vector3 spawnOffset = Vector3.zero;

        [SerializeField]
        protected double time;


        public override void Awake()
        {
            base.Awake();
            roomManager = GameObject.FindObjectOfType<MiniGameRoomManager>();
            roomManager.gameManagerFrameBase = this;
            foreach (var item in gameDataUIs)
            {
                item.gameObject.SetActive(false);
            }
            lightObject.SetActive(false);
            gameDataUIs[0].transform.parent.gameObject.SetActive(false);
        }

        public IEnumerator InitializePanelResult()
        {
            yield return null;
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Joined);
            if (panelResult == null)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    panelResult = PhotonNetwork.InstantiateRoomObject("Prefabs/UI/Canvas GameResult", Vector3.zero, Quaternion.identity).GetComponent<PanelResult>();
                }
                else
                {
                    while (panelResult == null)
                    {
                        yield return null;
                        panelResult = GameObject.FindObjectOfType<PanelResult>();
                    }
                }
                panelResult.transform.SetParent(this.transform);
                panelResult.gameObject.SetActive(false);
            }
        }

        public override void CrashPlayer(MindPlusPlayer mindPlusPlayer)
        {
            base.CrashPlayer(mindPlusPlayer);

            OutPlayer(gamePlayers.Find((x) => x.gameData.id == mindPlusPlayer.GetPhotonID()), true);

            for (int j = gamePlayers.Count - 1; j >= 0; j--)
            {
                if (gamePlayers[j].mindPlusPlayer == mindPlusPlayer)
                    gameDataUIs[j].Initialize();
            }
        }

        public virtual void OutPlayer(V player, bool isCrash = false)
        {
            if (string.IsNullOrEmpty(player.gameData.nickName))
                return;
            if (!gamePlayerRanks.Contains(player))
            {
                player.isDead = true;
                if(!isCrash)
                    gamePlayerRanks.Add(player);
            }
            else if(isCrash)
            {
                gamePlayerRanks.Remove(player);
            }
        }

        public void SetController(bool isInitialize = false)
        {
            MainView mobileControllerView = UIView.Get<MainView>();
            if (isInitialize)
            {
                mobileControllerView.mobileRotateInput.gameObject.SetActive(false);
                mobileControllerView.mobileMoveInput.gameObject.SetActive(false);
                roomManager.player.GetPart<PlayerRig>().playerController.UnResistInputAction();
            }
            else
            {
                mobileControllerView.mobileRotateInput.gameObject.SetActive(isRotate);
                mobileControllerView.mobileMoveInput.gameObject.SetActive(isJoystick);

                if (isJoystick)
                {
                    roomManager.player.GetPart<PlayerRig>().playerController.ResistInputAction();
                }
            }
        }

        public virtual void OnReadyGame()
        {
            SetController(true);
            foreach (var item in GameObject.FindObjectsOfType<MindPlusPlayer>())
            {
                PlacePlayer(item);
            }
            SetLocalPlayerCollision();
            bgmObject.SetActive(true);
        }

        public virtual void OnStartGame()
        {
            SetController();

        }

        public void PlacePlayer(MindPlusPlayer player)
        {
            SpawnPos spawnPos = GameObject.FindObjectOfType<SpawnPos>();

            int id = player.GetPhotonID();
            int index = roomManager.roomPlayers.playerActorNumbers.GetIndex(id);

            if(roomManager.gameNumber <= 1)
            {
                player.transform.position = roomManager.readySceneManager.gameSpawnPos[index].position + roomManager.spawnOffset + spawnOffset;
                player.transform.rotation = roomManager.readySceneManager.gameSpawnPos[index].rotation;
            }
            else
            {
                player.transform.position = spawnPos.spawnPosList[index].position + roomManager.spawnOffset + spawnOffset;
                player.transform.rotation = spawnPos.spawnPosList[index].rotation;
            }
            player.gameEffect = GameObject.FindObjectOfType<MindPlus.Game.GameEffect>();
        }

        public virtual int GetSurvivorsCount()
        {
            int count = 0;
            foreach (var p in gamePlayers)
            {
                if (!p.isDead)
                    count++;
            }   
            return count;
        }

        public virtual bool CheckAllDead()
        {
            bool isAllDead = true;
            foreach (var p in gamePlayers)
            {
                isAllDead = isAllDead && p.isDead;
            }
            return isAllDead;
        }

        public void SetLocalPlayerCollision()
        {
            PlayerCollision playerCollision = roomManager.player.GetComponent<PlayerCollision>();
            if (playerCollision == null)
                playerCollision = roomManager.player.gameObject.AddComponent<PlayerCollision>();
            playerCollision.collisionAction = CollisionAction;
        }

        public override IEnumerator WaitGameInfoPanelEnd()
        {
            lightObject.SetActive(true);


            setSkyBox.Set();

            OnReadyGame();

            float waitEndTime = Time.time + 10;
            while (true)
            {
                bool isAllInGame = true;
                foreach (var player in gamePlayers)
                {
                    isAllInGame = isAllInGame && player.gameState == GamePlayerBase.GameState.INGAME;
                }
                if (isAllInGame || Time.time - waitEndTime >= 0)
                    break;
                yield return null;
            }

        }

        public override IEnumerator DoStartGame()
        {
            roomManager.player.rBody.useGravity = useGravity;
            gameDataUIs[0].transform.parent.gameObject.SetActive(true);
            countDown.onFinish = null;
            countDown.gameObject.SetActive(true);
            yield return null;
            yield return new WaitUntil(() => countDown.isStart);

            foreach (var item in GameObject.FindObjectsOfType<PlayerCameraSetting>())
            {
                item.Initialize();
            }
            Camera.main.orthographic = isOrthographic;
            OnStartGame();
        }

        //플레이어가 들어와서 spawn이 될 때까지 기다렸다가 ready상태로 변경 
        public IEnumerator SetMinePlayerReady(System.Action onMinePlayerSpawn = null)
        {
            yield return new WaitUntil(() => roomManager.player != null);

            int id = roomManager.player.GetPhotonID();
            while (roomManager.roomPlayers.playerActorNumbers.GetIndex(id) < 0)
            {
                yield return null;
            }

            if (onMinePlayerSpawn != null)
                onMinePlayerSpawn.Invoke();
        }


        public virtual void SetPlayer()
        {
            foreach (var item in GameObject.FindObjectsOfType<MindPlusPlayer>())
            {
                var player = gamePlayers.Find((x) => x.gameData.id == item.GetPhotonID());
                if (player != null)
                {
                    player.Initialize(item);
                    SetPlayerOthers(player);
                }
            }
        }
        public virtual void SetPlayerOthers(V player)
        {

        }
        public void SetStartTime(int addSeconds = 0)
        {
            startTime = DateTime.Now.AddSeconds(addSeconds);
            if (PhotonNetwork.IsMasterClient)
            {
                roomManager.SetPlayerProperties("startTime", startTime.ToString());
            }
        }

        public override void SendPlayerGameState(string userID, int state)
        {
            photonView.RPC(nameof(ReceivePlayerGameState), RpcTarget.AllBuffered, userID, state);
        }

        [PunRPC]
        public void ReceivePlayerGameState(string userID, int state)
        {
            var player = gamePlayers.Find((x) => x.gameData.userId == userID);
            if (player == null)
                return;
            player.gameState = (GamePlayerBase.GameState)state;
        }

        public override IEnumerator DoAddGameData(MindPlusPlayer mindPlusPlayer)
        {
            yield return null;

            var player = new V();
            player.gameState = GamePlayerBase.GameState.INREADY;
            player.SetGameData(mindPlusPlayer.GetPhotonID(), -1, "", "");
            gamePlayers.Add(player);

            var ht = PhotonNetwork.CurrentRoom.CustomProperties;

            string userId = (string)roomManager.GetPlayerProperties(ht, roomManager.GetPropertiesName(mindPlusPlayer.GetPhotonID(), "userID"));
            string nick = (string)roomManager.GetPlayerProperties(ht, roomManager.GetPropertiesName(mindPlusPlayer.GetPhotonID(), "nick"));

            int index = -1;
            for(int i=0;i< gamePlayers.Count; i++)
            {
                if(gamePlayers[i].gameData.id == player.gameData.id)
                {
                    index = i;
                    break;
                }
            }

            player.SetGameData(mindPlusPlayer.GetPhotonID(), index, userId, nick);
            player.Initialize(mindPlusPlayer);

            gameDataUIs[index].gameData = player.gameData;
            gameDataUIs[index].RenewUI();
            gameDataUIs[index].SetProfile();
        }

        public void GameOver()
        {
            photonView.RPC(nameof(RPCGameOver), RpcTarget.All);
        }

        [PunRPC]
        public void RPCGameOver()
        {
            OnGameOver();
        }

        public virtual void OnGameOver()
        {

            Resources.UnloadUnusedAssets();


            for(int i=gamePlayerRanks.Count - 1; i >= 0; i--)
            {
                if (gamePlayerRanks[i].mindPlusPlayer == null || string.IsNullOrEmpty(gamePlayerRanks[i].gameData.userId))
                    gamePlayerRanks.RemoveAt(i);
            }
            panelResult.Initialize(gamePlayerRanks.Count);

            roomManager.StartCoroutine(roomManager.DoGetNextGame(5.5f));
        }

        public void SetTime()
        {
            time = (DateTime.Now - startTime).TotalSeconds;
        }
    }
}