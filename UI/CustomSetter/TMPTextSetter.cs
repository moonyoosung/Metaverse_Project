

using Slash.Unity.DataBind.Foundation.Setters;
using UnityEngine;
using TMPro;

[AddComponentMenu("Data Bind/UnityUI/Setters/[DB] TMPText Text Setter (Unity)")]

public class TMPTextSetter : ComponentSingleSetter<TMP_Text, string>
{
    protected override void UpdateTargetValue(TMP_Text target, string value)
    {
        target.text = value;
    }
}
