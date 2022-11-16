using System.Collections.Generic;
using UnityEngine;
using Slash.Unity.DataBind.Core.Presentation;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[RequireComponent(typeof(ContextHolder))]
public class UIView : MonoBehaviour
{
    [TitleGroup("[Option]")]
    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    public bool isPop = false;

    [TitleGroup("[ViewProperty]")]
    [GUIColor(1f, 0f, 0f, 1f)]
    [Tooltip("Pop될 때 분할된 다른 View와 함께 사라지는지 여부")]
    public bool isIndepedent = false;

    [TitleGroup("[ViewProperty]")]
    [GUIColor(1f, 0f, 0f, 1f)]
    [Tooltip("Push될 때 분할된 다른 View와 함께 나타나는지 여부")]
    public bool isWithOtherSide = false;

    [TitleGroup("[Events]")]
    public UnityEvent onStartShow;
    [TitleGroup("[Events]")]
    public UnityEvent onFinishShow;
    [TitleGroup("[Events]")]
    public UnityEvent onStartHide;
    [TitleGroup("[Events]")]
    public UnityEvent onFinishHide;


    public enum VisibleState
    {
        Appearing,
        Appeared,
        Disappearing,
        Disappeared,
        Wait
    }
    [HideInInspector]
    public VisibleState state;
    protected BaseUIManager UIManager { private set; get; }
    private static List<UIView> UIViews = new List<UIView>();
    private ContextHolder contextHolder;
    private UIAnimation[] animations;
    private Canvas canvas;
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
        set { contextHolder = value; }
    }
    public virtual void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        this.UIManager = uIManager;
        SetScreenMatchMode(UIManager.matchorWidthOrHeight);
        //처음 자신한테 설정된 Animation들을 읽어오기
        animations = GetComponentsInChildren<UIAnimation>();
        if (animations == null)
        {
            animations = new UIAnimation[0];
        }

        transform.localPosition = Vector3.zero;
        UIViews.Add(this);
    }
    private void SetScreenMatchMode(float match)
    {
        canvas = GetComponent<Canvas>();
        CanvasScaler[] canvasScalers = GetComponentsInChildren<CanvasScaler>();
        foreach (var canvasScaler in canvasScalers)
        {
            canvasScaler.matchWidthOrHeight = match;
        }
    }
    public bool IsShowing()
    {
        return canvas.enabled;
    }
    public List<Sequence> Show(float insertTime, float duration)
    {
        List<Sequence> sequences = new List<Sequence>();

        foreach (var animation in animations)
        {
            sequences.Add(animation.ShowSequences(insertTime, duration));
        }

        return sequences;
    }
    public List<Sequence> Show()
    {
        List<Sequence> sequences = new List<Sequence>();

        foreach (var animation in animations)
        {
            sequences.Add(animation.ShowSequences());
        }

        return sequences;
    }
    public virtual void OnStartShow()
    {
        canvas.enabled = true;
        state = VisibleState.Appearing;
        onStartShow?.Invoke();
    }
    public virtual void OnFinishShow()
    {
        state = VisibleState.Appeared;
        onFinishShow?.Invoke();
    }
    public List<Sequence> Hide(float insertTime, float duration)
    {
        List<Sequence> sequences = new List<Sequence>();

        foreach (var animation in animations)
        {
            sequences.Add(animation.HideSequences(insertTime, duration));
        }

        return sequences;
    }
    public List<Sequence> Hide()
    {
        List<Sequence> sequences = new List<Sequence>();

        foreach (var animation in animations)
        {
            sequences.Add(animation.HideSequences());
        }

        return sequences;
    }
    public virtual void OnStartHide()
    {
        state = VisibleState.Disappearing;
        onStartHide?.Invoke();
    }
    public virtual void OnFinishHide()
    {
        canvas.enabled = false;
        state = VisibleState.Disappeared;
        onFinishHide?.Invoke();
    }

    public static T Get<T>() where T : UIView
    {
        foreach (var view in UIViews)
        {
            if (view is T)
            {
                return view as T;
            }
        }
        Debug.LogError("Not Found UIView" + nameof(T));
        return null;
    }
    public static UIView Get(string viewName)
    {
        foreach (var UIView in UIViews)
        {
            if (UIView.name == viewName)
            {
                return UIView;
            }
        }

        Debug.LogError("Not Found UIView : " + viewName);
        return null;
    }
    public virtual void OnDestroy()
    {
        UIViews.Remove(this);
    }
    public virtual void Update()
    {
        if (canvas == null || !isPop)
        {
            return;
        }

        if (!canvas.enabled)
        {
            return;
        }

        if (Input.touchCount > 0 && state == VisibleState.Appeared)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    UIManager.Pop();
                    state = VisibleState.Wait;
                }
            }
        }
    }

    public void Remove()
    {
        UIViews.Remove(this);
    }
}
