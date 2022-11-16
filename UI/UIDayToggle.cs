using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDayToggle : MonoBehaviour
{
    public Action<int, bool> onToggleChanged;
    public int dateNum;
    public void OnToggleChanged(bool isOn)
    {
        onToggleChanged?.Invoke(dateNum, isOn);
    }
}
