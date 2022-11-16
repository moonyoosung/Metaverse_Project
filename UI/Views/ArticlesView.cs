using MindPlus.Contexts.Master.Menus.WellnessView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArticlesView : UIView
{
    public RectTransform content;
    public RectTransform articleContent;
    private ArticleDescriptions articles;
    private ArticlesViewContext context;
    private UIPool articlePool;
    private List<UIArticle> pools = new List<UIArticle>();

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.context = new ArticlesViewContext();
        ContextHolder.Context = context;
        this.articles = persistent.ResourceManager.ArticleDescriptions;
        articlePool = (uIManager as UIManager).GetPool(StringTable.UIArticlePool);
    }

    public override void OnStartShow()
    {
        foreach (var article in articles.articles)
        {
            UIArticle uIArticle = articlePool.Get<UIArticle>(articleContent);
            uIArticle.Set(article);
            pools.Add(uIArticle);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
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
}
