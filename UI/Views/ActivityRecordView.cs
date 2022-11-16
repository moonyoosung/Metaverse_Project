using kha2dev.DatePicker;
using MindPlus;
using MindPlus.Contexts.Master.Menus.ChallengeView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ActivityRecordView : UIView, ChallengeManager.IEventHandler
{
    public Transform content;
    private ChallengeManager.ActivityData data;
    private ActivityRecordViewContext context;
    private UIPool pool;
    private ChallengeManager challengeManager;
    private ImageContainer icons;
    private List<UIRecord> records = new List<UIRecord>();
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.context = new ActivityRecordViewContext();
        ContextHolder.Context = context;
        UIManager uimanager = uIManager as UIManager;
        this.pool = uimanager.GetPool(StringTable.UIRecordPool);
        this.challengeManager = persistent.ChallengeManager;
        this.icons = persistent.ResourceManager.ChallengeImages;
        challengeManager.ResistEvent(this);
    }
    public override void OnStartShow()
    {
        challengeManager.GetReward();
        base.OnStartShow();
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();
        data = null;
        foreach (var record in records)
        {
            record.InActivePool();
        }
        records.Clear();
    }
    public void Set(DateTime date)
    {
        foreach (var record in records)
        {
            record.InActivePool();
        }
        records.Clear();
        DateTime today = DateTime.Now;

        string dayText = "";
        if (date.Day == today.Day && date.Month == today.Month && date.Year == today.Year)
        {
            dayText += "Today, ";
        }

        dayText += date.ToString("MMMM dd", GameManager.Instance.CultureInfo);

        context.SetValue("DayText", dayText);

        List<ChallengeManager.Record> result = new List<ChallengeManager.Record>();

        foreach (var record in data.records)
        {
            DateTime recordTime = DateTime.Parse(record.createdTime);
            if (recordTime.Month == date.Month && recordTime.Day == date.Day)
            {
                result.Add(record);
            }
        }

        if (result.Count <= 0)
        {
            context.SetValue("IsActiveNone", true);
        }
        else
        {
            context.SetValue("IsActiveNone", false);
        }

        for (int i = 0; i < result.Count; i++)
        {
            UIRecord UIRecord = pool.Get<UIRecord>(content);
            string challengeId = challengeManager.GetChallengeId(result[i].activityId);
            string activityName = challengeManager.GetActivityName(result[i].activityId);
            Sprite icon = icons.Get(challengeId);
            bool isLast = i == result.Count - 1 ? false : true;
            Color textColor = challengeManager.GetColor(challengeManager.GetPart(result[i].activityId));
            ChallengeManager.Challenge data = challengeManager.ChallangeList.Get(challengeId);
            UIRecord.Set(result[i].activityId, icon, activityName, result[i].heart, result[i].coin, textColor, data, isLast);
            records.Add(UIRecord);
        }
    }
   


    public void OnGetChallenges(ChallengeManager.ChallengeList data)
    {
    }

    public void OnGetActivities(ChallengeManager.ActivityData data)
    {
        this.data = data;
        DayDot(Get<CallendarView>().calendar.listDayObject);
        Set(DateTime.Today);
    }
    public void DayDot(List<DayDateMonoScript> dayObjects)
    {
        foreach (var record in data.records)
        {
            foreach (var day in dayObjects)
            {
                DateTime recordTime = DateTime.Parse(record.createdTime);
                if (day.dateTime.Day == recordTime.Day && day.dateTime.Month == recordTime.Month)
                {
                    string part = challengeManager.GetPart(record.activityId);

                    day.OnDotImage(part);

                    break;
                }
            }
        }
    }
}
