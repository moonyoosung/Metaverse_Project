using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggle : UIPoolObject
{
    public Toggle toggle;
    public Text uiText;
    private Action<string> onSelectToggle;
    public override void Initialize(Transform parent)
    {
        base.Initialize(parent);
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener((isOn) => { if (isOn) onSelectToggle?.Invoke(ID); });
    }
    public virtual void Set(ToggleGroup group, Action<string> onSelectToggle, string text, Sprite Image = null)
    {
        toggle.group = group;
        this.onSelectToggle = onSelectToggle;
        uiText.text = text;
        SetID(text);
    }

}
