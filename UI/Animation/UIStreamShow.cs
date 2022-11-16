using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class UIStreamShow : UIAnimManager.Stream
{
    public UIView[] targets;
    public bool isImmediately;
    public Action onComplete;
    public UIStreamShow(bool isImmediately, Action onComplete = null, params UIView[] targets) : base()
    {
        this.targets = targets;
        this.isImmediately = isImmediately;
        this.onComplete = onComplete;
    }
    public override string ID => "Show" + targets[0].name;

    public override IEnumerator Handle(UIAnimManager manager, MonoBehaviour caller)
    {
        totalSequence = DOTween.Sequence();

        foreach (var target in targets)
        {
            if (target == null)
                Debug.LogError("Error =>>>>>>>>>>>>" + ID);
            target?.OnStartShow();

            foreach (var showSequence in isImmediately == true ? target.Show(0f, 0f) : target.Show())
            {
                totalSequence.Insert(0f, showSequence);
            }
        }

        yield return totalSequence.WaitForCompletion();

        foreach (var target in targets)
        {
            target.OnFinishShow();
        }
        onComplete?.Invoke();
    }
}
