
using DG.Tweening;
using System;
using UnityEngine;

[AddComponentMenu("MindPlus/UI/Animation/[UI] Scale Animation")]
public class UIAnimScale : UIAnimation
{
    [Serializable]
    public class AnimParams
    {
        public float inserTime;
        public float duration;
        public Vector3 startScale;
        public Vector3 endScale;
        public Ease basicEase = Ease.Unset;
        public AnimationCurve ease = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
    }
    [Header("[ShowSetting]", order = 0)]
    public AnimParams show;
    [Header("[HideSetting]", order = 1)]
    public AnimParams hide;

    private Sequence DoScale(AnimParams animParams)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.OnStart(() => { transform.localScale = animParams.startScale; });
        Tween tween = transform.DOScale(animParams.endScale, animParams.duration);
        if (animParams.basicEase != Ease.Unset)
        {
            tween.SetEase(animParams.basicEase);
        }
        else
        {
            tween.SetEase(animParams.ease);
        }
        sequence.Insert(animParams.inserTime, tween);

        return sequence;
    }
    private Sequence DoScale(AnimParams animParams, float insertTime, float duration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.OnStart(() => { transform.localScale = animParams.startScale; });
        Tween tween = transform.DOScale(animParams.endScale, duration);
        if (animParams.basicEase != Ease.Unset)
        {
            tween.SetEase(animParams.basicEase);
        }
        else
        {
            tween.SetEase(animParams.ease);
        }
        sequence.Insert(insertTime, tween);

        return sequence;
    }
    public override Sequence ShowSequences()
    {
        return DoScale(show);
    }
    public override Sequence HideSequences()
    {
        return DoScale(hide);
    }

    public override Sequence ShowSequences(float insertTime, float duration)
    {
        return DoScale(show, insertTime, duration);
    }

    public override Sequence HideSequences(float insertTime, float duration)
    {
        return DoScale(hide, insertTime, duration);
    }
}
