using MindPlus.Contexts.Pool;
using Slash.Unity.DataBind.Core.Presentation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBioChallenge : UIPoolObject
{
    public Toggle toggle;
    public ContextHolder contextHolder;
    private UIBioChallengeContext context;
    public override void Initialize(Transform parent)
    {
        base.Initialize(parent);
        this.context = new UIBioChallengeContext();
        contextHolder.Context = context;
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public override void InActivePool()
    {
        base.InActivePool();
        toggle.group = null;
    }

    private void OnToggleChanged(bool isOn)
    {
        BioDataDetailView bioDataDetailView = UIView.Get<BioDataDetailView>();
        if (isOn)
        {
            bioDataDetailView.ChartPointOn(ID);
        }
        else
        {
            bioDataDetailView.OffPoints();
        }
    }

    public void Set(string text, Sprite Icon, string ID = "", ToggleGroup toggleGroup = null)
    {
        context.SetValue("Title", text);

        if (Icon == null)
        {
            context.SetValue("IsActiveIcon", false);
            return;
        }
        context.SetValue("IsActiveIcon", true);
        context.SetValue("Icon", Icon);
        SetID(ID);
        toggle.group = toggleGroup;
    }
}
