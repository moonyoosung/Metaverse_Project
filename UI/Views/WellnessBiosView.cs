using BeliefEngine.HealthKit;
using MindPlus;
using MindPlus.Contexts.Master.Menus.WellnessView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class WellnessBiosView : UIView
{
    public RectTransform content;
    private WellnessBiosViewContext context;
    private UIPool bioDataPool;
    private List<UIBioData> pools = new List<UIBioData>();
    private PluginManager pluginManager;
    private HealthManager healthManager;
    private ChallengeManager challengeManager;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        UIManager uimanager = uIManager as UIManager;
        this.bioDataPool = uimanager.GetPool(StringTable.UIBioDataPool);
        this.context = new WellnessBiosViewContext();
        this.pluginManager = persistent.PluginManager;
        this.healthManager = persistent.HealthManager;
        challengeManager = persistent.ChallengeManager;
        ContextHolder.Context = context;
    }
    public override void OnStartHide()
    {
        foreach (var pool in pools)
        {
            pool.Hide();
        }
        base.OnStartHide();
    }
    public override void OnStartShow()
    {
        foreach (var pool in pools)
        {
            pool.Show();
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        base.OnStartShow();
    }
    public void Set()
    {
        (UIManager as UIManager).ActiveIndicator(true);
        StartCoroutine(Setting());
    }
    private IEnumerator Setting()
    {
#if UNITY_IOS && !UNITY_EDITOR && !UNITY_ANDROID
        DateTime today = DateTime.Now;
        DateTime weekOfFirstDay = today.FirstDayOfWeek();
        DateTime weekOfLastDay = today.LastDayOfWeek();

        context.SetValue("WeekText", string.Format("This week ({0} {1} - {2}, {3})",
            today.ToString("MMM", GameManager.Instance.CultureInfo),
            weekOfFirstDay.ToString("dd"),
            weekOfLastDay.ToString("dd"),
            today.ToString("yyyy", GameManager.Instance.CultureInfo)));
        IOSPluginHandler iOSPlugin = (pluginManager.Handler as IOSPluginHandler);
        List<HKDataType> types = iOSPlugin.GetReadDataType();
        List<bool> completeChecker = new List<bool>();
        for (int i = 0; i < types.Count; i++)
        {
            completeChecker.Add(false);
            iOSPlugin.ReadData(types[i], weekOfLastDay, (weekOfLastDay - weekOfFirstDay).TotalDays, (result, dataType) =>
            {
                Debug.Log(dataType.ToString() + "Callback ReadHealthKitData================");
                float dataValue = 0f;
                string challCategory = healthManager.GetChallengeCategory(dataType);
                bool isInterger = healthManager.IsIntegerUnit(dataType);
                if (result.Count > 0)
                {
                    List<HealthManager.Record> records = healthManager.HealthStringSerialize(dataType, result, out DateTime start, out DateTime end);
                    float sum = 0f;
                    foreach (var record in records)
                    {
                        sum += float.Parse(record.count);
                    }
                    if (healthManager.IsAverageType(dataType))
                    {
                        dataValue = sum / records.Count;
                    }
                    else
                    {
                        dataValue = sum;
                    }
                    UIBioData bioData = bioDataPool.Get<UIBioData>(content);
                    bioData.Set(dataType, iOSPlugin.GetDataTitleName(dataType), dataValue, healthManager.GetUnit(dataType), challengeManager.GetColor(challCategory), isInterger);
                    pools.Add(bioData);
                }
                else
                {
                    UIBioData bioData = bioDataPool.Get<UIBioData>(content);
                    bioData.Set(dataType, iOSPlugin.GetDataTitleName(dataType), dataValue, healthManager.GetUnit(dataType), challengeManager.GetColor(challCategory), isInterger);
                    pools.Add(bioData);
                }
                completeChecker[i] = true;
                Debug.Log("==============" + dataType.ToString() + " IsComplete : " + completeChecker[i].ToString() + " typesCount : " + types.Count);
            });

            while (!completeChecker[i])
            {
                yield return null;
            }
            Debug.Log("=====================> "+types[i].ToString() + " Complete : " + completeChecker[i].ToString());
        }
#endif
#if UNITY_EDITOR
        yield return null;
        UIBioData uIBioData = bioDataPool.Get<UIBioData>(content);
        uIBioData.Set(HKDataType.HKQuantityTypeIdentifierStepCount, "Steps", 1.2345f, healthManager.GetUnit(HKDataType.HKQuantityTypeIdentifierStepCount), new Color(0.5f, 0.5f, 0.5f, 0.5f), true);
        UIBioData uIBioData2 = bioDataPool.Get<UIBioData>(content);
        uIBioData2.Set(HKDataType.HKQuantityTypeIdentifierHeartRate, "Heart Rate", 1.00f, healthManager.GetUnit(HKDataType.HKQuantityTypeIdentifierHeartRate), new Color(0.5f, 0.5f, 0.5f, 0.5f), false);
        UIBioData uIBioData3 = bioDataPool.Get<UIBioData>(content);
        uIBioData3.Set(HKDataType.HKQuantityTypeIdentifierDistanceWalkingRunning, "DistanceWalkingRunning", 0.5f, healthManager.GetUnit(HKDataType.HKQuantityTypeIdentifierDistanceWalkingRunning), new Color(0.5f, 0.5f, 0.5f, 0.5f), false);
        pools.Add(uIBioData);
        pools.Add(uIBioData2);
        pools.Add(uIBioData3);
#endif

        WellnessView wellnessView = Get<WellnessView>();
        ArticlesView articlesView = Get<ArticlesView>();


        wellnessView.Push("", false, false, null, new UIView[] { this }, new UIView[] { articlesView });
        (UIManager as UIManager).ActiveIndicator(false);

        healthManager.systemTime = DateTime.Now.AddMinutes(10);
    }
    public void ClearData()
    {
        foreach (var pool in pools)
        {
            pool.InActivePool();
        }
        pools.Clear();
    }
}
