using DG.Tweening;
using MindPlus;
using MindPlus.Contexts.MeditationView;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class BreathView : UIFlatView
{
    #region UIAnim
    [Header("[diminish]", order = 0)]
    public UIAnimScale.AnimParams diminish;
    [Header("[diffusion]", order = 1)]
    public UIAnimScale.AnimParams diffusion;
    public GameObject uIAnimScaleImage;
    public CanvasGroup fadeInGroup;

    public void DoScale(UIAnimScale.AnimParams animParams, float insertTime, float duration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.OnStart(() => { uIAnimScaleImage.transform.localScale = animParams.startScale; });
        Tween tween = uIAnimScaleImage.transform.DOScale(animParams.endScale, duration);
        if (animParams.basicEase != Ease.Unset)
        {
            tween.SetEase(animParams.basicEase);
        }
        else
        {
            tween.SetEase(animParams.ease);
        }
        sequence.Insert(insertTime, tween);
    }

    public void DoFadeIn()
    {
        fadeInGroup.DOFade(1, 2.25f);
    }
    #endregion

    public enum State { Idle, Breathing2, Breathing3 }
    public PlayerRig rig;
    private Avatar avatar;
    private Persistent persistent;
    private BreathViewContext context;
    private bool isStart = false;
    public float maxSeconds;
    private float seconds;
    public AudioSource effectAudio;
    //private int cycle = 0;
    private int count = 0;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);

        this.persistent = persistent;
        this.context = new BreathViewContext();
        this.ContextHolder.Context = context;
        this.isStart = false;
        this.maxSeconds = 180f;
        this.seconds = 0;
        this.fadeInGroup.alpha = 0f;
        this.count = 0;
        context.SetValue("StateInfoText", "Pay attention to your breathing.");
        MeditationUIManager UIManager = (uIManager as MeditationUIManager);
        this.context.onClickClose += () =>
        {
           
            UIManager.Pop("",false, false, true, () => {
                AudioManager.Instance.SetActiveSceneSoundMute(false);
                this.persistent.UIManager.Push(null, false, true, null, UIView.Get("MainView"));
            });
        };
    }

    public override void OnStartShow()
    {
        base.OnStartShow();
        StartCoroutine(DoMeditation());
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();
        this.persistent.NetworkManager.currentRoomManager.player.GetPart<PlayerRig>().GetComponent<AnimationController>().SetIdle();
         UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Meditation");
    }

    private IEnumerator DoMeditation()
    {
        yield return null;

        Redeay();
        yield return new WaitForSeconds(13f);
       
        SetAnimation(State.Breathing2);
        yield return new WaitUntil(() => rig.animationController.animator.GetCurrentAnimatorStateInfo(0).IsName("Breathing2"));
        count = 1;
        isStart = true;
        Diffusion(rig.animationController.animator.GetCurrentAnimatorStateInfo(0).length);
        DoFadeIn();

        yield return new WaitUntil(() => rig.animationController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

        while (isStart)
        {
            yield return new WaitUntil(() => rig.animationController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

            ++count;
            int cycle = (count / 2);
            if (rig.animationController.animator.GetCurrentAnimatorStateInfo(0).IsName("Breathing2"))
            {
                SetAnimation(State.Breathing3);
                Diminish(rig.animationController.animator.GetCurrentAnimatorStateInfo(0).length, cycle);
            }
            if (rig.animationController.animator.GetCurrentAnimatorStateInfo(0).IsName("Breathing3"))
            {
                Diffusion(rig.animationController.animator.GetCurrentAnimatorStateInfo(0).length, cycle);
                SetAnimation(State.Breathing2);
            }
            yield return new WaitUntil(() => rig.animationController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        }
    }

    private IEnumerator End()
    {
        SetAnimation(State.Idle);

        yield return new WaitUntil(() => rig.animationController.animator.GetCurrentAnimatorStateInfo(0).IsName("Breathing4"));
        yield return new WaitUntil(() => rig.animationController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        context.onClickClose.Invoke();
        persistent.ChallengeManager.PostAchievementActivity(ActivityID.BreathingExercise, DateTime.Now);
    }

    public override void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            isStart = false;
            StartCoroutine(End());
        }
#endif
        if (!isStart) return;

        if (seconds >= maxSeconds)
        {
            isStart = false;
            StartCoroutine(End());
        }

        seconds += Time.deltaTime;
        StopWatch(seconds);
    }
    public void Diminish(float sec, int cycle = 0)
    {
        if (cycle != 2 && cycle != 3)
        {
            effectAudio.PlayOneShot(AudioManager.Instance.GetEffectClip(AudioManager.Effect.BREATHE_VOICE_OUT));
        }
        effectAudio.PlayOneShot(AudioManager.Instance.GetEffectClip(AudioManager.Effect.BREATH_OUT));
        context.SetValue("StateInfoText", "Breathe Out");
        DoScale(diminish, 0, sec);
    }

    public void Diffusion(float sec, int cycle = 0)
    {
        if(cycle == 1)
        {
            effectAudio.PlayOneShot(AudioManager.Instance.GetEffectClip(AudioManager.Effect.BREATHE_VOICE_2));
        }
        else if(cycle < 1 || cycle > 2)
        {
            effectAudio.PlayOneShot(AudioManager.Instance.GetEffectClip(AudioManager.Effect.BREATHE_VOICE_IN));
        }  
        
        effectAudio.PlayOneShot(AudioManager.Instance.GetEffectClip(AudioManager.Effect.BREATH_IN));
        context.SetValue("StateInfoText", "Breathe In");
        DoScale(diffusion, 0, sec);
    }

    public void StopWatch(float stopWatch)
    {
        TimeSpan timespan = TimeSpan.FromSeconds(stopWatch);
        context.SetValue("TimeText", string.Format("{0:00}:{1:00}", timespan.Minutes, timespan.Seconds));
    }

    public void SetAnimation(State state)
    {
        rig.animationController.animator.SetInteger("State", (int)state);
    }

    private void SetAvatar()
    {
        CustomAvatar customAvatar = persistent.AccountManager.PlayerData.GetCustomAvatar();

        if (avatar == null)
        {
            avatar = new Avatar
            (
                persistent.ResourceManager.GetAvatarProbs(),
                rig.transform,
                customAvatar,
                rig.customization.Rebind,
                rig.customization.OnSetBone
            );
        }
        else
        {
            avatar.ResetParts(customAvatar);
        }

        foreach (var skinnedMeshRenderer in avatar.avataSet.skinnedMeshRenderers)
        {
            if (skinnedMeshRenderer == null || skinnedMeshRenderer.gameObject == null) continue;

            skinnedMeshRenderer.gameObject.layer = LayerMask.NameToLayer("Avatar");
        }
    }

    private void Redeay()
    {
        SetAvatar();
        SetAnimation(State.Idle);
        context.SetValue("StateInfoText", "Pay attention to your breathing.");
        effectAudio.PlayOneShot(AudioManager.Instance.GetEffectClip(AudioManager.Effect.BREATHE_VOICE_1));
    }
}

