using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArticleDescriptionView : UIView
{
    public RectTransform content;
    private ArticleDescriptions articleDescriptions;
    private UIPool subScriptPool;
    private UIPool panelScriptPool;
    private UIPool addScriptPool;
    private List<UIScriptFrame> pools = new List<UIScriptFrame>();
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.articleDescriptions = persistent.ResourceManager.ArticleDescriptions;
        UIManager UIManager = uIManager as UIManager;
        this.subScriptPool = UIManager.GetPool(StringTable.UIScriptSubPool);
        this.panelScriptPool = UIManager.GetPool(StringTable.UIScriptPanelPool);
        this.addScriptPool = UIManager.GetPool(StringTable.UIScriptAddPool);
    }
    public override void OnStartShow()
    {
        UIView.Get<WellnessView>().ActiveDivider(false);
        base.OnStartShow();
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();
        foreach (var pool in pools)
        {
            pool.InActivePool();
        }
        pools.Clear();
    }
    public override void OnStartHide()
    {
        UIView.Get<WellnessView>().ActiveDivider(true);
        base.OnStartHide();
    }
    public void Set(ArticleDescriptions.Article article)
    {
        foreach (var scriptData in article.scriptDatas)
        {
            UIScriptFrame uiFrame = GetFrame(scriptData);
            uiFrame.Set(scriptData);
            pools.Add(uiFrame);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }
    private UIScriptFrame GetFrame(ScriptData scriptData)
    {
        if(scriptData is SubScriptData)
        {
            return subScriptPool.Get<UIScriptSubjectFrame>(content);
        }
        else if( scriptData is PanelScriptData)
        {
            return panelScriptPool.Get<UIScriptPanelFrame>(content);
        }
        else if(scriptData is AddScriptData)
        {
            return addScriptPool.Get<UIScriptAddFrame>(content);
        }
        
        Debug.LogError("Not Convert ScripData");
        return null;
    }
}
