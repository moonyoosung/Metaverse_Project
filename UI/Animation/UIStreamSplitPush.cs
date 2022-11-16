using MindPlus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStreamSplitPush : UIAnimManager.Stream
{
    private UIView[] leftTargets;
    private UIView[] rightTargets;
    private UINavigation[] navigation;
    private Action onComplete;
    private bool isShowImmediately;
    private bool isHideImmediately;
    public override string ID => leftTargets != null && rightTargets != null ?
        "SplitBothPush" + leftTargets[0].name + rightTargets[0].name :
        leftTargets != null ? "SplitLeftPush" + leftTargets[0].name :
        "SplitRightPush" + rightTargets[0].name;

    public UIStreamSplitPush(UINavigation[] navis, bool isShowImmediately, bool isHideImmediately, UIView[] leftViews, UIView[] rightViews, Action onComplete = null) : base()
    {
        this.navigation = navis;
        this.leftTargets = leftViews;
        this.rightTargets = rightViews;
        this.isShowImmediately = isShowImmediately;
        this.isHideImmediately = isHideImmediately;
        this.onComplete = onComplete;
    }

    public override IEnumerator Handle(UIAnimManager manager, MonoBehaviour caller)
    {
        UIView leftCurrent = null;
        UIView rightCurrent = null;
        bool isLeftHasIndependent = false;
        bool isRightHasIndependent = false;
        bool isLeftWithOtherSide = false;
        bool isRightWithOtherSide = false;

        leftCurrent = navigation[0].Current;
        if (leftTargets != null)
        {
            foreach (var target in leftTargets)
            {
                if (target.isIndepedent && !isLeftHasIndependent)
                {
                    isLeftHasIndependent = true;
                }
                if (target.isWithOtherSide && !isLeftWithOtherSide)
                {
                    isLeftWithOtherSide = true;
                }
                navigation[0].Push(target);
            }
        }

        rightCurrent = navigation[1].Current;
        if (rightTargets != null)
        {
            foreach (var target in rightTargets)
            {
                if (target.isIndepedent && !isRightHasIndependent)
                {
                    isRightHasIndependent = true;
                }
                if (target.isWithOtherSide && !isRightWithOtherSide)
                {
                    isRightWithOtherSide = true;
                }
                navigation[1].Push(target);
            }
        }

        UIStreamHide hide = null;
        if (leftCurrent != null && rightCurrent != null)
        {
            if (isLeftHasIndependent && !isRightHasIndependent)
            {
                if (isLeftWithOtherSide)
                {
                    hide = new UIStreamHide(isHideImmediately, null, leftCurrent);
                }
                else
                {
                    hide = new UIStreamHide(isHideImmediately, null, leftCurrent, rightCurrent);
                }
            }
            else if (!isLeftHasIndependent && isRightHasIndependent)
            {
                if (isRightWithOtherSide)
                {
                    hide = new UIStreamHide(isHideImmediately, null, leftCurrent);
                }
                else
                {
                    hide = new UIStreamHide(isHideImmediately, null, rightCurrent, rightCurrent);
                }
            }
            else
            {
                hide = new UIStreamHide(isHideImmediately, null, leftCurrent, rightCurrent);
            }
        }
        else if (leftCurrent != null)
        {
            if (isLeftWithOtherSide)
            {
                hide = new UIStreamHide(isHideImmediately, null, leftCurrent, rightCurrent);
            }
            else
            {
                hide = new UIStreamHide(isHideImmediately, null, leftCurrent);
            }
        }
        else if (rightCurrent != null)
        {
            if (isRightWithOtherSide)
            {
                hide = new UIStreamHide(isHideImmediately, null, leftCurrent, rightCurrent);
            }
            else
            {
                hide = new UIStreamHide(isHideImmediately, null, rightCurrent);
            }
        }
        else
        {
            Debug.Log("Error=>>>>>>>>>>>>>>SplitHide Both Null");
        }

        if (hide != null)
        {
            yield return caller.StartCoroutine(hide.Handle(manager, caller));
        }

        UIStreamShow show = null;
        Debug.Log("======>SplitShow" + ID);

        if (leftTargets != null && rightTargets != null)
        {
            show = new UIStreamShow(isShowImmediately,null, Util.MergeTwoArray(leftTargets, rightTargets));
        }
        else if (leftTargets != null)
        {
            show = new UIStreamShow(isShowImmediately,null, leftTargets);
        }
        else if (rightTargets != null)
        {
            show = new UIStreamShow(isShowImmediately,null, rightTargets);
        }
        else
        {
            Debug.Log("Error=>>>>>>>>>>>>>>SplitShow Both Null");
        }

        if (show != null)
        {
            yield return caller.StartCoroutine(show.Handle(manager, caller));
        }

        onComplete?.Invoke();
    }


}
