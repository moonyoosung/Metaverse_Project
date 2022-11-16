using MindPlus.Contexts.TitleView;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCustomView : UIFlatView
{
    private TitleCustomViewContext context;
    public CaptureScreen capture;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);

        this.context = new TitleCustomViewContext();
        ContextHolder.Context = context;

        context.onClickBack += OnClickBack;
        context.onClickNext += OnClickNext;
        context.onClickColor += OnClickColor;
        context.OnChangeAnchor += OnChangeAnchor;

        context.SetValue("IsActiveAnchorHead", true);
        context.SetValue("backButtonText", "Back to avatar");
    }


    private void OnChangeAnchor(AvatarPartsType type, bool isOn = true)
    {
        if (isOn)
        {
            ItemListView itemView = Get<ItemListView>();
            ItemListView.Option itemOption = ItemListView.Option.None;
            itemOption |= ItemListView.Option.Owned;
            itemView.Set(type, itemOption);
            context.SetValue("IsActiveColorButton", type == AvatarPartsType.Hair);
            if (type != AvatarPartsType.Hair)
                PopColorColorPaletteView();
            SetVirtualCamera(type);
        }
    }

    public void SetVirtualCamera(AvatarPartsType type)
    {
        CustomCameraContoller customCameraContoller = GameObject.FindObjectOfType<CustomCameraContoller>();

        switch (type)
        {
            case AvatarPartsType.Bags:
            case AvatarPartsType.Belts:
            case AvatarPartsType.Bottom:
            case AvatarPartsType.Hand:
            case AvatarPartsType.Top:
            case AvatarPartsType.Glove:
                customCameraContoller.ActiveVirtualCamera(CustomCameraContoller.VirtualCamera.Body);
                break;
            case AvatarPartsType.Head:
            case AvatarPartsType.Beard:
            case AvatarPartsType.Earrings:
            case AvatarPartsType.Glasses:
            case AvatarPartsType.Hair:
            case AvatarPartsType.Hats:
            case AvatarPartsType.Necklaces:
                customCameraContoller.ActiveVirtualCamera(CustomCameraContoller.VirtualCamera.Head);
                break;
        }
    }

    private void OnClickColor()
    {
        context.onClickColor -= OnClickColor;

        ColorPaletteView colorPalette = Get<ColorPaletteView>();
        Push(colorPalette.name, true, true, () => { context.onClickColor += OnClickColor; }, colorPalette);
        context.SetValue("IsActiveColorPallete", true);
        context.SetValue("IsActiveColorButton", false);
        context.SetValue("IsActiveAnchorHeadSub", false);
        context.SetValue("backButtonText", "Done");
    }

    private void OnClickNext()
    {
        context.onClickNext -= OnClickNext;
        CharacterView characterView = Get<CharacterView>();
        HalfPanelView halfPanelView = Get<HalfPanelView>();
        ConductAgreeView conductAgreeView = Get<ConductAgreeView>();

        MindPlusPlayer player = Get<SelectCharacterView>().MindPlusPlayer;
        player.GetPart<PlayerRig>().customization.PutUserCustome();

        if (capture)
        {
            context.SetValue("IsActiveCapture", true);
            capture.onShutter.Invoke(player.GetPart<PlayerRig>().animationController.animator, () => context.SetValue("IsActiveCapture", false));
        }
        characterView.Push("", false, false, () => { context.onClickNext += OnClickNext; }, null,new UIView[] { halfPanelView , conductAgreeView});
    }
    public void SetItemListView()
    {
        ItemListView itemView = Get<ItemListView>();
        itemView.GetItemList();

        if (navigation.history.Count <= 0)
        {
            PushNavigation(itemView);
        }
    }
    private void OnClickBack()
    {
        context.onClickBack -= OnClickBack;

        if (navigation.Current is ItemListView)
        {
            Get<CharacterView>().Pop(false, true, false, false, true, null, () => { context.onClickBack += OnClickBack; });
        }
        else
        {
            SetItemListView();
            Pop(true, true, true, null, () => { context.onClickBack += OnClickBack; });
            context.SetValue("backButtonText", "Back to avatar");
            context.SetValue("IsActiveColorButton", true);
            context.SetValue("IsActiveColorPallete", false);
        }
    }

    public void PopColorColorPaletteView()
    {
        ColorPaletteView colorPalette = Get<ColorPaletteView>();
        if (navigation.Current == Get<ColorPaletteView>())
        {
            Pop(true, true, true, null, () =>
            {
                context.onClickBack += OnClickBack;
                context.SetValue("backButtonText", "Back to avatar");
                context.SetValue("IsActiveColorPallete", false);
                context.SetValue("IsActiveAnchorHeadSub", context.IsActiveAnchorHead);
            });
        }
    }

    public override void OnStartShow()
    {
        if (context.IsActiveAnchorHead)
            SetVirtualCamera(AvatarPartsType.Hair);
        if (context.IsActiveAnchorTopBottom)
        {
            if(context.IsBagPart)
                SetVirtualCamera(AvatarPartsType.Bags);
            else
                SetVirtualCamera(AvatarPartsType.Top);
        }

        
        base.OnStartShow();
    }

}
