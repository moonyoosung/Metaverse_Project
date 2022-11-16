using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapToggle : MonoBehaviour
{
    public Image background;
    public Text dateText;
    public Toggle toggle;

    public void Initialize()
    {
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(OnValueChaged);
    }

    private void OnValueChaged(bool isOn)
    {
        background.gameObject.SetActive(!isOn);
        toggle.graphic.gameObject.SetActive(isOn);
    }
}
