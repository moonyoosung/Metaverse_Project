using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChallCategory : UIPoolObject
{
    [GUIColor(1f, 0f, 0f, 1f)]
    public VerticalLayoutGroup verticalLayoutGroup;
    [GUIColor(1f, 0f, 0f, 1f)]
    public GridLayoutGroup gridLayoutGroup;
    [GUIColor(1f, 0f, 0f, 1f)]
    public Text text;

    public void SetText()
    {
        string Title = "";

        if (ID.Length == 0)
            Title = "Empty String";
        else if (ID.Length == 1)
            Title = char.ToUpper(ID[0]).ToString();
        else
        {
            if (ID == "inapp")
            {
                ID = "In-App";
            }
            Title = char.ToUpper(ID[0]) + ID.Substring(1);
        }

        text.text = string.Format("{0} Activities({1}) ", Title, gridLayoutGroup.transform.childCount);
    }
    public override void InActivePool()
    {
        base.InActivePool();
        text.text = string.Empty;
    }

    public void LayoutForceUpdate()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)gridLayoutGroup.transform);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)verticalLayoutGroup.transform);
    }
}
