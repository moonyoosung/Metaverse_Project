using System.Collections;
using UnityEngine;
using MindPlus.Contexts.Master.Menus;
using MindPlus;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingView : UIFlatView
{
    public ScrollRect scroll;
    private SettingViewContext context;
    private AccountManager accountManager;
    private RoomDataBaseManager roomDataBaseManager;
    private Persistent persistent;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);

        UIManager uiManager = uIManager as UIManager;
        this.context = new SettingViewContext();
        ContextHolder.Context = context;
        uiManager.MasterContext.MenuViewContext.settingViewContext = context;
        this.persistent = persistent;
        this.accountManager = persistent.AccountManager;
        this.roomDataBaseManager = persistent.RoomDataBaseManager;

        scroll.content.localPosition = Vector3.zero;

        context.SetValue("CoinText", "21312");
        context.SetValue("HeartText", "3");
        context.SetValue("IsAlbe", false);

        context.onClickClose += OnClickClose;
        context.onClickLogout += OnClickLogout;
        context.onMusicToggleChanged += OnMusicToggleChanged;
        context.onMusicSliderValueChanged += OnMusicSliderValueChanged;
        context.onSoundToggleChanged += OnSoundToggleChanged;
        context.onSoundSliderValueChanged += OnSoundSliderValueChanged;
        context.onAmbienceToggleChanged += OnAmbienceToggleChanged;
        context.onAmbienceSliderValueChanged += OnAmbienceSliderValueChanged;
        context.onVoiceToggleChanged += OnVoiceToggleChanged;
        context.onVoiceSliderValueChanged += OnVoiceSliderValueChanged;
        context.onClickEmail += OnClickEmail;

        UnityEngine.Audio.AudioMixer  mixer = AudioManager.Instance.audioMixer;

        //오디오 믹스에 설정된 값으로 초기화
        if (mixer.GetFloat("Background", out float backgroundValue))
        {
            context.SetValue("MusicSliderValue", backgroundValue);
            AudioManager.Instance.SetAudioGroupValue("Meditation/Background", backgroundValue);
        }
        if (mixer.GetFloat("Effect", out float effectValue))
        {
            context.SetValue("SoundSliderValue", effectValue); 
            AudioManager.Instance.SetAudioGroupValue("Meditation/Effect", effectValue);
        }
        if (mixer.GetFloat("Ambience", out float ambienceValue))
        {
            context.SetValue("AmbienceSliderValue", ambienceValue);
        }
        if (mixer.GetFloat("Voice", out float voiceValue))
        {
            context.SetValue("VoiceSliderValue", voiceValue);
        }
        //context.SetValue("VoiceSliderValue", 1f);
    }

    private void OnClickEmail()
    {
        Application.OpenURL("mailto:careplay_cs@looxidlabs.com");
    }

    private void OnMusicToggleChanged(bool isMute)
    {
        Debug.Log("OnMusicToggleChanged" + isMute);
        if (isMute)
        {
            AudioManager.Instance.SetAudioGroupMute("Background", true);
            AudioManager.Instance.SetAudioGroupMute("Meditation/Background", true);

            context.SetValue("MusicSliderValue", -40f);
        }
        else
        {
            context.SetValue("MusicSliderValue", -36f);
            AudioManager.Instance.SetAudioGroupMute("Background", false, -36f);
            AudioManager.Instance.SetAudioGroupMute("Meditation/Background", false, -36f);
        }
    }

    private void OnMusicSliderValueChanged(float value)
    {
        Debug.Log("OnMusicSliderValueChanged" + value);
        if (value == -40)
        {
            context.SetValue("MusicToggle", true);
        }
        else
        {
            if (context.MusicToggle)
            {
                context.SetValue("MusicToggle", false);
            }
            else
            {
                AudioManager.Instance.SetAudioGroupValue("Background", value);
                AudioManager.Instance.SetAudioGroupValue("Meditation/Background", value);
            }
        }
    }

    private void OnSoundToggleChanged(bool isMute)
    {
        Debug.Log("OnSoundToggleChanged" + isMute);
        if (isMute)
        {
            AudioManager.Instance.SetAudioGroupMute("Effect", true);
            AudioManager.Instance.SetAudioGroupMute("Meditation/Effect", true);

            context.SetValue("SoundSliderValue", -40f);
        }
        else
        {
            context.SetValue("SoundSliderValue", -36f);
            AudioManager.Instance.SetAudioGroupMute("Effect", false, -36f);
            AudioManager.Instance.SetAudioGroupMute("Meditation/Effect", false, -36f);
        }
    }

    private void OnSoundSliderValueChanged(float value)
    {
        Debug.Log("OnSoundSliderValueChanged" + value);
        if (value == -40)
        {
            context.SetValue("SoundToggle", true);
        }
        else
        {
            if (context.SoundToggle)
            {
                context.SetValue("SoundToggle", false);
            }
            else
            {
                AudioManager.Instance.SetAudioGroupValue("Effect", value);
                AudioManager.Instance.SetAudioGroupValue("Meditation/Effect", value);
            }
        }
    }

    private void OnAmbienceToggleChanged(bool isMute)
    {
        Debug.Log("OnAmbienceToggleChanged" + isMute);
        if (isMute)
        {
            AudioManager.Instance.SetAudioGroupMute("Ambience", true);

            context.SetValue("AmbienceSliderValue", -40f);
        }
        else
        {
            context.SetValue("AmbienceSliderValue", -36f);
            AudioManager.Instance.SetAudioGroupMute("Ambience", false, -36f);
        }
    }

    private void OnAmbienceSliderValueChanged(float value)
    {
        Debug.Log("OnAmbienceSliderValueChanged" + value);
        if (value == -40)
        {
            context.SetValue("AmbienceToggle", true);
        }
        else
        {
            if (context.AmbienceToggle)
            {
                context.SetValue("AmbienceToggle", false);
            }
            else
            {
                AudioManager.Instance.SetAudioGroupValue("Ambience", value);
            }
        }
    }

    private void OnVoiceToggleChanged(bool isMute)
    {
        Debug.Log("VoiceVoiceVoice2" + isMute);
        if (isMute)
        {
            AudioManager.Instance.SetAudioGroupMute("Voice", true);

            context.SetValue("VoiceSliderValue", -40f);
        }
        else
        {
            context.SetValue("VoiceSliderValue", -36f);
            AudioManager.Instance.SetAudioGroupMute("Voice", false, -36f);
        }
    }

    private void OnVoiceSliderValueChanged(float value)
    {
        Debug.Log("VoiceVoiceVoiceVoice" + value);
        if (value == -40)
        {
            context.SetValue("VoiceToggle", true);
        }
        else
        {
            if (context.VoiceToggle)
            {
                context.SetValue("VoiceToggle", false);
            }
            else
            {
                AudioManager.Instance.SetAudioGroupValue("Voice", value);
            }
        }
    }

    //private void OnVoiceToggleChanged(bool isMute)
    //{
    //    if (isMute)
    //    {
    //        VoiceManager.Instance.ControlTotalVolumeSpeakr(0);

    //        context.SetValue("VoiceSliderValue", 0f);
    //    }
    //    else
    //    {
    //        VoiceManager.Instance.ControlTotalVolumeSpeakr(0.1f);
    //        context.SetValue("VoiceSliderValue", 0.1f);
    //    }
    //}

    //private void OnVoiceSliderValueChanged(float value)
    //{
    //    if (value <= 0)
    //    {
    //        context.SetValue("VoiceToggle", true);
    //    }
    //    else
    //    {
    //        if (context.VoiceToggle)
    //        {
    //            context.SetValue("VoiceToggle", false);
    //        }
    //        else
    //        {
    //            VoiceManager.Instance.ControlTotalVolumeSpeakr(value);
    //        }
    //    }
    //}

    private void OnClickClose()
    {
        context.onClickClose -= OnClickClose;
        UIManager.Pop("", false, false, true, null, () => { context.onClickClose += OnClickClose; });
    }

    public override void OnStartShow()
    {
        base.OnStartShow();
    }

    public override void OnFinishHide()
    {
        base.OnFinishHide();
    }

    public void Set()
    {
        
        context.SetValue("CoinText", accountManager.PlayerData.coin == 0 ? "0" : string.Format(Format.Money, accountManager.PlayerData.coin));
        context.SetValue("HeartText", accountManager.PlayerData.heart == 0 ? "0" : string.Format(Format.Money, accountManager.PlayerData.heart));
    }

    private void OnClickLogout()
    {
        if (accountManager == null) return;
        StartCoroutine(Logout());
    }

    private IEnumerator Logout()
    {
        context.onClickLogout -= OnClickLogout;

        yield return null;

        bool isLeave = false;
        //방 나가기
       roomDataBaseManager.RoomAPIHandler.LeaveRoom(roomDataBaseManager.Current,
            () =>
            { 
                isLeave = true;
                roomDataBaseManager.Current = null;
            }
        );

        yield return new WaitUntil(() => isLeave);

        DisconnectAll();

        //현재 활성화된 씬
        Scene current = SceneManager.GetActiveScene();

        //언로드
        AsyncOperation async = SceneManager.UnloadSceneAsync(current);

        async.allowSceneActivation = true;
        while (!async.isDone)
        {
            yield return null;
        }
        async = null;

        //여기서 로그아웃 상태 전환
        GameManager.isLoggingOut = true;
       
        //비우기
        var itemDatas = Get<ItemListView>().GetItemDatas();
        itemDatas = null;

        PushToTalk pushToTalk = Get<MainView>().playerVoice.pushToTalk;
        pushToTalk.DisableRecorder();

        //씬 전환
        async = SceneManager.LoadSceneAsync("Title", LoadSceneMode.Additive);
        async.allowSceneActivation = true;
        Scene current2 = SceneManager.GetActiveScene();
        Destroy(GameManager.Instance.gameObject);

        while (!async.isDone)
        {
            yield return null;
        }
        context.onClickLogout += OnClickLogout;
        UIManager.PopToRoot(null, 0);
    }

    public void DisconnectAll()
    {
        //웹소켓 끊기
        accountManager.DisconnectSocket();

        //포톤 끊기
        persistent.PhotonChat.Disconnect();
        persistent.VoiceManager.ppvs.Disconnect();
        Photon.Pun.PhotonNetwork.Disconnect();
        Destroy(Photon.Voice.PUN.PhotonVoiceNetwork.Instance.gameObject);
    }
}

