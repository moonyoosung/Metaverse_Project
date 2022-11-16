using BeliefEngine.HealthKit;
using ChartAndGraph;
using MindPlus;
using MindPlus.Contexts.Master.Menus.WellnessView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using static ChartAndGraph.GraphData;

public class BioDataDetailView : UIView
{

    public ToggleGroup toggleGroup;
    public RectTransform graphPanel;
    public RectTransform content;
    public BarChart Bar;
    public GraphChart chart;
    public VerticalAxis verticalAxis;
    public UIDayToggle[] uIDayToggles;
    private BioDataDetailViewContext context;
    private PluginManager pluginManager;
    private HealthManager healthManager;
    private ImageContainer challGraphImages;
    private ChallengeManager challengeManager;
    private DateTime today;
    private HKDataType type = HKDataType.NUM_TYPES;
    private Dictionary<string, List<HealthManager.Record>> dayofWeeksDatas = new Dictionary<string, List<HealthManager.Record>>();
    private UIPool bioPointpool;
    private List<UIBioPoint> points = new List<UIBioPoint>();
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.context = new BioDataDetailViewContext();
        ContextHolder.Context = context;
        this.pluginManager = persistent.PluginManager;
        this.healthManager = persistent.HealthManager;
        this.challGraphImages = persistent.ResourceManager.ChallGrpahImages;
        this.challengeManager = persistent.ChallengeManager;
        this.bioPointpool = (uIManager as UIManager).GetPool(StringTable.UIBioPointPool);
        context.onClickSetData += OnClickSetData;
        foreach (var dayToggle in uIDayToggles)
        {
            dayToggle.onToggleChanged += OnToggleChanged;
        }
    }
    public override void OnStartShow()
    {
        content.anchoredPosition = Vector2.zero;
        base.OnStartShow();
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();
        toggleGroup.SetAllTogglesOff(false);
    }
    private void OnToggleChanged(int dateNum, bool isOn)
    {

        if (!isOn)
        {
            if (!toggleGroup.AnyTogglesOn())
            {
                DateTime weekOfFirstDay = today.FirstDayOfWeek();
                DateTime weekOfLastDay = today.LastDayOfWeek();

                context.SetValue("ThisWeekText", string.Format("{0} {1} - {2} {3}, {4}",
                    weekOfFirstDay.ToString("MMM", GameManager.Instance.CultureInfo),
                    weekOfFirstDay.ToString("dd"),
                    weekOfLastDay.ToString("MMM", GameManager.Instance.CultureInfo),
                    weekOfLastDay.ToString("dd"),
                    today.ToString("yyyy", GameManager.Instance.CultureInfo)));
                context.SetValue("Date", "This Week");


                List<HealthManager.Record> allDatas = new List<HealthManager.Record>();
                foreach (var days in dayofWeeksDatas)
                {
                    foreach (var dayData in days.Value)
                    {
                        allDatas.Add(dayData);
                    }
                }
                ReadDayOfWeekData(allDatas);
            }
            return;
        }

        DateTime firstDayofWeek = today.FirstDayOfWeek();
        DateTime targetDay = firstDayofWeek.AddDays(dateNum);
        Get<BioActivityListView>().OffToggles();
        OffPoints();

        context.SetValue("Date", string.Format("{0} {1}, {2}", targetDay.ToString("MMM", GameManager.Instance.CultureInfo), targetDay.ToString("dd"), targetDay.ToString("yyyy")));
        ReadDayOfWeekData(dayofWeeksDatas[targetDay.ToString("ddd", GameManager.Instance.CultureInfo)]);
    }
    public void OffPoints()
    {
        foreach (var point in points)
        {
            point.DisableImage();
        }
    }
    private void OnClickSetData(int direction)
    {
        context.onClickSetData -= OnClickSetData;
        toggleGroup.SetAllTogglesOff(false);
        Get<BioActivityListView>().OffToggles();
        OffPoints();
        StartCoroutine(Set(type, direction, () => { context.onClickSetData += OnClickSetData; }));
    }

    // Use this for initialization
    public IEnumerator Set(HKDataType type, int direction = 0, Action onComplete = null)
    {
        Debug.Log("======================OnStartSet=================");

        this.type = type;
        (UIManager as UIManager)?.ActiveIndicator(true);

        HealthActivityPair.Pair pair = healthManager.HealthActivityPair.GetPair(type);
        bool isActiveDescription = pair.title == "" ? false : true;
        context.SetValue("IsActiveDescription", isActiveDescription);
        if (isActiveDescription)
        {
            context.SetValue("DescriptionTitle", pair.title);
            context.SetValue("Description", pair.description);
        }

        List<HealthManager.Record> records = new List<HealthManager.Record>();
#if UNITY_IOS && UNITY_EDITOR
        HealthManager.Record[] mock = new HealthManager.Record[] {
            new HealthManager.Record { count = "3110", time = "2022-09-22T17:26:30", unit = healthManager.GetUnit(type)},
            //new HealthManager.Record { count = "5000", time = "2022-09-21T15:26:30", unit = healthManager.GetUnit(type)},
            //new HealthManager.Record { count = "99", time = "2022-09-20T16:26:30", unit = healthManager.GetUnit(type)},
            //new HealthManager.Record { count = "72", time = "2022-09-14T18:26:30", unit = healthManager.GetUnit(type)},
            //new HealthManager.Record { count = "74", time = "2022-09-15T20:26:30", unit = healthManager.GetUnit(type)},
            //new HealthManager.Record { count = "70", time = "2022-09-16T22:26:30", unit = healthManager.GetUnit(type)},
            //new HealthManager.Record { count = "66", time = "2022-09-17T22:26:30", unit = healthManager.GetUnit(type)},

        };
        foreach (var mockData in mock)
        {
            records.Add(mockData);
        }
#endif
        bool isReading = true;

#if UNITY_IOS && !UNITY_EDITOR
        ReadThisWeek(direction, out DateTime weekOfFirstDay, out DateTime weekOfLastDay);

        IOSPluginHandler iOSPlugin = (pluginManager.Handler as IOSPluginHandler);
        iOSPlugin.ReadData(this.type, weekOfLastDay, (weekOfLastDay - weekOfFirstDay).TotalDays, (result, datatype) =>
        {
            isReading = false;
            this.type = datatype;
           
            if (result.Count > 0)
            {
                records = healthManager.HealthStringSerialize(datatype, result, out DateTime start, out DateTime end);
            }
        });

#else
        ReadThisWeek(direction, out DateTime weekOfFirstDay, out DateTime weekOfLastDay);
        isReading = false;
#endif
        while (isReading)
        {
            yield return null;
        }
        yield return FillGraph(this.type, records);

        (UIManager as UIManager)?.ActiveIndicator(false);
        onComplete?.Invoke();
    }

    private void ReadThisWeek(int direction, out DateTime weekOfFirstDay, out DateTime weekOfLastDay)
    {
        if (direction == 0)
        {
            today = DateTime.Now;
        }
        else if (direction == -1)
        {
            today = today.AddDays(-7);
        }
        else
        {
            today = today.AddDays(+7);
        }

        weekOfFirstDay = today.FirstDayOfWeek();
        weekOfLastDay = today.LastDayOfWeek();

        context.SetValue("ThisWeekText", string.Format("{0} {1} - {2} {3}, {4}",
            weekOfFirstDay.ToString("MMM", GameManager.Instance.CultureInfo),
            weekOfFirstDay.ToString("dd"),
            weekOfLastDay.ToString("MMM", GameManager.Instance.CultureInfo),
            weekOfLastDay.ToString("dd"),
            today.ToString("yyyy", GameManager.Instance.CultureInfo)));
        context.SetValue("Date", "This Week");

    }

    /// <summary>
    /// This method waits for the bar chart to fill up before filling the graph
    /// </summary>
    /// <returns></returns>
    private IEnumerator FillGraph(HKDataType type, List<HealthManager.Record> records)
    {
        foreach (var point in points)
        {
            point.InActivePool();
        }
        points.Clear();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        float minCount = float.MaxValue;
        float maxCount = float.MinValue;
        float weekTotal = 0;
        DateTime weekLastTime = new DateTime();
        float weekLastValue = 0;
        float weekAverage = 0;

        dayofWeeksDatas.Clear();

        for (int i = 0; i < Bar.DataSource.TotalCategories; i++)
        {
            string categoryName = Bar.DataSource.GetCategoryName(i);
            dayofWeeksDatas.Add(categoryName, new List<HealthManager.Record>());
        }

        for (int i = 0; i < records.Count; i++)
        {
            DateTime day = DateTime.ParseExact(records[i].time, Format.DateFormat, GameManager.Instance.CultureInfo);
            SortDayOfWeek(day, records[i]);
            float count = float.Parse(records[i].count);
            weekTotal += count;

            if (weekLastTime <= day)
            {
                weekLastValue = count;
                weekLastTime = day;
            }
            if(minValue > count)
            {
                minValue = count;
            }
            if(maxValue < count)
            {
                maxValue = count;
            }
            if(minCount > count)
            {
                minCount = count;
            }
            if(maxCount < count)
            {
                maxCount = count;
            }
        }

        if (records.Count > 0)
        {
            if (healthManager.IsAverageType(type))
            {
                weekAverage = (float)Util.IntRound((weekTotal / records.Count), 3);
            }
            else
            {
                weekAverage = weekTotal;
            }
        }
        if (minValue > weekAverage)
        {
            minValue = weekAverage;
        }
        if (maxValue < weekAverage)
        {
            maxValue = weekAverage;
        }
        if (minValue == int.MaxValue)
        {
            minValue = 0;
        }
        if (maxValue == int.MinValue)
        {
            maxValue = 100;
        }
        if (minValue == maxValue)
        {
            minValue = 0;
        }
        if(minCount == int.MaxValue)
        {
            minCount = 0;
        }
        if(maxCount == int.MinValue)
        {
            maxCount = 0;
        }

        string unit = healthManager.GetUnit(type);
        context.SetValue("DataUnit", unit);
        context.SetValue("GraphUnit", string.Format("({0})", unit));
        context.SetValue("AverageText", healthManager.GetAverageText(type));

        SetDataText(weekAverage, minCount, maxCount, weekLastValue, weekLastTime, unit);

        Bar.DataSource.ClearValues();
        chart.DataSource.ClearCategory("Category1");

        SetChartVerticalView(minValue, maxValue);

        Debug.Log("====================================SetGraph=================================");
        for (int i = 0; i < Bar.DataSource.TotalCategories; i++)
        {
            string categoryName = Bar.DataSource.GetCategoryName(i);

            if (dayofWeeksDatas[categoryName].Count <= 0)
            {
                continue;
            }
            float dayTotal = 0f;
            foreach (var record in dayofWeeksDatas[categoryName])
            {
                dayTotal += float.Parse(record.count);
            }


            float dayAverage = 0f;
            if (healthManager.IsAverageType(type))
            {
                dayAverage = (float)Util.IntRound((dayTotal / dayofWeeksDatas[categoryName].Count));
            }
            else
            {
                dayAverage = dayTotal;
            }
            for (int j = 0; j < Bar.DataSource.TotalGroups; j++)
            {
                string groupName = Bar.DataSource.GetGroupName(j);
                Vector3 position;
                Bar.GetBarTrackPosition(categoryName, groupName, out position); // find the position of the top of the bar chart

                UIBioPoint uIBioPoint = bioPointpool.Get<UIBioPoint>(chart.transform);
                double x, y;
                chart.PointToClient(position, out x, out y); // convert it to graph coordinates
                float lerpX = (float)x * 0.1f;
                double verticalSize = chart.DataSource.VerticalViewSize;
                List<string> healthIDs = new List<string>();

                DateTime day = DateTime.ParseExact(dayofWeeksDatas[categoryName][0].time, Format.DateFormat, GameManager.Instance.CultureInfo);
                foreach (var challengeID in challengeManager.GetChallengeIds(day))
                {
                    if (!healthIDs.Contains(challengeID))
                    {
                        healthIDs.Add(challengeID);
                    }
                }
                uIBioPoint.Set(new Vector2(graphPanel.sizeDelta.x * lerpX,
                    (float)((dayAverage - chart.DataSource.VerticalViewOrigin) / verticalSize * graphPanel.sizeDelta.y) + 12f),
                    healthIDs,
                    challGraphImages);

                points.Add(uIBioPoint);

                chart.DataSource.AddPointToCategory("Category1", x, dayAverage); // drop the y value and set your own value
                Debug.Log(categoryName + "\t Day Average : " + dayAverage.ToString());
            }
        }
    }

    private void SetChartVerticalView(float minValue, float maxValue)
    {
        if (!healthManager.IsIntegerUnit(type))
        {
            verticalAxis.MainDivisions.FractionDigits = 3;
            chart.DataSource.VerticalViewOrigin = minValue == 0f ? 0f : minValue * 0.9f;
            if (chart.DataSource.VerticalViewOrigin > minValue)
            {
                chart.DataSource.VerticalViewOrigin = minValue;
            }
            chart.DataSource.VerticalViewSize = minValue - chart.DataSource.VerticalViewOrigin + (maxValue - minValue) * 1.1f;
        }
        else
        {
            verticalAxis.MainDivisions.FractionDigits = 0;
            chart.DataSource.VerticalViewOrigin = minValue == 0f ? 0f : minValue * 0.9f;
            if (chart.DataSource.VerticalViewOrigin > minValue)
            {
                chart.DataSource.VerticalViewOrigin = minValue;
            }
            chart.DataSource.VerticalViewSize = minValue - chart.DataSource.VerticalViewOrigin + (maxValue - minValue) * 1.1f;
        }

    }

    private void SetDataText(float average, float minValue, float maxValue, float lastValue, DateTime lastTime, string unit)
    {
        if (!healthManager.IsIntegerUnit(type))
        {
            context.SetValue("AverageValue", average == 0f ? "--" : average < 1 ? string.Format("<b>{0:0.#####}</b>", average) : string.Format("<b>{0:#.#####}</b>", average));

            if (minValue < 1 || maxValue < 1)
            {
                context.SetValue("RangeValue",
                string.Format("<b>{0:0.###} - {1:0.###}</b> {2}",
                minValue == float.MaxValue ? "--" : minValue == 0f ? "0" : minValue,
                maxValue == float.MinValue ? "--" : maxValue == 0f ? "0" : maxValue, unit));
            }
            else
            {
                context.SetValue("RangeValue",
                string.Format("<b>{0:#.###} - {1:#.###}</b> {2}",
                minValue == float.MaxValue ? "--" : minValue == 0f ? "0" : minValue,
                maxValue == float.MinValue ? "--" : maxValue == 0f ? "0" : maxValue, unit));
            }

            context.SetValue("LastTime", string.Format("Latest: {0}", lastTime.ToString("hh:mm tt", GameManager.Instance.CultureInfo)));
            context.SetValue("LastValue", lastValue == 0f ? "--" :
                lastValue < 1 ? string.Format("<b>{0:0.###}</b> {1}", lastValue, unit) : string.Format("<b>{0:#.###}</b> {1}", lastValue, unit));
        }
        else
        {
            int convertAverage = (int)average;
            int convertMinValue = (int)minValue;
            int convertMaxValue = (int)maxValue;
            int convertlastValue = (int)lastValue;
            context.SetValue("AverageValue", convertAverage == 0f ? "--" : string.Format("<b>{0}</b>", convertAverage));
            context.SetValue("RangeValue",
                string.Format("<b>{0} - {1}</b> {2}",
                minValue == float.MaxValue ? "--" : convertMinValue,
                maxValue == float.MinValue ? "--" : convertMaxValue, unit));
            context.SetValue("LastTime", string.Format("Latest: {0}", lastTime.ToString("hh:mm tt", GameManager.Instance.CultureInfo)));
            context.SetValue("LastValue", lastValue == 0f ? "--" : string.Format("<b>{0}</b> {1}", (int)convertlastValue, unit));
        }
    }

    private void ReadDayOfWeekData(List<HealthManager.Record> records)
    {
        if (records.Count > 0)
        {
            float average = 0f;
            float total = 0f;
            DateTime lastTime = new DateTime();
            float lastValue = 0f;
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            foreach (var record in records)
            {
                float count = float.Parse(record.count);
                DateTime recordTime = DateTime.ParseExact(record.time, Format.DateFormat, GameManager.Instance.CultureInfo);
                total += count;
                if (minValue > count)
                {
                    minValue = count;
                }
                if (maxValue < count)
                {
                    maxValue = count;
                }
                if (lastTime <= recordTime)
                {
                    lastValue = count;
                    lastTime = recordTime;
                }
            }
            if (healthManager.IsAverageType(this.type))
            {
                average = (float)Util.IntRound((total / records.Count));
            }
            else
            {
                average = total;
            }
            string unit = healthManager.GetUnit(type);
            SetDataText(average, minValue, maxValue, lastValue, lastTime, unit);
        }
        else
        {
            context.SetValue("AverageValue", "--");
            context.SetValue("RangeValue", "--");
            context.SetValue("LastTime", "--");
            context.SetValue("LastValue", "--");
        }
    }

    private void SortDayOfWeek(DateTime day, HealthManager.Record record)
    {
        string date = day.ToString("ddd", GameManager.Instance.CultureInfo);
        foreach (var dayofWeek in dayofWeeksDatas)
        {
            if (dayofWeek.Key == date)
            {
                dayofWeek.Value.Add(record);
                return;
            }
        }
        Debug.LogError("Not Match Date");
    }

    public void ChartPointOn(string challengeID)
    {
        foreach (var point in points)
        {
            point.CheckPoint(challengeID);
        }
    }
}
