using MindPlus.Contexts.Master.Menus.WellnessView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class BioActivityListView : UIView, ChallengeManager.IEventHandler
{
    public ToggleGroup toggleGroup;
    public RectTransform content;
    private BioActivityListViewContext context;
    private UIPool bioPool;
    private List<UIBioChallenge> pools = new List<UIBioChallenge>();
    private HealthManager healthManager;
    private ChallengeManager challengeManager;
    private ImageContainer icons;
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.context = new BioActivityListViewContext();
        ContextHolder.Context = context;
        this.bioPool = (uIManager as UIManager).GetPool(StringTable.UIBioActivityPool);
        this.healthManager = persistent.HealthManager;
        this.challengeManager = persistent.ChallengeManager;
        challengeManager.ResistEvent(this);
        this.icons = persistent.ResourceManager.ChallengeImages;
    }

    public void OnGetActivities(ChallengeManager.ActivityData data)
    {
        List<ChallengeManager.Record> result = new List<ChallengeManager.Record>();

        foreach (var record in data.records)
        {
            result.Add(record);
        }

        Set(result);
    }

    public void OnGetChallenges(ChallengeManager.ChallengeList data)
    {
    }

    public override void OnStartShow()
    {
        challengeManager.GetReward();
        base.OnStartShow();
    }
    private void Set(List<ChallengeManager.Record> records)
    {
        foreach (var pool in pools)
        {
            pool.InActivePool();
        }
        pools.Clear();
        context.SetValue("Day", "View Your activity");
        List<ChallengeManager.Record> target = new List<ChallengeManager.Record>();
        if (records.Count < 0)
        {
            foreach (var record in challengeManager.activityData.records)
            {
                target.Add(record);
            }
        }
        else
        {
            target = records;
        }
        List<string> overLaps = new List<string>();

        foreach (var record in target)
        {
            bool isOverlap = false;
            string challengeId = challengeManager.GetChallengeId(record.activityId);
            foreach (var overlap in overLaps)
            {
                if (overlap == challengeId)
                {
                    isOverlap = true;
                    break;
                }
            }

            if (isOverlap)
            {
                continue;
            }
            overLaps.Add(challengeId);
            UIBioChallenge uIBioActivity = bioPool.Get<UIBioChallenge>(content);
            string challengeName = challengeManager.GetChallengeName(challengeId);
            Sprite icon = icons.Get(challengeId);
            uIBioActivity.Set(challengeName, icon, challengeId, toggleGroup);
            pools.Add(uIBioActivity);
        }

        if (overLaps.Count <= 0)
        {
            UIBioChallenge uIBioActivity = bioPool.Get<UIBioChallenge>(content);
            uIBioActivity.Set("No activities yet today", null);
            pools.Add(uIBioActivity);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }
    public void OffToggles()
    {
        toggleGroup.SetAllTogglesOff();
    }

    public void PoolClear()
    {
        foreach (var pool in pools)
        {
            pool.InActivePool();
        }
        pools.Clear();
    }
}
