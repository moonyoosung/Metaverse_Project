using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UIPeopleGroup : UILayoutGroup
{
    public RectTransform rectTransform;

    public override void Set()
    {
        base.Set();
        rectTransform = GetComponent<RectTransform>();
    }

    public override bool IsADDAvailable()
    {
        if (group.transform.childCount >= 7)
        {
            return false;
        }
        return true;
    }
}
