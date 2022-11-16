using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonGraphics : Button
{
    [GUIColor(0,1,0)]
    public List<Graphic> graphics = new List<Graphic>();
    public void GetChildGraphicsAtTarget()
    {
        Graphic[] childGraphics = targetGraphic.GetComponentsInChildren<Graphic>(true);
        List<Graphic> childs = new List<Graphic>();
        foreach (var child in childGraphics)
        {
            if(child == targetGraphic)
            {
                continue;
            }
            childs.Add(child);
        }
        this.graphics = childs;
    }



    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        Color color = Color.black;

        switch (state)
        {
            case SelectionState.Normal:
                color = this.colors.normalColor;
                break;
            case SelectionState.Highlighted:
                color = this.colors.highlightedColor;
                break;
            case SelectionState.Pressed:
                color = this.colors.pressedColor;
                break;
            case SelectionState.Selected:
                color = this.colors.selectedColor;
                break;
            case SelectionState.Disabled:
                color = this.colors.disabledColor;
                break;
        }

        if (base.gameObject.activeInHierarchy)
        {
            switch (this.transition)
            {
                case Selectable.Transition.ColorTint:
                    ColorTween(color * this.colors.colorMultiplier, instant);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
    private void ColorTween(Color targetColor, bool instant)
    {
        if (this.targetGraphic == null)
        {
            return;
        }
        foreach (var graphic in graphics)
        {
            graphic.CrossFadeColor(targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
        }
    }
}