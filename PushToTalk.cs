using MindPlus.Contexts.Master;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PushToTalk : MonoBehaviour
{
    public Toggle toggle;
    private Recorder playerRecorder;
    private MainViewContext context;
    private VoiceManager voiceManager;

    public void Initialize(MainViewContext context, VoiceManager voiceManager)
    {
        this.context = context;
        this.voiceManager = voiceManager;
        gameObject.SetActive(true);
        toggle.onValueChanged.AddListener(OnToggleChanged);
        voiceManager.onDeviceChange += delegate { };
    }

    private void OnToggleChanged(bool isOn)
    {
        if (!playerRecorder)
        {
            voiceManager.onAction.Invoke();
            return;
        }
        context.SetValue("IsActiveMute", !isOn);
        context.SetValue("IsActiveMic", isOn);
        playerRecorder.TransmitEnabled = isOn;
    }

    void OnDestroy()
    {
        if (voiceManager != null)
        {
            voiceManager.onDeviceChange -= delegate {  };
        }
    }

    public void DisableRecorder()
    {
        toggle.isOn = false;
        playerRecorder = null;
    }

    //스프라이트 이미지 전환
    public void SetData(PhotonVoiceView pvv)
    {
        playerRecorder = pvv.RecorderInUse;
        playerRecorder.TransmitEnabled = true;
        toggle.isOn = playerRecorder.TransmitEnabled;
        OnToggleChanged(playerRecorder.TransmitEnabled);
    }
}
