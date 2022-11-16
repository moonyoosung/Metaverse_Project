using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pixelplacement;

public class LocomotionButtonUI : MonoBehaviour
{
    public Image box;
    public Image icon;
    public Text text;

    public void On(float duration = 0.1f)
    {
        Tween.Color(box, new Color(1, 1, 1, 1), duration, 0);
        Tween.Color(icon, Color.white, duration, 0);
        Tween.Color(text, new Color(54 / 255f, 144 / 255f, 233 / 255f, 1), duration, 0);
    }

    public void Off(float duration = 0.1f)
    {
        Tween.Color(box, new Color(1, 1, 1, 0), duration, 0);
        Tween.Color(icon, Color.black, duration, 0);
        Tween.Color(text, new Color(106 / 255f, 106 / 255f, 106 / 255f, 1), duration, 0);
    }
}
