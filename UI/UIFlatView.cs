using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFlatView : UIView
{
    protected UINavigation navigation;

    public void PushNavigation(UIView view)
    {
        if(navigation.Current == view)
        {
            return;
        }

        navigation.Push(view);
    }
    public void Push(string streamID = "", bool isShowImmediately = false, bool isHideImmediately = false, Action onComplete = null, params UIView[] views)
    {
        UIManager.Push(navigation, streamID, isShowImmediately, isHideImmediately, onComplete, views);
    }
    public void Pop(bool isShowImmediately = false, bool isHideImmediately = false, bool isCurrentShow = true, Action onPop = null, Action onComplete = null)
    {
        UIManager.Pop(navigation, isShowImmediately, isHideImmediately, isCurrentShow, onPop, onComplete);
    }
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        this.navigation = new UINavigation("Flat" + gameObject.name);
        base.Initialize(persistent, uIManager);
    }
    public override void OnStartShow()
    {
        base.OnStartShow();
        if (navigation.history.Count > 0)
        {
            UIManager.Interrupt(new UIStreamShow(false,null, navigation.Current), this.gameObject.name);
        }
    }
    public override void OnStartHide()
    {
        if (navigation.history.Count > 0)
        {
            UIManager.Interrupt(new UIStreamHide(false, null, navigation.GetShowing()), this.gameObject.name);
        }
        base.OnStartHide();
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();

    }

    public bool AlreadyExist(UIView view)
    {
        if(navigation.history.Count > 0)
        {
            foreach (var item in navigation.history)
            {
                if (item.Equals(view))
                    return true;
            }
        }
        return false;
    }
}
