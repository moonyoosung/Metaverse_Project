using Slash.Unity.DataBind.Foundation.Setters;
using UnityEngine;
using TMPro;

[AddComponentMenu("Data Bind/UnityUI/Setters/[DB] TMPText MaxVisible Setter (Unity)")]

public class TmpTextMaxVisibleSetter : ComponentSingleSetter<TMP_Text, int>
{
    protected override void UpdateTargetValue(TMP_Text target, int value)
    {
        target.maxVisibleLines = value;
    }
}