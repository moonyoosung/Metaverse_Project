using MindPlus.Contexts.Master.Menus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeView : UIMultiSplitView
{
    private ChallengeViewContext context;
    private Stack<string> titleTextStack = new Stack<string>();
    private LocalPlayerData playerData;
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        Sprite titleIcon = persistent.ResourceManager.ImageContainer.Get("menuchallenge");
        Sprite backIcon = persistent.ResourceManager.ImageContainer.Get("menubackarrow");
        this.context = new ChallengeViewContext(titleIcon, backIcon);
        this.playerData = persistent.AccountManager.PlayerData;
        ContextHolder.Context = context;
        context.onClickClose += OnClickClose;
        context.onClickBack += OnClickBack;
        titleTextStack.Push("Challenges");
        SetBackButton(false);
    }
    public override void OnStartShow()
    {
        context.SetValue("CoinText", playerData.coin == 0 ? "0" : string.Format(Format.Money, playerData.coin));
        context.SetValue("HeartText", playerData.heart == 0 ? "0" : string.Format(Format.Money, playerData.heart));
        base.OnStartShow();
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();
        titleTextStack.Clear();
        titleTextStack.Push("Challenges");
        SetBackButton(false);
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
        titleTextStack.Pop();

        if (navigations[0].history.Count <= 2)
        {
            SetBackButton(false);
        }
        else
        {
            SetBackButton(true);
        }


        Pop(navigations[0].Current.isIndepedent, navigations[1].Current.isIndepedent, false, false, true, null,
            () =>
            {
                context.onClickBack += OnClickBack;
            });
    }
    private void OnClickClose()
    {
        context.onClickClose -= OnClickClose;
        UIManager.Pop("", false, false, true, null, () => { context.onClickClose += OnClickClose; });
    }
}

