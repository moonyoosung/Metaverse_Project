using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassListView : UIView
{
    public RectTransform content;

    public override void OnFinishHide()
    {
        base.OnFinishHide();
        content.anchoredPosition = Vector2.zero;
        Get<LevelCardView>().ResistLevelCardButton();
    }
}
