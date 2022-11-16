using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapBox : MonoBehaviour
{
    public RectTransform rect;
    public Image background;
    public Image image;

    public Sprite back_orizin;
    public Sprite back_tansparent;
    public void DisableImage()
    {
        background.sprite = back_tansparent;
        image.sprite = back_tansparent;
    }
    public void EnableImage(Sprite image)
    {
        background.sprite = back_orizin;
        this.image.sprite = image;
    }
}
