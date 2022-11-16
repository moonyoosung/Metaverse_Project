using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMultiSplitView : UIView
{
    protected UINavigation[] navigations;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        InstantiateNavigations();
        base.Initialize(persistent, uIManager);
    }
    public override void OnStartShow()
    {
        OnStartSplitShow();
        base.OnStartShow();
    }
    public virtual void OnStartSplitShow()
    {
        foreach (var navigation in navigations)
        {
            if (navigation.history.Count > 0)
            {
                UIManager.Interrupt(new UIStreamShow(false, null, navigation.Current), navigation.ID);
            }
        }
    }
    public override void OnStartHide()
    {
        foreach (var navigation in navigations)
        {
            if (navigation.history.Count > 0)
            {
                UIView[] targets = navigation.GetShowing();
                if(targets == null || targets.Length <= 0)
                {
                    continue;
                }
                UIManager.Interrupt(new UIStreamHide(false, null, targets), navigation.ID);
            }
        }
        base.OnStartHide();
    }

    public override void Update()
    {
        base.Update();
    }
    private void InstantiateNavigations()
    {
        navigations = new UINavigation[]
        {
            new UINavigation("Left" + gameObject.name) ,
            new UINavigation("Right" + gameObject.name)
        };
    }
    public void PushNavigation(UIView leftView = null, UIView rightView = null)
    {
        if (leftView != null)
        {
            if (navigations[0].Current != leftView)
            {
                navigations[0].Push(leftView);
            }
        }
        if (rightView != null)
        {
            if (navigations[1].Current != rightView)
            {
                navigations[1].Push(rightView);
            }
        }
    }

    public void Push(string streamID = "", bool isShowImmediately = false, bool isHideImmediately = false, Action onComplete = null, UIView[] leftViews = null, UIView[] rightViews = null)
    {
        UIManager.PushSplit(navigations, streamID, isShowImmediately, isHideImmediately, onComplete, leftViews, rightViews);
    }
    public void Pop(bool isLeft = true, bool isRight = true, bool isShowImmediately = false, bool isHideImmediately = false, bool isCurrentShow = true, Action onPop = null, Action onComplete = null)
    {
        UIManager.PopSplit(navigations, isShowImmediately, isHideImmediately, isCurrentShow, isLeft, isRight, onPop, onComplete);
    }
}
