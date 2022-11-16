
using Slash.Unity.DataBind.Core.Data;
using Slash.Unity.DataBind.Core.Presentation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseUIManager : MonoBehaviour
{
    private UINavigation navigation;
    protected UIAnimManager UIAnimManager;
    public float matchorWidthOrHeight = 0f;
    public ContextHolder ContextHolder
    {
        get
        {
            if (contextHolder == null)
            {
                contextHolder = GetComponent<ContextHolder>();
            }
            return contextHolder;
        }
        private set { contextHolder = value; }
    }

    private ContextHolder contextHolder;
    public virtual IEnumerator Initialize(Persistent Persistent)
    {
        this.navigation = new UINavigation("Base " + gameObject.name);
        this.UIAnimManager = Persistent.UIAnimationManager;
        //CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
        //Default 해상도 비율
        //float fixedAspectRatio = 16f/9f;

        ////현재 해상도의 비율
        //float currentAspectRatio = (float)Screen.width / (float)Screen.height;

        ////현재 해상도 가로 비율이 더 길 경우
        //if (currentAspectRatio > fixedAspectRatio) matchorWidthOrHeight = 1f;
        ////현재 해상도의 세로 비율이 더 길 경우
        //else if (currentAspectRatio < fixedAspectRatio) matchorWidthOrHeight = 0f;
        //else
        //{
        //    matchorWidthOrHeight = 0.5f;
        //}
        //Debug.Log("Screen : height, " + Screen.height + "width, " + Screen.width);
        //if (Screen.height >= Screen.width)
        //{
        //    matchorWidthOrHeight = 1f;
        //}
        //else
        //{
        //    matchorWidthOrHeight = 0f;
        //}
        matchorWidthOrHeight = 0.5f;

        yield return null;
    }

    public void Push(string streamID = "", bool isShowImmediately = false, bool isHideImmediately = false, Action onComplete = null, params UIView[] views)
    {
        UIAnimManager.Push(new UIStreamPush(navigation, views,
            isShowImmediately, isHideImmediately, onComplete), streamID);
    }
    public void Push(UINavigation navi, string streamID = "", bool isShowImmediately = false, bool isHideImmediately = false,
        Action onComplete = null, params UIView[] views)
    {
        UIAnimManager.Push(new UIStreamPush(navi, views,
            isShowImmediately, isHideImmediately, onComplete), streamID);
    }
    public void PushSplit(UINavigation[] navis, string streamID = "", bool isShowImmediately = false, bool isHideImmediately = false,
    Action onComplete = null, UIView[] leftViews = null, UIView[] rightViews = null)
    {
        UIAnimManager.Push(new UIStreamSplitPush(navis, isShowImmediately, isHideImmediately, leftViews, rightViews, onComplete), streamID);
    }
    public void Pop(string streamID = "", bool isShowImmediately = false, bool isHideImmediately = false
        , bool isCurrentShow = true, Action onPop = null, Action onComplete = null)
    {
        if (navigation.history.Count <= 0)
        {
            return;
        }

        UIAnimManager.Push(new UIStreamPop(navigation, isShowImmediately, isHideImmediately, isCurrentShow, onPop, onComplete), streamID);
    }
    public void Pop(UINavigation navi, bool isShowImmediately = false, bool isHideImmediately = false
        , bool isCurrentShow = true, Action onPop = null, Action onComplete = null)
    {
        UIAnimManager.Push(new UIStreamPop(navi, isShowImmediately, isHideImmediately, isCurrentShow, onPop, onComplete));
    }
    public void PopSplit(UINavigation[] navi, bool isShowImmediately = false, bool isHideImmediately = false
        , bool isCurrentShow = true, bool isLeft = true, bool isRight = true, Action onPop = null, Action onComplete = null)
    {
        UIAnimManager.Push(new UIStreamSplitPop(navi, isShowImmediately, isHideImmediately, isCurrentShow, isLeft, isRight, onPop, onComplete));
    }
    public void PopToRoot(UINavigation navi = null, int stackCnt = 0, bool isShowImmediately = false, bool isHideImmediately = false, bool isCurrentShow = false)
    {
        UIAnimManager.Push(new UIStreamPopToRoot(navi == null ? navigation : navi, stackCnt, isShowImmediately, isHideImmediately, isCurrentShow));
    }
    public void Show(string streamID = "", bool isImmediately = false, params UIView[] views)
    {
        UIAnimManager.Push(new UIStreamShow(isImmediately,null, views), streamID);
    }
    public void Hide(string streamID = "", bool isImmediately = false, Action onComplete = null, params UIView[] views)
    {
        UIAnimManager.Push(new UIStreamHide(isImmediately, onComplete, views), streamID);
    }
    public void Interrupt(UIAnimManager.Stream stream, string streamID = "")
    {
        UIAnimManager.Interrupt(stream, streamID);
    }
}
