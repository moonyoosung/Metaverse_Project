using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIStreamPop : UIAnimManager.Stream
{
    private UINavigation navigation;
    private bool isHideImmediately;
    private bool isShowImmediately;
    private bool isCurrentShow;
    private Action onPop;
    private Action onComplete;
    public UIStreamPop(UINavigation navigation, bool isShowImmediately = false, bool isHideImmediately = false, bool isCurrentShow = true,
        Action onPop = null, Action onComplete = null) : base()
    {
        this.navigation = navigation;
        this.isShowImmediately = isShowImmediately;
        this.isHideImmediately = isHideImmediately;
        this.isCurrentShow = isCurrentShow;
        this.onPop = onPop;
        this.onComplete = onComplete;
    }
    public override string ID => "Pop" + navigation.Current.name;

    public override IEnumerator Handle(UIAnimManager manager, MonoBehaviour caller)
    {
        UIView prev = navigation.Current;
        UIView next = navigation.Pop();
        onPop?.Invoke();

        UIStreamHide hide = new UIStreamHide(isHideImmediately, null, prev);

        yield return caller.StartCoroutine(hide.Handle(manager, caller));

        if (next != null && isCurrentShow)
        {
            UIStreamShow show = new UIStreamShow(isShowImmediately,null, next);

            yield return caller.StartCoroutine(show.Handle(manager, caller));
        }
        onComplete?.Invoke();
    }
}
