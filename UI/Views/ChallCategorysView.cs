using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System;

public class ChallCategorysView : UIView, ChallengeManager.IEventHandler
{
    [TitleGroup("[Option]")]
    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    public ScrollRect scroll;
    private UIPool ChallCategoryPool;
    private UIPool assignPool;
    private LayoutGroup contentLayoutGroup;
    private ChallengeManager challengeManager;
    private List<UIChallCategory> categories = new List<UIChallCategory>();
    private List<UIChallenge> challenges = new List<UIChallenge>();
    private ImageContainer iconContainer;
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.challengeManager = persistent.ChallengeManager;
        this.iconContainer = persistent.ResourceManager.ChallengeImages;
        challengeManager.ResistEvent(this);
        this.assignPool = (UIManager as UIManager).GetPool(StringTable.UIAssignmentPool);
        this.ChallCategoryPool = (UIManager as UIManager).GetPool(StringTable.UIChallCategoryPool);
        contentLayoutGroup = scroll.content.GetComponent<VerticalLayoutGroup>();
    }
    public override void OnStartShow()
    {
        challengeManager.GetChallenges();
        base.OnStartShow();
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();
        foreach (var challenge in challenges)
        {
            challenge.InActivePool();
        }
        challenges.Clear();
        foreach (var category in categories)
        {
            category.InActivePool();
        }
        categories.Clear();
    }
    public void OnGetChallenges(ChallengeManager.ChallengeList data)
    {
        foreach (var challenge in data.challenges)
        {
            //카테고리 리스트에서 검색해서 없으면 새로 카테고리 생성
            UIChallCategory category = GetCategory(challenge.part);

            UIChallenge assignment = assignPool.Get<UIChallenge>(category.gridLayoutGroup.transform);
            assignment.Set(challengeManager.activityData, challenge, iconContainer.Get(challenge.challengeId));
            challenges.Add(assignment);

            category.LayoutForceUpdate();
        }

        foreach (var category in categories)
        {
            category.SetText();
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)contentLayoutGroup.transform);
    }

    private UIChallCategory GetCategory(string target)
    {
        foreach (var category in categories)
        {
            if (category.ID == target)
            {
                return category;
            }
        }
        //새로 생성
        UIChallCategory uIChallCategory = ChallCategoryPool.Get<UIChallCategory>(scroll.content);
        uIChallCategory.ID = target;
        categories.Add(uIChallCategory);

        return uIChallCategory;
    }

    public void OnGetActivities(ChallengeManager.ActivityData data)
    {
    }
}
