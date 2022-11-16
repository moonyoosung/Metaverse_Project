using MindPlus.Contexts.Pool;
using Slash.Unity.DataBind.Core.Presentation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRecord : UIPoolObject
{
    public ContextHolder contextHolder;
    private UIRecordContext context;
    private ChallengeManager.Challenge data;
    private string activityId = "";
    public override void Initialize(Transform parent)
    {
        base.Initialize(parent);
        this.context = new UIRecordContext();
        contextHolder.Context = context;
    }
    public override void InActivePool()
    {
        context.onClickRecord -= OnClickRecord;
        data = null;
        activityId = "";
        base.InActivePool();
    }
    private void OnClickRecord()
    {
        context.onClickRecord -= OnClickRecord;
        ChallengeView challengeView = UIView.Get<ChallengeView>();
        ActivityMapView mapView = UIView.Get<ActivityMapView>();
        ChallengeDescriptionView descriptionView = UIView.Get<ChallengeDescriptionView>();

        descriptionView.Set(data.challengeId);
        mapView.Set(data);

        challengeView.SetBackButton(true, data.name);
        challengeView.Push("", false, false, null, new UIView[] { mapView }, new UIView[] { descriptionView });
    }
    public void Set(string activityId, Sprite icon, string title, int heart, int coin, Color textColor, ChallengeManager.Challenge data, bool enableLine = true)
    {
        this.activityId = activityId;
        context.SetValue("Icon", icon);
        context.SetValue("Title", title);
        context.SetValue("Heart", string.Format(Format.Money, heart));
        context.SetValue("Coin", string.Format(Format.Money, coin));
        context.SetValue("IsActiveLine", enableLine);
        context.SetValue("TitleTextColor", textColor);
        this.data = data;
        context.onClickRecord += OnClickRecord;
    }
}
