using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIContentGroup : UILayoutGroup
{
    public RectTransform rectTransform;
    public override void Set()
    {
        base.Set();
        rectTransform = GetComponent<RectTransform>();
    }
    public override bool IsADDAvailable()
    {
        if (group.transform.childCount >= 4)
        {
            return false;
        }
        return true;
    }
}
