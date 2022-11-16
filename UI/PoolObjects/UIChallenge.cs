using MindPlus.Contexts.Pool;
using Slash.Unity.DataBind.Core.Presentation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIChallenge : UIPoolObject
{
    [HideInInspector]
    public UIChallengeContext context;
    public ContextHolder contextHolder;
    private ChallengeManager.Challenge data;
    private ChallengeManager.ActivityData activityData;
    public override void Initialize(Transform parent)
    {
        base.Initialize(parent);
        this.context = new UIChallengeContext();
        contextHolder.Context = context;

        context.onClickAssignment += OnClickAssignMent;
    }

    public override void InActivePool()
    {
        base.InActivePool();
        data = null;
        activityData = null;
    }
    public void Set(ChallengeManager.ActivityData activityData, ChallengeManager.Challenge data, Sprite icon = null)
    {
        this.data = data;
        this.activityData = activityData;
        context.SetValue("Name", data.name);
        if (icon)
        {
            context.SetValue("Icon", icon);
        }
        //bool hasHabits = data.habits != 0 ? true : false;
        context.SetValue("IsActiveLevel", true);
        context.SetValue("IsActiveProgress", true);
        context.SetValue("Level", string.Format("Lv. {0}", data.level));
        context.SetValue("Progress", string.Format("{0} %", Math.Round(data.progress * 100)));
    }
    private void OnClickAssignMent()
    {
        context.onClickAssignment -= OnClickAssignMent;
        ChallengeView challengeView = UIView.Get<ChallengeView>();
        ActivityMapView mapView = UIView.Get<ActivityMapView>();
        ChallengeDescriptionView descriptionView = UIView.Get<ChallengeDescriptionView>();

        descriptionView.Set(data.challengeId);
        mapView.Set(data);

        challengeView.SetBackButton(true, data.name);
        challengeView.Push("", false, false, () =>
        {
            context.onClickAssignment += OnClickAssignMent;
        }, new UIView[] { mapView }, new UIView[] { descriptionView });
    }
}
