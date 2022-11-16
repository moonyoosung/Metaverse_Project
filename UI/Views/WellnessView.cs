using MindPlus.Contexts.Master.Menus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellnessView : UIMultiSplitView
{
    private WellnessViewContext context;
    private Stack<string> titleTextStack = new Stack<string>();
    private LocalPlayerData playerData;
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        Sprite titleIcon = persistent.ResourceManager.ImageContainer.Get("menuwellness");
        Sprite backIcon = persistent.ResourceManager.ImageContainer.Get("menubackarrow");
        this.context = new WellnessViewContext(titleIcon, backIcon);
        this.playerData = persistent.AccountManager.PlayerData;
        ContextHolder.Context = context;
        context.onClickClose += OnClickClose;
        context.onClickBack += OnClickBack;
        titleTextStack.Push("Wellness");
        SetBackButton(false);

    }
    public override void OnStartSplitShow()
    {
        context.SetValue("CoinText", playerData.coin == 0 ? "0" : string.Format(Format.Money, playerData.coin));
        context.SetValue("HeartText", playerData.heart == 0 ? "0" : string.Format(Format.Money, playerData.heart));

        foreach (var navigation in navigations)
        {
            if (navigation.history.Count > 0)
            {
                if (navigation.Current is WellnessBiosView)
                {
                    return;
                }
                UIManager.Interrupt(new UIStreamShow(false, null, navigation.Current), navigation.ID);
            }
        }
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();
        titleTextStack.Clear();
        titleTextStack.Push("Wellness");
        SetBackButton(false);
        Get<BioActivityListView>().PoolClear();
        Get<WellnessBiosView>().ClearData();
    }
    public void SetBackButton(bool isInteractTitle, string titleText = "")
    {
        if (titleText != "")
        {
            titleTextStack.Push(titleText);
        }
        context.SetValue("IsTitleInteract", isInteractTitle);
        context.SetValue("TitleText", titleTextStack.Peek());
    }

    private void OnClickBack()
    {
        context.onClickBack -= OnClickBack;

        Pop(navigations[0].Current == null ? false : navigations[0].Current.isIndepedent, navigations[1].Current == null ? false : navigations[1].Current.isIndepedent, false, false, true,
            () =>
            {
                titleTextStack.Pop();

                if (navigations[0].Current is WellnessBiosView)
                {
                    SetBackButton(false);
                }
                else
                {
                    SetBackButton(true);
                }

            },
            () =>
            {
                context.onClickBack += OnClickBack;
            }); ;
    }
    private void OnClickClose()
    {
        context.onClickClose -= OnClickClose;
        UIManager.Pop("", false, false, true, null, () =>
        {
            context.onClickClose += OnClickClose;
        });
    }
    public void ActiveDivider(bool isActive)
    {
        context.SetValue("IsActivePageDivider", isActive);
    }
}
