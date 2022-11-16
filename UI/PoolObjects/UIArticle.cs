using MindPlus.Contexts.Pool;
using Slash.Unity.DataBind.Core.Presentation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIArticle : UIPoolObject
{
    public ContextHolder contextHolder;
    private UIArticleContext context;
    private ArticleDescriptions.Article article;

    public override void Initialize(Transform parent)
    {
        base.Initialize(parent);
        this.context = new UIArticleContext();
        contextHolder.Context = context;
    }
    public override void InActivePool()
    {
        base.InActivePool();
        article = null;
        if (context.onClickArticle == OnClickArticle)
            context.onClickArticle -= OnClickArticle;
    }
    public void Set(ArticleDescriptions.Article article)
    {
        this.article = article;
        context.SetValue("Title", article.title);
        context.SetValue("Image", article.image);
        context.onClickArticle += OnClickArticle;
    }

    private void OnClickArticle()
    {
        context.onClickArticle -= OnClickArticle;

        ArticleDescriptionView descriptionView = UIView.Get<ArticleDescriptionView>();
        WellnessView wellnessView = UIView.Get<WellnessView>();
        wellnessView.SetBackButton(true, "CarePlay Articles");
        descriptionView.Set(article);
        wellnessView.Push("", false, false, null, new UIView[] { descriptionView });
    }
}
