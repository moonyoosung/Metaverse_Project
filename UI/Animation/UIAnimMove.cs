using UnityEngine;
using System;
using DG.Tweening;

[AddComponentMenu("MindPlus/UI/Animation/[UI] Move Animation")]
public class UIAnimMove : UIAnimation
{
    [Serializable]
    public class AnimParams
    {
        public RectTransform rectTransform;
        public float inserTime;
        public float duration;
        public Vector2 startPos;
        public Vector2 endPos;
        public Ease basicEase = Ease.Unset;
        public AnimationCurve ease = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
    }

    [Header("[ShowSetting]", order = 0)]
    public AnimParams show;
    [Header("[HideSetting]", order = 1)]
    public AnimParams hide;
    private Sequence DoMove(AnimParams animParams)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.OnStart(() => { animParams.rectTransform.anchoredPosition = animParams.startPos; });
        Tween tween = animParams.rectTransform.DOAnchorPos(animParams.endPos, animParams.duration);

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
    private Sequence DoMove(AnimParams animParams, float insertTime, float duration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.OnStart(() => { animParams.rectTransform.anchoredPosition = animParams.startPos; });
        Tween tween = animParams.rectTransform.DOAnchorPos(animParams.endPos, duration);

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
        return DoMove(hide, insertTime, duration);
    }

    public override Sequence HideSequences()
    {
        return DoMove(hide);
    }

    public override Sequence ShowSequences(float insertTime, float duration)
    {
        return DoMove(show, insertTime, duration);
    }

    public override Sequence ShowSequences()
    {
        return DoMove(show);
    }
}
