using MindPlus.Contexts.Master.Menus;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class CustomView : UIFlatView, PostPurchaseItemCommand.IEventHandler, GetUserCustomCommand.IEventHandler
{
    private CustomViewContext context;
    private APIManager apiManager;
    private LocalPlayerData localPlayerData;
    private ProfileAvatarView profileAvatarView;
    public PlayerRig originRig;
    private List<string> differences = new List<string>();
    public List<UIItemListData.Item> ownedChangeItems;
    public List<UIItemListData.Item> newChangeItems;

    public PurchaseCheckoutLayout purchaseCheckoutLayout;
    public ProfileRotationArea profileRotationArea;
    public RawImage image;

    public CustomAvatar originAvatar;
    private AvatarPartsType currentAvatarPartsType = AvatarPartsType.Hair;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.apiManager = persistent.APIManager;
        this.localPlayerData = persistent.AccountManager.PlayerData;
        this.context = new CustomViewContext();
        ContextHolder.Context = context;

        apiManager.ResisterEvent(this);

        context.onClickBack += OnClickBack;
        context.onClickColor += OnClickColor;
        context.OnChangeAnchor += OnChangeAnchor;
        context.onClickDone += OnClickDone;
        context.onClickCheckout += OnClickCheckout;
        context.onClickClose += () =>
        {
            PopColorColorPaletteView();
            UIManager.Pop();
            Get<ProfileAvatarView>().SetAvatarActive(false);
        };
        context.onClickClosePopup += () =>
        {
            context.SetValue("IsActiveCheckout", false);
            context.SetValue("IsActiveNotice", false);
        };

        context.SetValue("IsActiveAnchorHead", true);
        context.SetValue("IsActiveBackButton", false);
        context.SetValue("backButtonText", "Back to avatar");
        ActiveSelfBuyButton(true, false);
    }
    public override void OnStartShow()
    {
        image.enabled = true;
        profileAvatarView = Get<ProfileAvatarView>();
        if (NetworkManager.Instance.currentRoomManager.player != null)
        {
            originRig = NetworkManager.Instance.currentRoomManager.player.GetPart<PlayerRig>();
            originRig.customization.avatar.onChageCustom += OnChangeCustom;
            SetAvatar(originRig.customization.avatar.customAvatar);
            originAvatar = originRig.customization.avatar.customAvatar.GetNew();
        }

        OnChangeAnchor(currentAvatarPartsType, true);
        SetCost(localPlayerData.coin, localPlayerData.heart);

        profileRotationArea.SetTarget(profileAvatarView.rig.customization.avatar.avataSet.gameObject);
        profileRotationArea.enabled = true;

        if (context.IsActiveAnchorHead)
            SetVirtualCamera(AvatarPartsType.Hair);
        if (context.IsActiveAnchorTopBottom)
        {
            if (context.IsBagPart)
                SetVirtualCamera(AvatarPartsType.Bags);
            else
                SetVirtualCamera(AvatarPartsType.Top);
        }

        ActiveSelfBuyButton(true, false);
        context.SetValue("IsActiveCheckout", false);
        context.SetValue("IsActiveNotice", false);
        base.OnStartShow();
        Get<ItemListView>().onGetItemList += OnChangeCustom;
    }

    public override void OnFinishShow()
    {
        base.OnFinishShow();

        ItemListView itemView = Get<ItemListView>();
        itemView.Set(currentAvatarPartsType);
        Get<ProfileAvatarView>().SetAvatarActive(true);
    }

    public override void OnFinishHide()
    {
        Get<ItemListView>().onGetItemList -= OnChangeCustom;
        if (originRig != null)
        {
            originRig.customization.avatar.onChageCustom -= OnChangeCustom;
            originRig.customization.avatar.ResetParts(originAvatar);
            Get<ProfileAvatarView>().Set(true, originAvatar);
        }
        base.OnFinishHide();
    }

    public override void OnStartHide()
    {
        profileRotationArea.enabled = false;
        image.enabled = false;
        base.OnStartHide();
    }

    void SetCost(int coin, int heart)
    {
        context.SetValue("costCoinText", coin == 0 ? "0" : string.Format(Format.Money, coin));
        context.SetValue("costHeartText", heart == 0 ? "0" : string.Format(Format.Money, heart));
    }
    void ActiveSelfBuyButton(bool isActive, bool isInteractable = true)
    {
        context.SetValue("IsActiveBuyButton", isActive);
        context.SetValue("IsActiveDoneButton", !isActive);
        context.SetValue("IsInteractableBuyButton", isInteractable);
    }

    void OnChangeCustom()
    {
        SetAvatar(originRig.customization.avatar.customAvatar);
        differences = CustomAvatar.GetCustomDifferences(originAvatar, profileAvatarView.rig.customization.avatar.customAvatar);
        ownedChangeItems = new List<UIItemListData.Item>();
        newChangeItems = new List<UIItemListData.Item>();

        for (int i = differences.Count - 1; i >= 0; i--)
        {
            foreach (var itemData in Get<ItemListView>().GetItemDatas())
            {
                if (itemData.customId == differences[i])
                {
                    if (itemData.owned == 1)
                    {
                        ownedChangeItems.Add(itemData);
                    }
                    else
                    {
                        newChangeItems.Add(itemData);
                    }
                    break;
                }
            }
        }

        if (newChangeItems.Count > 0)
            ActiveSelfBuyButton(true, true);
        else if (ownedChangeItems.Count > 0)
            ActiveSelfBuyButton(false);
        else
            ActiveSelfBuyButton(true, false);
    }

    public void SetAvatar(CustomAvatar customAvatar)
    {
        profileAvatarView.SetAvatar(customAvatar);
    }

    private void OnChangeAnchor(AvatarPartsType type, bool isOn = true)
    {
        if (isOn)
        {
            currentAvatarPartsType = type;
            context.SetValue("IsActiveColorButton", type == AvatarPartsType.Hair);
            if (type != AvatarPartsType.Hair)
                PopColorColorPaletteView();
            if (!string.IsNullOrEmpty(localPlayerData.userId))
                try
                {
                    ItemListView itemView = Get<ItemListView>();
                    itemView.Set(type);
                }
                catch (System.Exception e)
                {
                    Debug.Assert(false, e.Message);
                }
            SetVirtualCamera(type);
        }
    }

    private void OnClickColor()
    {
        context.onClickColor -= OnClickColor;

        ColorPaletteView colorPalette = Get<ColorPaletteView>();
        Push(colorPalette.name, true, true, () => { context.onClickColor += OnClickColor; }, colorPalette);
        context.SetValue("IsActiveColorPallete", true);
        context.SetValue("IsActiveAnchorHeadSub", false);
        context.SetValue("IsActiveBackButton", true);
        context.SetValue("backButtonText", "Done");
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
            PopColorColorPaletteView();
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
                context.SetValue("IsActiveBackButton", false);
                context.SetValue("IsActiveColorPallete", false);
                context.SetValue("IsActiveAnchorHeadSub", context.IsActiveAnchorHead);
            });
        }
    }

    public void OnClickDone()
    {
        if (context.IsActiveDoneButton)
        {
            //originAvatar = originRig.customization.avatar.customAvatar.GetNew();
            originAvatar = profileAvatarView.rig.customization.avatar.customAvatar.GetNew();
            originRig.customization.avatar.customAvatar = originAvatar.GetNew();
            originRig.customization.PutUserCustome();

            PopColorColorPaletteView();
            UIManager.Pop();
            Get<ProfileAvatarView>().Capture();
        }
        else if (context.IsActiveBuyButton)
        {
            int itemCount = newChangeItems.Count;
            int heartCost = 0;
            int coinCost = 0;
            foreach (var item in newChangeItems)
            {
                if (item.isCoin())
                    coinCost += item.GetCost();
                else
                    heartCost += item.GetCost();
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(itemCount);
            sb.Append(" item(s)");
            context.SetValue("CheckoutItemCountText", sb.ToString());
            context.SetValue("IsActiveCheckout", true);
            StartCoroutine(purchaseCheckoutLayout.DoSetHeartCoin(heartCost, coinCost));
        }
    }

    public void OnClickCheckout()
    {
        AudioManager.Instance.EffectSoundPlay(AudioManager.Effect.PURCHASE);
        JArray jArray = new JArray();
        foreach (var item in newChangeItems)
        {
            jArray.Add(item.customId);
        }

        apiManager.PostAsync(new PostPurchaseItemCommand(), string.Format("/users/{0}/custom", localPlayerData.userId), new JObject

           (new JProperty("customs", jArray))
        );
    }

    public void OnPostPurchaseItemSuccess(NetworkMessage message)
    {
        context.SetValue("IsActiveCheckout", false);
        Get<ItemListView>().GetItemList();
        OnChangeCustom();
        GetUserCustom(localPlayerData.userId);
    }

    public void OnPostPurchaseItemFailed(NetworkMessage message)
    {
        context.SetValue("IsActiveCheckout", false);
        int errorCode = APIManager.GetErrorCode(message);

        switch (errorCode)
        {
            case 1010:
            case 1011:
                context.SetValue("IsActiveNotice", true);
                break;
        }
    }

    public void GetUserCustom(string userId)
    {
        apiManager.GetAsync(new GetUserCustomCommand(), string.Format("/users/{0}", userId));
    }

    public void OnGetUserCustomSuccess(NetworkMessage message)
    {
        LocalPlayerData playerData = JsonUtility.FromJson<LocalPlayerData>(message.body);
        if (playerData.userId == localPlayerData.userId)
        {
            localPlayerData.userId = playerData.userId;
            localPlayerData.coin = playerData.coin;
            localPlayerData.heart = playerData.heart;
            SetCost(localPlayerData.coin, localPlayerData.heart);
            Get<ProfileView>().SetCost(localPlayerData.coin, localPlayerData.heart);
        }
    }

    public void OnGetUserCustomFailed(NetworkMessage message)
    {

    }
}
