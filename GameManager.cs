using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MindPlus
{
    public class GameManager : MonoBehaviour, LoadSceneManager.IEventHandler
    {
        [HideInInspector]
        public bool isInitalized = false;
        public bool isSlashEnd = false;
        public static bool isLoggingOut = false;

        private static GameManager instance;
        public Camera SceneCamera { private set; get; }
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<GameManager>();
                }
                return instance;
            }
        }
        public LoadSceneManager LoadSceneManager { private set; get; }
        public Persistent Persistent { private set; get; }
        public CultureInfo CultureInfo { private set; get; }
        public SplashView splashView;
        private DateTime lastOperationTime;

        private void Awake()
        {
            //if (GameManager.Instance != this)
            //{
            //    Destroy(this);
            //    return;
            //}

            if (!isInitalized)
            {
                lastOperationTime = DateTime.Now;
                UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
                CultureInfo = new CultureInfo("en-US");
                Screen.orientation = ScreenOrientation.LandscapeLeft;

                //Screen.SetResolution(1920, 1080, true);
                CreateLoadSceneManager();

                LoadSceneManager.Resister(this);

                StartCoroutine(Initialize());
            }
        }

        private void FixedUpdate()
        {
            
        }
#if UNITY_EDITOR
        //private int day = 1;
        //private DateTime today;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Photon.Pun.PhotonNetwork.DestroyAll();
            }
            
            ////test
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    AudioManager.Instance.SetEffectSound((AudioManager.Effect)Random.Range(0,17));
            //}
            //if (Input.GetKeyDown(KeyCode.F1))
            //{
            //    NetworkManager.Instance.currentRoomManager.player.SetSelfOnly();
            //}
            //if (Input.GetKeyDown(KeyCode.Keypad0))
            //{
            //    today = DateTime.Now;
            //    Persistent.ChallengeManager.PostAchievementActivity("CGST", today.AddDays(day));
            //    day++;
            //}
            //if (Input.GetKeyDown(KeyCode.Keypad1))
            //{
            //    today = DateTime.Now;
            //    Persistent.ChallengeManager.PostAchievementActivity("ET30", today.AddDays(day));
            //    day++;
            //}
            //if (Input.GetKeyDown(KeyCode.Keypad2))
            //{
            //    today = DateTime.Now;
            //    Persistent.ChallengeManager.PostAchievementActivity("FRIV", today.AddDays(day));
            //    day++;
            //}

            //if (Input.GetKeyDown(KeyCode.Keypad3))
            //{
            //    today = DateTime.Now;
            //    Persistent.ChallengeManager.PostAchievementActivity("FRNW", today.AddDays(day));
            //    day++;
            //}
            //if (Input.GetKeyDown(KeyCode.Keypad4))
            //{
            //    today = DateTime.Now;
            //    Persistent.ChallengeManager.PostAchievementActivity("MNGM", today.AddDays(day));
            //    day++;
            //}
            //if (Input.GetKeyDown(KeyCode.Keypad5))
            //{
            //    today = DateTime.Now;
            //    Persistent.ChallengeManager.PostAchievementActivity("SC10", today.AddDays(day));
            //    day++;
            //}
            //if (Input.GetKeyDown(KeyCode.Keypad6))
            //{
            //    today = DateTime.Now;
            //    Persistent.ChallengeManager.PostAchievementActivity("WRD5", today.AddDays(day));
            //    day++;
            //}
        }
#endif

        public void SetSceneCamera(Camera camera)
        {
            SceneCamera = camera;
        }

        private IEnumerator Initialize()
        {
            Scene current = SceneManager.GetSceneByName("Title");
            foreach (var root in current.GetRootGameObjects())
            {
                if (root.TryGetComponent(out Camera camera))
                {
                    this.SceneCamera = camera;
                    break;
                }
                if (root.TryGetComponent(out GameRoot gameRoot))
                {
                    this.SceneCamera = gameRoot.mainCamera;
                    break;
                }
            }


            if (!isLoggingOut)
            {
                yield return LoadSceneManager.AdditiveSceneAsync("Persistent", true, (scene) =>
                {
                    foreach (var rootObject in scene.GetRootGameObjects())
                    {
                        if (rootObject.TryGetComponent(out Persistent component))
                        {
                            Persistent = component;
                            break;
                        }
                    }

                    SceneManager.MoveGameObjectToScene(this.gameObject, scene);

                    SceneManager.SetActiveScene(scene);
                });
            }
            else
            {
                Scene temp = NetworkManager.Instance.gameObject.scene;
                foreach (var rootObject in temp.GetRootGameObjects())
                {
                    if (rootObject.TryGetComponent(out Persistent component))
                    {
                        Persistent = component;
                        break;
                    }
                }

                SceneManager.MoveGameObjectToScene(this.gameObject, temp);

                SceneManager.SetActiveScene(temp);
            }


            yield return Persistent.Initialize(LoadSceneManager, isLoggingOut);
            isInitalized = true;

            SceneManager.SetActiveScene(current);

            foreach (var root in current.GetRootGameObjects())
            {
                if (root.TryGetComponent(out RoomManager roomManager))
                {
                    Persistent.NetworkManager.ResistEvent(roomManager);
                    //Debug.Log("a NetworkManager.ResistEvent : " + roomManager.gameObject.name);
                    yield return roomManager.Initialize(this);
                    break;
                }
            }
            if (splashView)
            {
                yield return new WaitUntil(() => splashView.isMinRun);
                splashView.FadeOut();
            }
        }

        private void CreateLoadSceneManager()
        {
            GameObject gameObject = new GameObject("LoadSceneManager");
            this.LoadSceneManager = gameObject.AddComponent<LoadSceneManager>();
            gameObject.transform.SetParent(this.transform);
        }

        public void OnStartSceneChanged(Scene prev)
        {
            foreach (var root in prev.GetRootGameObjects())
            {
                if (root.TryGetComponent(out RoomManager roomManager))
                {
                    Persistent.NetworkManager.UnResistEvent(roomManager);
                    break;
                }
            }
        }

        public IEnumerator OnEndScenechanged(Scene next)
        {
            foreach (var root in next.GetRootGameObjects())
            {
                if (root.TryGetComponent(out Camera camera))
                {
                    this.SceneCamera = camera;
                    break;
                }

                if (root.TryGetComponent(out GameRoot gameRoot))
                {
                    this.SceneCamera = gameRoot.mainCamera;
                    break;
                }
            }

            foreach (var root in next.GetRootGameObjects())
            {
                if (root.TryGetComponent(out RoomManager roomManager))
                {
                    /*
                    if (roomManager.hasReadyScene)
                    {
                        AsyncOperation readyAsyncOperation = SceneManager.LoadSceneAsync("Ready", LoadSceneMode.Additive);
                        readyAsyncOperation.allowSceneActivation = true;

                        while (!readyAsyncOperation.isDone)
                        {
                            yield return null;
                        }
                        roomManager.readySceneManager = GameObject.FindObjectOfType<ReadySceneManager>();
                        roomManager.readySceneManager.Initialize(roomManager);
                    }
                    */
                    Persistent.NetworkManager.ResistEvent(roomManager);
                    yield return roomManager.Initialize(this);
                }
            }
        }

        public void OnApplicationQuit()
        {
            Persistent.AccountManager.DisconnectSocket();
        }
        private void OnApplicationPause(bool pause)
        {
            if (!isInitalized)
            {
                return;
            }
            if (pause)
            {
                this.lastOperationTime = DateTime.Now;
            }
        }
        private void OnApplicationFocus(bool focus)
        {
            if (!isInitalized)
            {
                return;
            }
            if (focus)
            {
                if((DateTime.Now - lastOperationTime).TotalMinutes >= 10)
                {
                    Persistent.AccountManager.DisconnectSocket();
                }
            }
        }






        #region "Test??"
        public void CreateCharacter()
        {
            MindPlusPlayer player = Persistent.PhotonInstantiation.SpawnPlayer(new BuilderSetting(false, true, true, false));

            player.Initialize(this, false, true);
            player.transform.SetParent(null);
            player.transform.localPosition = new Vector3(0, 10f, 0);
            player.transform.localRotation = Quaternion.Euler(Vector3.zero);
            player.GetPart<LookAtCanvas>().setPlayerNick.transform.localRotation = new Quaternion(0, 180f, 0, 0);
            Persistent.UIManager.Show("", false, UIView.Get<MainView>());
        }

        #endregion
    }

}
