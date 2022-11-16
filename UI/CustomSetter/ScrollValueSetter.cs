using Slash.Unity.DataBind.Foundation.Setters;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Data Bind/UnityUI/Setters/[DB] Scroll Value Setter (Unity)")]
class ScrollValueSetter : ComponentSingleSetter<Scrollbar, float>
{
    protected override void UpdateTargetValue(Scrollbar target, float value)
    {
        target.value = value;
    }
}
