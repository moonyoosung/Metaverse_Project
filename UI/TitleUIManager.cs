using MindPlus.Contexts;
using Slash.Unity.DataBind.Core.Presentation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUIManager : BaseUIManager
{
    public MasterTitleContext Context { private set; get; }
    public override IEnumerator Initialize(Persistent persistent)
    {
        yield return base.Initialize(persistent);
        this.Context = new MasterTitleContext();
        ContextHolder.Context = Context;

        UIView[] UIViews = GetComponentsInChildren<UIView>();

        foreach (var view in UIViews)
        {
            view.Initialize(persistent, this);
        }

        bool isInit = false;

        Hide(StringTable.Init, true, ()=> { isInit = true; },UIViews);

        while (!isInit)
        {
            yield return null;
        }
    }
}
