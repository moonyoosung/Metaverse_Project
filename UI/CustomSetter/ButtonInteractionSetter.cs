
using Slash.Unity.DataBind.Foundation.Setters;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Data Bind/UnityUI/Setters/[DB] Button Interact Setter (Unity)")]
public class ButtonInteractionSetter : ComponentSingleSetter<Button, bool>
{
    protected override void UpdateTargetValue(Button target, bool value)
    {
        target.interactable = value;
    }
}
