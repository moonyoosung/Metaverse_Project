using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBioPoint : UIPoolObject
{
    public Image image;
    private RectTransform rectTransform;
    private List<string> challengeIds;
    private ImageContainer challImages;
    public override void Initialize(Transform parent)
    {
        base.Initialize(parent);
        this.rectTransform = GetComponent<RectTransform>();
    }
    public override void InActivePool()
    {
        base.InActivePool();
        challengeIds.Clear();
        challImages = null;
    }
    public void Set(Vector2 localPosition, List<string> challengeIds, ImageContainer imageContainer)
    {
        rectTransform.anchoredPosition = localPosition;
        this.challengeIds = new List<string>(challengeIds);
        this.challImages = imageContainer;
        image.enabled = false;
    }

    public void CheckPoint(string targetId)
    {
        foreach (var challengeID in challengeIds)
        {
            if (targetId == challengeID)
            {
                if (!image.enabled)
                {
                    image.sprite = challImages.Get(targetId);
                    image.enabled = true;
                }
                return;
            }
        }
        image.enabled = false;
    }
    public void DisableImage()
    {
        image.enabled = false;
    }
}
