using BeliefEngine.HealthKit;
using MindPlus.Contexts.Pool;
using Slash.Unity.DataBind.Core.Presentation;
using UnityEngine;
using MindPlus;

public class UIBioData : UIPoolObject
{
    public ContextHolder ContextHolder;
    private UIBioDataContext context;
    private HKDataType hKDataType;
    public override void Initialize(Transform parent)
    {
        base.Initialize(parent);

        this.context = new UIBioDataContext();
        ContextHolder.Context = context;
    }
    public void Hide()
    {
        if (context.onClickDetail == OnClickDetail)
            context.onClickDetail -= OnClickDetail;
    }
    public void Show()
    {
        if (context.onClickDetail != OnClickDetail)
            context.onClickDetail += OnClickDetail;
    }
    public void Set(HKDataType hKDataType, string healthId, float dataValue, string unit, Color color, bool isInteger)
    {
        this.hKDataType = hKDataType;
        context.SetValue("DataTextColor", color);
        context.SetValue("DotIconColor", color);
        SetID(healthId);
        context.SetValue("TitleText", healthId);
        Debug.Log("==============BioDataValue : " + dataValue.ToString() + "==============="+ isInteger);
        if (isInteger)
        {
            context.SetValue("DateText", dataValue == 0f ? "--" : dataValue.ToString());
        }
        else
        {
            context.SetValue("DateText", dataValue == 0f ? "--" : dataValue < 1 ? string.Format("{0:0.###}", dataValue) : string.Format("{0:#.###}", dataValue));
        }
        context.SetValue("UnitText", unit);
    }
    private void OnClickDetail()
    {
        context.onClickDetail -= OnClickDetail;
        WellnessView wellnessView = UIView.Get<WellnessView>();
        BioDataDetailView bioDataDetailView = UIView.Get<BioDataDetailView>();
        BioActivityListView bioActivityListView = UIView.Get<BioActivityListView>();
        bioDataDetailView.StartCoroutine(bioDataDetailView.Set(hKDataType, 0, () =>
        {
            Debug.Log("======================OnCompleteSet=================");
            wellnessView.SetBackButton(true, context.GetValue("TitleText").ToString());
            wellnessView.Push("", false, false, null, new UIView[] { bioDataDetailView }, new UIView[] { bioActivityListView });
        }));
    }
    public override void InActivePool()
    {
        base.InActivePool();
        if (context.onClickDetail == OnClickDetail)
        {
            context.onClickDetail -= OnClickDetail;
        }
    }
}
