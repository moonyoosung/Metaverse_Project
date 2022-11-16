using MindPlus.Contexts.Master.Menus.ChallengeView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ActivityMapView : UIView
{
    public UIMapToggle[] mapToggles;
    public Toggle[] lines;
    private ActivityMapViewContext context;
    private ImageContainer icons;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.context = new ActivityMapViewContext();
        ContextHolder.Context = context;
        this.icons = persistent.ResourceManager.ChallengeImages;

        foreach (var toggle in mapToggles)
        {
            toggle.Initialize();
        }
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();
        foreach (var toggle in mapToggles)
        {
            toggle.toggle.isOn = false;
        }
        foreach (var line in lines)
        {
            line.isOn = false;
        }
    }
    public void Set(ChallengeManager.Challenge data)
    {
        context.SetValue("Level", string.Format("Lv. {0}", data.level.ToString()));
        Sprite icon = icons.Get(data.challengeId);
        context.SetValue("Icon", icon);
        if(data.startDate == "" || data.startDate == null)
        {
            context.SetValue("DDay", "");
        }
        else
        {
            DateTime start = Convert.ToDateTime(data.startDate);
            DateTime end = Convert.ToDateTime(data.endDate);

            int dDay = (end - start).Days;
            //D-19 (7/26~8/26)
            context.SetValue("DDay", string.Format("D-{0} ({1}~{2})", dDay, start.ToString("MM/dd"), end.ToString("MM.dd")));
        }


        for (int i = 0; i < data.records.Length; i++)
        {
            DateTime date = Convert.ToDateTime(data.records[i]);
            mapToggles[i].dateText.text = date.ToString("MM.dd", CultureInfo.InvariantCulture);
            mapToggles[i].toggle.isOn = true;
            if (i >= 1)
            {
                lines[i - 1].isOn = true;
            }
        }
    }
}
