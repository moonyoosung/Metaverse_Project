using kha2dev.DatePicker;
using MindPlus.Contexts.Master.Menus.ChallengeView;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CallendarView : UIView
{
    [TitleGroup("[Option]")]
    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    public CalendarDatePicker calendar;
    private CallendarViewContext context;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.context = new CallendarViewContext();
        ContextHolder.Context = context;
        calendar.Initialize();
        context.onClickToday += OnClickToday;
    }
    public void Set(Action onSetComplete = null)
    {
        //calendar.Show(OnClickDate, onSetComplete, OnClickCallendarButton, OnClickCallendarButton);
    }
    private void OnClickCallendarButton()
    {
        Get<ActivityRecordView>().DayDot(calendar.listDayObject);
    }

    private void OnClickDate(DateTime date)
    {
        Get<ActivityRecordView>().Set(date);
    }
    

    public override void OnFinishHide()
    {
        base.OnFinishHide();
        calendar.Hide();
    }
    public override void OnStartShow()
    {
        calendar.Show(OnClickDate, null, OnClickCallendarButton, OnClickCallendarButton);
        base.OnStartShow();
    }
    private void OnClickToday()
    {
        context.onClickToday -= OnClickToday;

        calendar.Show(OnClickDate, ()=> 
        {
            ActivityRecordView view = Get<ActivityRecordView>();
            view.Set(DateTime.Now);
            view.DayDot(calendar.listDayObject);
            context.onClickToday += OnClickToday;
        }, OnClickCallendarButton, OnClickCallendarButton);
    }
}
