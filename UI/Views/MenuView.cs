using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MindPlus.Contexts.Master;
using System;

public class MenuView : UIView
{
    private MenuViewContext context;
    private APIManager apiManager;
    private AccountManager accountManager;
    private PluginManager pluginManager;
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.apiManager = persistent.APIManager;
        this.accountManager = persistent.AccountManager;
        this.pluginManager = persistent.PluginManager;

        UIManager mainUiManager = uIManager as UIManager;
        this.context = new MenuViewContext();
        ContextHolder.Context = context;
        mainUiManager.MasterContext.MenuViewContext = context;

        context.onClickWorld += OnClickWorld;
        context.onClickPeople += OnClickPeoPle;
        context.onClickChallenge += OnClickChallenge;
        context.onClickWellness += OnClickWellness;
        context.onClickMyProfile += OnClickMyProfile;
        context.onClickSetting += OnClickSetting;

        context.SetValue("IsAlbe", false);
        context.SetValue("ThumbnailIcon", persistent.ResourceManager.ImageContainer.Get("loadingpeople"));
    }

    private void OnClickWellness()
    {
        context.onClickWellness -= OnClickWellness;
        WellnessView view = Get<WellnessView>();
#if UNITY_IOS && !UNITY_EDITOR
        if (!pluginManager.Handler.CheckPermission())
        {
            AllowHealthAppView allowHealthApp = UIView.Get<AllowHealthAppView>();
            view.PushNavigation(allowHealthApp);
        }
        else
        {
            WellnessBiosView leftView = Get<WellnessBiosView>();
            leftView.Set();
        }
#elif UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_STANDALONE_WIN
        AllowHealthAppView allowHealthApp = UIView.Get<AllowHealthAppView>();
        view.PushNavigation(allowHealthApp);
#endif

        UIManager.Push("", false, false, () => { context.onClickWellness += OnClickWellness; }, view);
    }

    public override void OnStartShow()
    {
        ThumbnailRequest();
        context.SetValue("NameText", accountManager.PlayerData.userName);
        context.SetValue("IdText", "@" + accountManager.PlayerData.userId);
        base.OnStartShow();
    }
    private void OnClickMyProfile()
    {
        context.onClickMyProfile -= OnClickMyProfile;

        ProfileView view = UIView.Get<ProfileView>();
        view.Set(accountManager.PlayerData.userId, true);
        UIManager.Push("", false, false, () => { context.onClickMyProfile += OnClickMyProfile; }, view);

    }
    private void OnClickChallenge()
    {
        context.onClickChallenge -= OnClickChallenge;

        ChallengeView view = Get<ChallengeView>();
        ChallCategorysView challCategorysView = Get<ChallCategorysView>();
        LevelCardView levelCardView = Get<LevelCardView>();

        view.PushNavigation(challCategorysView, levelCardView);

        UIManager.Push("", false, false, () => { context.onClickChallenge += OnClickChallenge; }, view);

    }
    private void OnClickPeoPle()
    {
        context.onClickPeople -= OnClickPeoPle;

        UIView view = UIView.Get<PeopleView>();
        UIManager.Push("", false, false, () => { context.onClickPeople += OnClickPeoPle; }, view);
    }
    private void OnClickWorld()
    {
        context.onClickWorld -= OnClickWorld;

        UIView view = UIView.Get<WorldView>();
        UIManager.Push("", false, false, () => { context.onClickWorld += OnClickWorld; }, view);
    }
    private void OnClickSetting()
    {
        context.onClickSetting -= OnClickSetting;

        SettingView view = UIView.Get<SettingView>();
        view.Set();
        UIManager.Push("", false, false, () => { context.onClickSetting += OnClickSetting; }, view);

    }
    private void ThumbnailRequest()
    {
        if (!string.IsNullOrEmpty(accountManager.PlayerData.userId))
        {
            string thumbnail = string.Format("users/{0}/{0}.jpg", accountManager.PlayerData.userId);

            apiManager.DownLoadTexture(thumbnail, (sprite) =>
            {
                context.SetValue("ThumbnailIcon", sprite);
            });
        }
    }
}
