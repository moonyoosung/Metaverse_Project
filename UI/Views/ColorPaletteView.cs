using MindPlus.Contexts.Master;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPaletteView : UIView
{
    public CustomToggleGroup toggleGroup;
    private ColorPalleteViewContext context;
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.context = new ColorPalleteViewContext();
        ContextHolder.Context = context;
        foreach (var toggle in toggleGroup.GetToggles())
        {
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        
        context.onClickDone += () =>
        {
            ItemListView itemView = Get<ItemListView>();
            itemView.Set();
        };


    }
    //Avatar의 Top Bottom 머티리얼 가져와서 색상 적용하기

    private void OnValueChanged(bool isOn)
    {
        if (!isOn)
        {
            return;
        }

        SelectCharacterView selectCharacterView = Get<SelectCharacterView>();
        MindPlusPlayer player;
        if (selectCharacterView)
            player = Get<SelectCharacterView>().MindPlusPlayer;
        else
            player = NetworkManager.Instance.currentRoomManager.player;

        if (player == null)
        {
            return;
        }
        Customization customization = player.GetPart<PlayerRig>().customization;

        if (customization.avatar.avataSet.skinnedMeshRendererInfos[(int)AvatarPartsType.Hair] == null)
            return;

        Color targetColor = toggleGroup.GetFirstActiveToggle().colors.normalColor;


        string hexColor = ColorUtility.ToHtmlStringRGB(targetColor).ToLower();
        string color = GetColorId(hexColor);
        customization.avatar.customAvatar.SetColorString(AvatarPartsType.Hair, color);

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetColor("_BaseColor", targetColor);
        if(customization.avatar.avataSet.skinnedMeshRendererInfos[(int)AvatarPartsType.Hair].skinnedMeshRenderer)
            customization.avatar.avataSet.skinnedMeshRendererInfos[(int)AvatarPartsType.Hair].skinnedMeshRenderer.SetPropertyBlock(mpb);

        customization.avatar.ChangePart(customization.avatar.customAvatar.GetWholeString(AvatarPartsType.Hair));
    }

    public static string GetColorId(string hexColor)
    {
        string color = "";
        switch (hexColor)
        {
            case "ff9dff":
                color = "01";
                break;
            case "98bdff":
                color = "02";
                break;
            case "e08a65":
                color = "03";
                break;
            case "65e08a":
                color = "04";
                break;
            case "282728":
                color = "05";
                break;
            case "ed5875":
                color = "06";
                break;
            case "eed556":
                color = "07";
                break;
            case "b859ec":
                color = "08";
                break;
            case "984529":
                color = "09";
                break;
            case "9bfff9":
                color = "10";
                break;
            default:
                break;
        }
        return color;
    }

    public static Color GetColor(string id)
    {
        Color color = Color.white;
        switch (id)
        {
            case "01":
                ColorUtility.TryParseHtmlString("#ff9dffff", out color);
                break;
            case "02":
                ColorUtility.TryParseHtmlString("#151a6cff", out color);
                break;
            case "03":
                ColorUtility.TryParseHtmlString("#e08a65ff", out color);
                break;
            case "04":
                ColorUtility.TryParseHtmlString("#65e08aff", out color);
                break;
            case "05":
                ColorUtility.TryParseHtmlString("#282728ff", out color);
                break;
            case "06":
                ColorUtility.TryParseHtmlString("#ed5875ff", out color);
                break;
            case "07":
                ColorUtility.TryParseHtmlString("#eed556ff", out color);
                break;
            case "08":
                ColorUtility.TryParseHtmlString("#692187ff", out color);
                break;
            case "09":
                ColorUtility.TryParseHtmlString("#984529ff", out color);
                break;
            case "10":
                ColorUtility.TryParseHtmlString("#9bfff9ff", out color);
                break;
            default:
                break;
        }
        return color;
    }
}
