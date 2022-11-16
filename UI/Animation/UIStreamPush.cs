using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIStreamPush : UIAnimManager.Stream
{
    private UIView[] targets;
    private UINavigation navigation;
    private Action onComplete;
    private bool isShowImmediately;
    private bool isHideImmediately;
    public override string ID => targets[0].name;
    public UIStreamPush(UINavigation navigation, UIView[] targets, bool isShowImmediately, bool isHideImmediately, Action onComplete = null) : base()
    {
        this.navigation = navigation;
        this.targets = targets;
        this.isShowImmediately = isShowImmediately;
        this.isHideImmediately = isHideImmediately;
        this.onComplete = onComplete;
    }

    public override IEnumerator Handle(UIAnimManager manager, MonoBehaviour caller)
    {
        UIView Current = navigation.Current;

        foreach (var target in targets)
        {
            navigation.Push(target);
        }

        if (Current != null)
        {
            UIStreamHide hide = new UIStreamHide(isHideImmediately, null, Current);
            yield return caller.StartCoroutine(hide.Handle(manager, caller));
        }


        UIStreamShow show = new UIStreamShow(isShowImmediately,null, targets);
        yield return caller.StartCoroutine(show.Handle(manager, caller));

        onComplete?.Invoke();
    }
}
