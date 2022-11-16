using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStreamSplitPop : UIAnimManager.Stream
{
    private UINavigation[] navigation;
    private bool isHideImmediately;
    private bool isShowImmediately;
    private bool isCurrentShow;
    private bool isLeft;
    private bool isRight;
    private Action onPop;
    private Action onComplete;
    public UIStreamSplitPop(UINavigation[] navis, bool isShowImmediately = false, bool isHideImmediately = false, bool isCurrentShow = true,
        bool isLeft = true, bool isRight = true, Action onPop = null, Action onComplete = null) : base()
    {
        this.navigation = navis;
        this.isShowImmediately = isShowImmediately;
        this.isHideImmediately = isHideImmediately;
        this.isCurrentShow = isCurrentShow;
        this.isLeft = isLeft;
        this.isRight = isRight;
        this.onPop = onPop;
        this.onComplete = onComplete;
    }
    public override string ID => isLeft && isRight ? "SplitBothPop" + navigation[0].Current.name + navigation[1].Current.name :
        isLeft ? "SplitLeftPop" + navigation[0].Current.name : "SplitRightPop" + navigation[1].Current.name;

    public override IEnumerator Handle(UIAnimManager manager, MonoBehaviour caller)
    {
        UIStreamHide hide = null;
        UIView leftPrev = null;
        UIView leftNext = null;
        UIView rightPrev = null;
        UIView rightNext = null;

        if (!isLeft && !isRight)
        {
            leftPrev = navigation[0].Current;
            leftNext = navigation[0].Pop();
            rightPrev = navigation[1].Current;
            rightNext = navigation[1].Pop();

            onPop?.Invoke();

            hide = new UIStreamHide(isHideImmediately, null, leftPrev, rightPrev);
        }
        else if (isLeft)
        {
            leftPrev = navigation[0].Current;
            leftNext = navigation[0].Pop();
            onPop?.Invoke();

            hide = new UIStreamHide(isHideImmediately, null, leftPrev);
        }
        else if (isRight)
        {
            rightPrev = navigation[1].Current;
            rightNext = navigation[1].Pop();
            onPop?.Invoke();

            hide = new UIStreamHide(isHideImmediately, null, rightPrev);
        }

        if (hide != null)
        {
            yield return caller.StartCoroutine(hide.Handle(manager, caller));
        }

        if (isLeft && navigation[1].Current != null)
        {
            rightNext = navigation[1].Current;
        }
        else if (isRight && navigation[0].Current != null)
        {
            leftNext = navigation[0].Current;
        }

        UIStreamShow show = null;

        if (leftPrev != null && rightPrev != null)
        {
            //???? ?? Hide ?? ???? ?????? ????
            if (leftPrev.isIndepedent)
            {
                if (rightNext != null && !rightNext.IsShowing())
                {
                    show = new UIStreamShow(isShowImmediately,null, leftNext, rightNext);
                }
                else
                {
                    show = new UIStreamShow(isShowImmediately,null, leftNext);
                }
            }
            else
            {
                if (leftNext != null && !leftNext.IsShowing())
                {
                    show = new UIStreamShow(isShowImmediately,null, leftNext, rightNext);
                }
                else
                {
                    show = new UIStreamShow(isShowImmediately,null, rightNext);
                }
            }
        }
        else if (leftPrev != null)
        {
            //?????? Hide ?? ???? ????
            if (rightNext != null && !rightNext.IsShowing())
            {
                show = new UIStreamShow(isShowImmediately,null, leftNext, rightNext);
            }
            else
            {
                show = new UIStreamShow(isShowImmediately,null, leftNext);
            }

        }
        else if (rightPrev != null)
        {
            //???????? Hide ?? ?????? ????
            if (leftNext != null && !leftNext.IsShowing())
            {
                show = new UIStreamShow(isShowImmediately,null, leftNext, rightNext);
            }
            else
            {
                show = new UIStreamShow(isShowImmediately,null, rightNext);
            }
        }


        if (show != null)
        {
            yield return caller.StartCoroutine(show.Handle(manager, caller));
        }
        onComplete?.Invoke();
    }
}
