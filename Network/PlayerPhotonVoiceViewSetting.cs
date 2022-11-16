using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using UnityEngine.Events;
using Photon.Voice.Unity.UtilityScripts;
using System;
using Photon.Voice;

public class PlayerPhotonVoiceViewSetting : MonoBehaviourPun
{
    [Header("Voice Detector Calibrate")]
    public bool inProgressCalibration = false;
    public UnityEvent onStartCallibrate;
    public UnityEvent onEndCallibrate;

    public Action onAction;
    public PhotonVoiceNetwork photonVoiceNetwork;
    private bool isCoroutineing;
    private WaitForSeconds wait  = new WaitForSeconds(0.05f);
    private VoiceManager voiceManager;

    public void Initalize(MindPlus.GameManager gameManager)
    {
        if (photonView == null || !photonView.IsMine)
            return;

        this.voiceManager = gameManager.Persistent.VoiceManager;

        if (voiceManager.recorder)
        {
            Recorder recorder = voiceManager.recorder;
            StartCoroutine(RecorderSetting(recorder));
            StartCoroutine(SetRecorderInVoiceNetwork(recorder));

            PhotonVoiceView pvv = GetComponent<PhotonVoiceView>();
            pvv.RecorderInUse = recorder;
        }
        isCoroutineing = false;
    }

    private IEnumerator RecorderSetting(Recorder recorder)
    {
        yield return new WaitUntil(() => voiceManager.pvv.RecorderInUse);
        recorder.VoiceDetectionThreshold = 0.04f;
        recorder.TransmitEnabled = true;
        recorder.VoiceDetection = true;
        recorder.ReactOnSystemChanges = true;
        recorder.SkipDeviceChangeChecks = true;
        recorder.gameObject.SetActive(true);
        recorder.IsRecording = false;
        yield return new WaitForSeconds(0.1f);
        recorder.IsRecording = true;
    }

    public IEnumerator Connect(Action onConnect = null)
    {
        if (photonView != null && photonView.IsMine && !isCoroutineing)
        {
            isCoroutineing = true;

            GameObject rObject = Instantiate<GameObject>(Resources.Load<GameObject>("Voice"), voiceManager.gameObject.transform);
            Resources.UnloadUnusedAssets();
            voiceManager.recorder = rObject.GetComponent<Recorder>();
            yield return null;
            yield return StartCoroutine(DoFindMicrophoneDevice(voiceManager.recorder, rObject)); 
            yield return StartCoroutine(SetRecorderInVoiceNetwork(voiceManager.recorder));
            onConnect?.Invoke();
        }
        isCoroutineing = false;
    }

    IEnumerator DoFindMicrophoneDevice(Recorder recorder, GameObject rObject)
    {
        yield return wait;

        Photon.Voice.DeviceInfo deviceInfo = Photon.Voice.DeviceInfo.Default;
        Debug.Log("Voice IEnumerator DoFindMicrophoneDevice(Recorder recorder, GameObject rObject) " + deviceInfo.ToString()); 
        try
        {
            deviceInfo = recorder.MicrophoneDevice;
        }
        catch (System.Exception)
        {

        }

        if (deviceInfo == Photon.Voice.DeviceInfo.Default)// || recorder.MicrophoneDevice.IDInt <= 0)
        {
            //?????? ?????? ???? ????
        }
        else
        {
            Debug.Log("Voice   else");

            PhotonVoiceView pvv = GetComponent<PhotonVoiceView>();
            pvv.SpeakerInUse = GetComponent<Speaker>();
            yield return new WaitUntil(() => voiceManager.pvv.SpeakerInUse);
            pvv.RecorderInUse = recorder;
            yield return StartCoroutine(RecorderSetting(recorder));
            voiceManager.pvv = pvv;
            yield return new WaitUntil(() => voiceManager.pvv.RecorderInUse.gameObject);
        }
    }

    IEnumerator SetRecorderInVoiceNetwork(Recorder recorder)
    {
        if (recorder)
        { 
            PhotonVoiceNetwork photonVoiceNetwork = null;
            while (photonVoiceNetwork == null)
            {
                yield return null;
                photonVoiceNetwork = GameObject.FindObjectOfType<PhotonVoiceNetwork>();
            }
            photonVoiceNetwork.PrimaryRecorder = recorder;
        }
    }

    public void VoiceDetectorCalibrate()
    {
        onStartCallibrate.Invoke();
        inProgressCalibration = true;
        StartCoroutine(DoCalibrate());
        voiceManager.pvv.RecorderInUse.VoiceDetectorCalibrate(2000, VoiceDetectorCalibrateCallback);
    }

    IEnumerator DoCalibrate()
    {
        while (inProgressCalibration)
        {
            yield return null;
        }
        onEndCallibrate.Invoke();
    }

    void VoiceDetectorCalibrateCallback(float f)
    {
        inProgressCalibration = false;
    }

    public void Disconnect()
    {
        if (photonVoiceNetwork == null) return;
        photonVoiceNetwork.Disconnect();
    }
}
