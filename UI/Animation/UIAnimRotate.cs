using UnityEngine;
using System;
using DG.Tweening;

[AddComponentMenu("MindPlus/UI/Animation/[UI] Rotate Animation")]
public class UIAnimRotate : UIAnimation
{
    [Serializable]
    public class AnimParams
    {
        public float inserTime;
        public float duration;
        public Vector3 startRotate;
        public Vector3 endRotate;
        public Ease basicEase = Ease.Unset;
        public AnimationCurve ease = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
    }
    [Header("[ShowSetting]", order = 0)]
    public AnimParams show;
    [Header("[HideSetting]", order = 1)]
    public AnimParams hide;
    private Sequence DoRotate(AnimParams animParams)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.OnStart(() => { transform.localEulerAngles = animParams.startRotate; });
        Tween tween = transform.DOLocalRotate(animParams.endRotate, animParams.duration, RotateMode.FastBeyond360);
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
    private Sequence DoRotate(AnimParams animParams, float insertTime, float duration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.OnStart(() => { transform.localEulerAngles = animParams.startRotate; });
        Tween tween = transform.DOLocalRotate(animParams.endRotate, duration, RotateMode.FastBeyond360);
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
    public override Sequence HideSequences(float insertTime, float duration)
    {
        return DoRotate(hide, insertTime, duration);
    }

    public override Sequence HideSequences()
    {
        return DoRotate(hide);
    }

    public override Sequence ShowSequences(float insertTime, float duration)
    {
        return DoRotate(show, insertTime, duration);
    }

    public override Sequence ShowSequences()
    {
        return DoRotate(show);
    }
}
