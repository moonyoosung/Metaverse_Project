using MindPlus;
using MindPlus.Contexts;
using Slash.Unity.DataBind.Core.Presentation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeditationUIManager : BaseUIManager
{
    public MasterMeditationContext Context { private set; get; }

    private void Start()
    {
        StartCoroutine(Initialize(GameManager.Instance.Persistent));
    }

    public override IEnumerator Initialize(Persistent persistent)
    {
        yield return base.Initialize(persistent);
        this.Context = new MasterMeditationContext();
        ContextHolder.Context = Context;

        //몇개가 들어갈지 모르니 일단 다가져오자...
        UIView[] UIViews = GetComponentsInChildren<UIView>();

        foreach (var view in UIViews)
        {
            view.Initialize(persistent, this);
        }
        BreathView breathView = UIView.Get<BreathView>();
        this.Push(breathView.name, true, true, null, breathView);
    }
}

