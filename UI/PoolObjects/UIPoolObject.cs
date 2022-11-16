using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoolObject : MonoBehaviour
{
    private Transform orizinParent;
    private List<UIAnimFade.CustomGraphic> graphics = new List<UIAnimFade.CustomGraphic>();

    public string ID;
    protected void SetID(string ID)
    {
        this.ID = ID;
    }
    public virtual void Initialize(Transform parent)
    {
        orizinParent = parent;

        Graphic[] graphics = GetComponentsInChildren<Graphic>(true);
        foreach (var graphic in graphics)
        {
            this.graphics.Add(new UIAnimFade.CustomGraphic(graphic));
        }
    }
    public virtual void InActivePool()
    {
        transform.SetParent(orizinParent);
        gameObject.SetActive(false);
        ID = string.Empty;

        foreach (var graphic in graphics)
        {
            graphic.graphic.color = new Color(graphic.graphic.color.r, graphic.graphic.color.g, graphic.graphic.color.b, graphic.alpha);
        }
    }
}
