using DG.Tweening;
using MindPlus.Contexts.Master;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

[System.Serializable]
public class Alarm
{
    public string userId;
    public NotifyData[] pushAlarms;
}

[System.Serializable]
public class NotifyData
{
    public float showingTime;
    public string type;
    public string activityName;
    public string userName;
    public string challengeId;
    public string challangeName;
    public string heart;
    public string coin;
    public string weight;
    public string level;
    public string userClass;
    public string message;
    public Sprite icon;
    public Action button;
    public NotifyData(float showingTime, string type, string message, Sprite icon = null, string heart = "", string coin = "", Action button = null)
    {
        this.showingTime = showingTime;
        this.message = message;
        this.type = type;
        this.icon = icon;
        this.heart = heart;
        this.coin = coin;
        this.button = button;
    }
    public string GetTitle()
    {
        switch (type)
        {
            case "Reward":
                return activityName;
            case "LevelUp":
                return challangeName;
            case "ClassUp":
                return type;
            default:
                return type;
        }
    }
    public string GetMessage()
    {
        switch (type)
        {
            case "Reward":
                return string.Format("You completed [{0}] activity today!", activityName);
            case "LevelUp":
                return string.Format("You are now Level {0}!", level);
            case "ClassUp":
                return string.Format("You are now Class {0}!", LevelCardView.GetTier(int.Parse(userClass)));
            default:
                return string.Format("You are invited to [{0}]", message);
                //return message;
        }
    }
}
public class NotifyView : UIView
{
    [TitleGroup("[Option]")]
    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    public Gradient2 gradient;
    [TitleGroup("[Option]")]
    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    public GameObject inputArea;
    private NotifyContext context;
    private Queue<NotifyData> queues = new Queue<NotifyData>();
    private float showingTime = 0f;
    private float currentTime = 0f;
    private ImageContainer challIcons;
    private UnityEngine.Gradient[] gradients;
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.context = new NotifyContext();
        ContextHolder.Context = context;
        (UIManager as UIManager).MasterContext.Notify = context;
        this.challIcons = persistent.ResourceManager.ChallengeImages;
        this.gradients = persistent.ResourceManager.GetUIResources().levelGradients;
        context.SetValue("HeartIcon", persistent.ResourceManager.ImageContainer.Get("heart"));
        context.SetValue("CoinIcon", persistent.ResourceManager.ImageContainer.Get("coin"));
        gradient.enabled = false;
        StartCoroutine(CheckQueue());
    }
    public void PushNotify(NotifyData data)
    {
        queues.Enqueue(data);
    }
    private IEnumerator CheckQueue()
    {
        while (true)
        {
            if (queues.Count > 0 && state == VisibleState.Disappeared)
            {
                SetContext(queues.Dequeue());
                UIManager.Show(gameObject.name, false, this);
            }
            yield return null;
        }
    }

    private IEnumerator Timer()
    {
        while (currentTime < showingTime)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) && state == VisibleState.Appeared)
            {
                if (!EventSystem.current.currentSelectedGameObject == inputArea)
                {
                    currentTime = showingTime;
                    state = VisibleState.Wait;
                }
            }
#else
            if (Input.touchCount > 0 && state == VisibleState.Appeared)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    if (!EventSystem.current.currentSelectedGameObject == inputArea)
                    {
                        currentTime = showingTime;
                        state = VisibleState.Wait;
                    }
                }
            }
#endif
            currentTime += Time.deltaTime;
            yield return null;
        }

        UIManager.Hide(gameObject.name, false, null, this);
    }
    private void SetContext(NotifyData data)
    {
        context.SetValue("Icon", GetIcon(data));
        this.showingTime = data.showingTime;
        context.SetValue("TitleText", data.GetTitle());
        context.SetValue("ToastMessage", data.GetMessage());
        context.SetValue("IsActiveCoinIcon", data.heart == null || data.coin == null ? false : true);
        context.SetValue("IsActiveHeartIcon", data.heart == null || data.coin == null ? false: true);
        context.SetValue("IsActiveHeartText", (data.heart == null || data.heart == "") ? false : true);
        context.SetValue("IsActiveCoinText", (data.coin == null || data.coin == "") && (data.weight == null || data.weight == "") ? false : true);
        context.SetValue("IsActiveButton", data.button == null ? false : true);
        context.SetValue("HeartText", data.heart);
        context.SetValue("CoinText", data.coin == null ? data.weight : data.coin);
        if (data.button != null)
        {
            context.onClickButton = data.button;
        }
    }
    private Sprite GetIcon(NotifyData data)
    {
        switch (data.type)
        {
            case "Reward":
                return challIcons.Get(data.challengeId);
            case "LevelUp":
                return challIcons.Get(data.challengeId);
            case "ClassUp":
                gradient.enabled = true;
                gradient.EffectGradient = gradients[int.Parse(data.userClass)];
                return null;
            default:
                return data.icon;
        }
    }
    public override void OnStartShow()
    {
        base.OnStartShow();
        StartCoroutine(Timer());
    }
    public override void OnFinishShow()
    {
        AudioManager.Instance.EffectSoundPlay(AudioManager.Effect.NOTICE);
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();
        showingTime = 0f;
        currentTime = 0f;
    }
}
