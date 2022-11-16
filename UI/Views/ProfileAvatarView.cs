using MindPlus.Contexts.Master.ProfileView;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProfileAvatarView : UIView
{
    private Persistent persistent;
    private ProfileAvatarViewContext context;
    public PlayerRig rig;
    private Avatar avatar;
    public ProfileRotationArea profileRotationArea;
    public RawImage image;
    public CaptureScreen capture;

    Coroutine coVirtualCamera;
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.persistent = persistent;
        this.context = new ProfileAvatarViewContext();
        ContextHolder.Context = context;
        SetAvatarActive(false);
        context.onClickEdit += OnClick;
        UIManager UIManager = (uIManager as UIManager);
    }

    public override void OnStartShow()
    {
        //this.persistent.APIManager.ResisterEvent(this);
        image.enabled = true;
        base.OnStartShow();
        SetAvatarActive(true);
        if (coVirtualCamera != null)
            StopCoroutine(coVirtualCamera);
        coVirtualCamera = StartCoroutine(DoSetVirtualCamera());
    }

    public override void OnStartHide()
    {
        profileRotationArea.enabled = false;
        image.enabled = false;
        if (coVirtualCamera != null)
            StopCoroutine(coVirtualCamera);

        base.OnStartHide();
    }

    IEnumerator DoSetVirtualCamera()
    {
        CustomCameraContoller customCameraContoller = null;
        while (customCameraContoller == null)
        {
            customCameraContoller = GameObject.FindObjectOfType<CustomCameraContoller>();
            yield return null;
        }
        customCameraContoller.ActiveVirtualCamera(CustomCameraContoller.VirtualCamera.Body);
    }

    public void Set(bool isMyProfile, CustomAvatar custom)
    {
        SetAvatar(custom);
        context.SetValue("IsActiveEdit", isMyProfile);
    }

    public void SetAvatar(CustomAvatar customAvatar)
    {
        if (avatar == null)
        {
            rig.customization.Init();
            avatar = new Avatar(persistent.ResourceManager.GetAvatarProbs(), rig.transform,
            customAvatar, rig.customization.Rebind, rig.customization.OnSetBone);
            rig.customization.avatar = avatar;
        }
        else
        {
            avatar.ResetParts(customAvatar);
        }

        profileRotationArea.SetTarget(avatar.avataSet.gameObject);
        profileRotationArea.enabled = true;

        foreach (var skinnedMeshRenderer in avatar.avataSet.skinnedMeshRenderers)
        {
            if (skinnedMeshRenderer == null || skinnedMeshRenderer.gameObject == null) continue;

            skinnedMeshRenderer.gameObject.layer = LayerMask.NameToLayer("Avatar");
        }
    }

    public void SetAvatarActive(bool isActive)
    {
        rig.gameObject.SetActive(isActive);
    }

    private void OnClick()
    {
        persistent.UIManager.PopToRoot(null,1);
        CustomView view = Get<CustomView>();
        ItemListView itemView = Get<ItemListView>();
        itemView.Set();
        UIManager.Push(view.name, false, false, null, view);
        view.Push(view.name, false, false, null, itemView);
    }

    public void Capture()
    {
        if (NetworkManager.Instance.currentRoomManager && NetworkManager.Instance.currentRoomManager.player)
        {
            context.SetValue("IsActiveCapture", true);
            if (capture)
                capture.onShutter.Invoke(rig.GetComponent<Animator>(), () => {
                    context.SetValue("IsActiveCapture", false);
                    SetAvatarActive(false);
                });
        }
    }
}

