using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageToggle : UIToggle
{
    public Image uiImage;

    public override void Set(ToggleGroup group, Action<string> onSelectToggle, string text, Sprite Image = null)
    {
        base.Set(group, onSelectToggle, text, Image);
        uiImage.sprite = Image;
    }
}
