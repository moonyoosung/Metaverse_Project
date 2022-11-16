using MindPlus.Contexts.Master.Menus.ChallengeView;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeDescriptionView : UIView
{
    [TitleGroup("[Option]")]
    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    public VerticalLayoutGroup verticalLayoutGroup;
    private ChallengeDescriptions challengeDescriptions;
    private ChallengeDescriptionViewContext context;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.context = new ChallengeDescriptionViewContext();
        ContextHolder.Context = context;
        this.challengeDescriptions = persistent.ResourceManager.ChallengeDescriptions;
    }

    public void Set(string challengeId)
    {
        ChallengeDescriptions.Description description = challengeDescriptions.Get(challengeId);

        context.SetValue("Title", description.title);
        context.SetValue("Description", description.description);
        context.SetValue("Icon", description.image);

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)verticalLayoutGroup.transform);
    }
}
