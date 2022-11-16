using BeliefEngine.HealthKit;
using MindPlus.Contexts.Master.Menus.WellnessView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class AllowHealthAppView : UIView
{
    private AllowHealthAppViewContext context;
    private PluginManager pluginManager;
    private HealthManager healthManager;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.context = new AllowHealthAppViewContext();
        ContextHolder.Context = context;
        this.pluginManager = persistent.PluginManager;
        this.healthManager = persistent.HealthManager;
        context.onClickAllowall += OnClickAllowall;
    }

    //???? ???????? ???????? ???? ??????(Permission?? ???????? Type)
    //10?? ???????? ???? ???? ???? 1?? ?? ?????? ??????
    private void OnClickAllowall()
    {
        context.onClickAllowall -= OnClickAllowall;
#if UNITY_IOS && !UNITY_EDITOR
        pluginManager.Handler.Authorize((result) =>
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Debug.Log("=================Authorize Result : " + result + "=================");
            if (result)
            {
                Get<WellnessBiosView>().Set();
            }
            else
            {
                IOSPluginHandler.ShowAlert("Authorize", "HealthKit App Not Permited");
                context.onClickAllowall += OnClickAllowall;
            }
            pluginManager.Handler.SetAuthorized(true);
        });
#elif UNITY_EDITOR
        WellnessBiosView leftView = Get<WellnessBiosView>();
        leftView.Set();
#endif

    }
}