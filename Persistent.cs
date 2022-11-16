using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MindPlus;
using Photon.Pun;
using System;

public class Persistent : MonoBehaviour
{
    public APIManager APIManager { private set; get; }
    public AccountManager AccountManager { private set; get; }
    public PeopleManager PeopleManager { private set; get; }
    public RoomDataBaseManager RoomDataBaseManager { private set; get; }
    public ChallengeManager ChallengeManager { private set; get; }
    public HealthManager HealthManager { private set; get; }
    public NetworkManager NetworkManager { private set; get; }
    public ResourceManager ResourceManager { private set; get; }
    public PhotonChat PhotonChat { private set; get; }
    public UIAnimManager UIAnimationManager { private set; get; }
    public PluginManager PluginManager { private set; get; }
    public VoiceManager VoiceManager { private set; get; }
    public UIManager UIManager;
    public PhotonInstantiation PhotonInstantiation;
    public AudioManager AudioManager { private set; get; }

    public IEnumerator Initialize(LoadSceneManager loadSceneManager, bool isLoggingOut = false)
    {
        if (isLoggingOut)
        {
            RoomDataBaseManager.SetLoadSceneManager(loadSceneManager);
            yield break;
        }

        yield return CreateInstance();

        yield return InitializeInstance(loadSceneManager);
    }

    private IEnumerator CreateInstance()
    {
        CreateResourceManager();
        CreateUIAnimationManager();
        CreateAPIManager();
        CreateAccountManager();
        CreateRoomDataManager();
        CreatePeopleManager();
        CreateChallengeManager();
        CreateHealthManager();
        CreateNetworkBase();
        CreatePhotonChat();
        CreateVoiceManager();
        CreatePluginManager();

        yield return null;
    }


    private IEnumerator InitializeInstance(LoadSceneManager loadSceneManager)
    {
        APIManager.Initialize();
        AccountManager.Initialize(APIManager, RoomDataBaseManager);
        RoomDataBaseManager.Initalize(NetworkManager, AccountManager, loadSceneManager, APIManager);
        PeopleManager.Initialize(APIManager, AccountManager, ChallengeManager);
        NetworkManager.Initialize(RoomDataBaseManager, PhotonInstantiation, AccountManager);
        PluginManager.Initialize();
        ChallengeManager.Initialize(APIManager, AccountManager, ResourceManager.GetUIResources());
        HealthManager.Initialize(APIManager, AccountManager, PluginManager, ResourceManager);
        yield return UIAnimationManager.Initialize(ResourceManager.GetUIResources().StreamQueue.gameObject);
        yield return UIManager.Initialize(this);
        //VoiceManager.Initialize(NetworkManager, ResourceManager);
    }
    private void CreateHealthManager()
    {
        this.HealthManager = new GameObject("HealthManager").AddComponent<HealthManager>();
    }
    private void CreateChallengeManager()
    {
        this.ChallengeManager = new ChallengeManager();
    }
    private void CreateAccountManager()
    {
        this.AccountManager = new AccountManager();
    }
    private void CreateRoomDataManager()
    {
        this.RoomDataBaseManager = new RoomDataBaseManager();
    }
    private void CreatePeopleManager()
    {
        this.PeopleManager = new PeopleManager();
    }
    private void CreateNetworkBase()
    {
        GameObject networkBase = new GameObject("NetworkBase");
        networkBase.AddComponent<PhotonView>().ViewID = 999;
        NetworkManager = networkBase.AddComponent<NetworkManager>();
    }
    private void CreateResourceManager()
    {
        this.ResourceManager = new ResourceManager(PhotonInstantiation.platform);
    }

    private void CreatePhotonChat()
    {
        GameObject photonChat = new GameObject("PhotonChat");
        //networkBase.AddComponent<PhotonView>().ViewID = 2;
        PhotonChat = photonChat.AddComponent<PhotonChat>();
    }
    private void CreateAPIManager()
    {
        GameObject APIManagerobj = new GameObject("APIManager");
        APIManagerobj.transform.SetParent(transform);
        this.APIManager = APIManagerobj.AddComponent<APIManager>();
    }
    private void CreateUIAnimationManager()
    {
        GameObject UIAnimationobj = new GameObject("UIAnimationManager");
        UIAnimationobj.transform.SetParent(transform);
        this.UIAnimationManager = UIAnimationobj.AddComponent<UIAnimManager>();
    }

    private void CreateVoiceManager()
    {
        GameObject voiceManager = new GameObject("VoiceManager");
        VoiceManager = voiceManager.AddComponent<VoiceManager>();
    }
    private void CreatePluginManager()
    {
        GameObject pluginManager = new GameObject("PluginManager");
        this.PluginManager = pluginManager.AddComponent<PluginManager>();
    }
}
