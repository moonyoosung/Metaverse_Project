using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStreamPopToRoot : UIAnimManager.Stream
{
    private UINavigation navigation;
    private bool isHideImmediately;
    private bool isShowImmediately;
    private bool isCurrentShow;
    private int stackCnt;
    public override string ID => "PopToRoot" + stackCnt;
    public UIStreamPopToRoot(UINavigation navigation, int stackCnt = 0, bool isShowImmediately = false, bool isHideImmediately = false, bool isCurrentShow = true) : base()
    {
        this.navigation = navigation;
        this.stackCnt = stackCnt;
        this.isHideImmediately = isHideImmediately;
        this.isShowImmediately = isShowImmediately;
        this.isCurrentShow = isCurrentShow;
    }

    public override IEnumerator Handle(UIAnimManager manager, MonoBehaviour caller)
    {
        UIView[] prev = navigation.GetShowing(stackCnt);
        UIView target = navigation.PopToRoot(stackCnt);

        UIStreamHide hide = new UIStreamHide(isHideImmediately, null, prev);
        yield return caller.StartCoroutine(hide.Handle(manager, caller));

        if(target != null && isCurrentShow)
        {
            UIStreamShow show = new UIStreamShow(isShowImmediately,null, target);
            yield return caller.StartCoroutine(show.Handle(manager, caller));
        }
    }
}
