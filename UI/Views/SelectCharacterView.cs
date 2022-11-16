using MindPlus.Contexts.TitleView;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SelectCharacterView : UIView
{
    private SelectCharacterViewContext context;
    private LocalPlayerData localPlayerData;
    public MindPlusPlayer MindPlusPlayer;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);

        context = new SelectCharacterViewContext();
        ContextHolder.Context = context;

        context.onClickNext += OnClickNext;
        localPlayerData = persistent.AccountManager.PlayerData;
    }

    public override void OnStartShow()
    {
        base.OnStartShow();
        GameObject.FindObjectOfType<CustomCameraContoller>().ActiveVirtualCamera(CustomCameraContoller.VirtualCamera.Body);
        if (MindPlusPlayer != null)
        {
            var customization = MindPlusPlayer.GetPart<PlayerRig>().customization;
            customization.avatar.ResetParts(CustomAvatar.GetInitialSetting(customization.avatar.character));
        }
    }

    private void OnClickNext()
    {
        context.onClickNext -= OnClickNext;

        CharacterView characterView = Get<CharacterView>();

        TitleCustomView view = Get<TitleCustomView>();
        view.SetItemListView();

        characterView.Push("", false, false, ()=> { context.onClickNext += OnClickNext; }, null,new UIView[] { view });
    }

    public void OnCreatePlayer(MindPlusPlayer player)
    {
        this.MindPlusPlayer = player;
        Customization customization = player.GetPart<PlayerRig>().customization;

        context.onClickHeadChange = (isLeft) =>
            {

                if (isLeft)
                    customization.avatar.SetNextHead(AvatarPartsType.Head);
                else
                    customization.avatar.SetBeforeHead(AvatarPartsType.Head);

                localPlayerData.bags = customization.avatar.GetPartsWholeString(AvatarPartsType.Bags);
                localPlayerData.belts = customization.avatar.GetPartsWholeString(AvatarPartsType.Belts);
                localPlayerData.bottom = customization.avatar.GetPartsWholeString(AvatarPartsType.Bottom);
                localPlayerData.earrings = customization.avatar.GetPartsWholeString(AvatarPartsType.Earrings);
                localPlayerData.glasses = customization.avatar.GetPartsWholeString(AvatarPartsType.Glasses);
                localPlayerData.hair = customization.avatar.GetPartsWholeString(AvatarPartsType.Hair);
                //localPlayerData.custom.hand = customization.avatar.GetIndex(AvatarPartsType.Hand);
                localPlayerData.hats = customization.avatar.GetPartsWholeString(AvatarPartsType.Hats);
                localPlayerData.head = customization.avatar.GetPartsWholeString(AvatarPartsType.Head);
                localPlayerData.necklaces = customization.avatar.GetPartsWholeString(AvatarPartsType.Necklaces );
                localPlayerData.top = customization.avatar.GetPartsWholeString(AvatarPartsType.Top);
            };
    }
}
