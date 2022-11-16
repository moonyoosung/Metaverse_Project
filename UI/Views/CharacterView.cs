using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterView : UIMultiSplitView
{
    public ProfileRotationArea profileRotationArea;
    public RawImage image;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
    }
    public override void OnStartShow()
    {
        profileRotationArea.SetTarget(Get<SelectCharacterView>().MindPlusPlayer.GetPart<PlayerRig>().customization.avatar.avataSet.gameObject);
        profileRotationArea.enabled = true;
        image.enabled = true;
        base.OnStartShow();
    }
    public override void OnStartHide()
    {
        base.OnStartHide();
        profileRotationArea.enabled = false;
        image.enabled = false;
    }
}
