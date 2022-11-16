using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

[AddComponentMenu("MindPlus/UI/Animation/[UI] Fade Animation")]
public class UIAnimFade : UIAnimation
{
    [Serializable]
    public class AnimParams
    {
        public float insertTime;
        public float duration;
        [Range(0, 1f)]
        public float startRatio;
        [Range(0, 1f)]
        public float endRatio;
        public Ease basicEase = Ease.Unset;
        public AnimationCurve ease = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
    }
    public class CustomGraphic
    {
        public float alpha;
        public Graphic graphic;
        public CustomGraphic(Graphic graphic)
        {
            this.graphic = graphic;
            this.alpha = graphic.color.a;
        }
    }
    public List<CustomGraphic> Targets
    {
        get
        {
            List<CustomGraphic> removeList = new List<CustomGraphic>();
            foreach (var graphic in GetComponentsInChildren<Graphic>(true))
            {
                bool isEqul = false;

                foreach (var target in targets)
                {
                    if (target.graphic == null)
                    {
                        removeList.Add(target);
                        continue;
                    }
                    if (target.graphic == graphic)
                    {
                        isEqul = true;
                        break;
                    }
                }

                if (isEqul) continue;

                targets.Add(new CustomGraphic(graphic));
            }

            foreach (var remove in removeList)
            {
                targets.Remove(remove);
            }

            return targets;
        }
    }
    private List<CustomGraphic> targets = new List<CustomGraphic>();
    [Header("[ShowSetting]", order = 0)]
    public AnimParams show;
    [Header("[HideSetting]", order = 1)]
    public AnimParams hide;

    public override Sequence HideSequences(float insertTime, float duration)
    {
        return DoFade(hide, insertTime, duration);
    }

    public override Sequence HideSequences()
    {
        return DoFade(hide);
    }

    public override Sequence ShowSequences(float insertTime, float duration)
    {
        return DoFade(show, insertTime, duration);
    }
    public override Sequence ShowSequences()
    {
        return DoFade(show);
    }
    private Sequence DoFade(AnimParams animParams)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.OnStart(() =>
        {
            foreach (var target in Targets)
            {
                target.graphic.color = new Color(target.graphic.color.r, target.graphic.color.g, target.graphic.color.b, target.alpha * animParams.startRatio);
            }
        });

        foreach (var target in Targets)
        {
            Tween tween = target.graphic.DOFade(target.alpha * animParams.endRatio, animParams.duration);
            if (animParams.basicEase != Ease.Unset)
            {
                tween.SetEase(animParams.basicEase);
            }
            else
            {
                tween.SetEase(animParams.ease);
            }

            sequence.Insert(animParams.insertTime, tween);
        }
       

        return sequence;
    }
    private Sequence DoFade(AnimParams animParams, float insertTime, float duration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.OnStart(() =>
        {
            foreach (var target in Targets)
            {
                target.graphic.color = new Color(target.graphic.color.r, target.graphic.color.g, target.graphic.color.b, target.alpha * animParams.startRatio);
            }
        });
        foreach (var target in Targets)
        {
            Tween tween = target.graphic.DOFade(target.alpha * animParams.endRatio, duration);
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

        return sequence;
    }
}
