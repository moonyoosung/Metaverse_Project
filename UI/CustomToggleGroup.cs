using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomToggleGroup : ToggleGroup
{
    public List<Toggle> GetToggles()
    {
        return m_Toggles;
    }
  
}
