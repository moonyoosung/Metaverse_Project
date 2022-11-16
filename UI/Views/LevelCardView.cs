using MindPlus.Contexts.Master.Menus.ChallengeView;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class LevelCardView : UIView, ChallengeManager.IEventHandler
{
    [TitleGroup("[Option]")]
    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    public Gradient2 gradient;
    private LevelCardViewContext context;
    private ChallengeManager challengeManager;
    private UnityEngine.Gradient[] gradients;
    private Sprite[] levelIcons;
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.challengeManager = persistent.ChallengeManager;
        UIResources uIResources = persistent.ResourceManager.GetUIResources();
        this.gradients = uIResources.levelGradients;
        this.levelIcons = uIResources.levelIcons;
        challengeManager.ResistEvent(this);

        this.context = new LevelCardViewContext();
        ContextHolder.Context = context;

        context.onClickProgress += OnClickProgress;
    }



    public void OnGetChallenges(ChallengeManager.ChallengeList data)
    {
        context.SetValue("OverallProgress", string.Format("{0:0.##}", data.overallProgress));

        gradient.EffectGradient = gradients[data.userClass];
        context.SetValue("LevelText", data.userClass.ToString());
        context.SetValue("TierIcon", data.userClass < 5 ? levelIcons[data.userClass] : levelIcons[0]);

        int userTotalLevel = data.outdoorLevel + data.inappLevel + data.healthLevel;

        context.SetValue("InfoText", GetInfo(data.userClass, userTotalLevel));
        context.SetValue("OutdoorLevel", data.outdoorLevel.ToString());
        context.SetValue("InappLevel", data.inappLevel.ToString());
        context.SetValue("HealthLevel", data.healthLevel.ToString());
        context.SetValue("TotalLevel", userTotalLevel.ToString());
    }
    public static string GetTier(int level)
    {
        switch (level)
        {
            case 0: return "Bronze";
            case 1: return "Silver";
            case 2: return "Gold";
            case 3: return "Platinum";
            case 4: return "Diamond";
            default: return "UnLank";
        }
    }
    private string GetInfo(int classTier, int currentLevel)
    {
        int minimum = 0;
        int remainLevel = 0;
        float progress = 0f;
        switch (classTier)
        {
            case 0:
                minimum = 1;
                break;
            case 1:
                minimum = 3;
                break;
            case 2:
                minimum = 6;
                break;
            case 3:
                minimum = 10;
                break;
            case 4:
                break;
            default:
                Debug.LogError("Not Set LevelCard Info " + currentLevel);
                break;
        }

        remainLevel = minimum - currentLevel;
        progress = currentLevel / minimum;
        context.SetValue("LevelProgress", progress);
        if (classTier + 1 >= 4)
        {
            return "You have achieved the highest tier";
        }
        else
        {
            return string.Format("{0} level(s) away from {1} Class", remainLevel, GetTier(classTier + 1));
        }
    }
    public void ResistLevelCardButton()
    {
        if (context.onClickLevelCard != OnClickLevelCard)
        {
            context.onClickLevelCard += OnClickLevelCard;
        }
    }
    private void OnClickLevelCard()
    {
        context.onClickLevelCard -= OnClickLevelCard;
        ClassListView classListView = Get<ClassListView>();
        ChallengeView challengeView = Get<ChallengeView>();

        challengeView.SetBackButton(true, "Classes");

        challengeView.Push("", false, false, null, new UIView[] { classListView }, null);
    }
    private void OnClickProgress()
    {
        context.onClickProgress -= OnClickProgress;

        ChallengeView challengeView = Get<ChallengeView>();
        CallendarView view = Get<CallendarView>();
        ActivityRecordView achieveList = Get<ActivityRecordView>();
        //view.Set(() =>
        //{

        //});
        challengeView.SetBackButton(true, "Activity Overview");

        challengeView.Push("", false, false, () => { context.onClickProgress += OnClickProgress; }, new UIView[] { view }, new UIView[] { achieveList });
    }

    public void OnGetActivities(ChallengeManager.ActivityData data)
    {
    }
}
