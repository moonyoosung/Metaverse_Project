using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class UIStreamHide : UIAnimManager.Stream
{
    private UIView[] targets;
    private Action onComplete;
    private bool isImmediately;

    public UIStreamHide(bool isImmediately, Action onComplete, params UIView[] targets) : base()
    {
        this.targets = targets;
        this.isImmediately = isImmediately;
        this.onComplete = onComplete;
    }
    public override string ID => "Hide" + targets[0].name;

    public override IEnumerator Handle(UIAnimManager manager, MonoBehaviour caller)
    {
        totalSequence = DOTween.Sequence();

        foreach (var target in targets)
        {
            target.OnStartHide();

            foreach (var showSequence in isImmediately == true ? target.Hide(0f, 0f) : target.Hide())
            {
                totalSequence.Insert(0f, showSequence);
            }
        }

        yield return totalSequence.WaitForCompletion();

        foreach (var target in targets)
        {
            target.OnFinishHide();
        }

        onComplete?.Invoke();
    }
}

