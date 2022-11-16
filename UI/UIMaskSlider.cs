using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class UIMaskSlider : MonoBehaviour
{
    public RectTransform mask;
    public RectTransform picker;

    public float[] points;
    public AnimationCurve ease = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
    public float duration = 0.5f;
    private Sequence sequence;
    public Action<int, int> onStart;
    public int current = int.MaxValue;
    public void OnMove(int idx)
    {
        if (sequence.IsActive())
        {
            sequence.Complete(true);
        }

        sequence = DOTween.Sequence();
        sequence.Insert(0, mask.DOLocalMoveX(points[idx], duration, true));
        sequence.Insert(0, picker.DOLocalMoveX(-points[idx], duration, true));
        sequence.SetEase(ease);
        onStart?.Invoke(current, idx);
        sequence.OnComplete(() => { current = idx; });
    }
}
