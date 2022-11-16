using MindPlus.Contexts.Master.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PeopleView : UIFlatView
{
    public UIMaskSlider peopleMaskSlider;
    public UIMaskSlider profileMaskSlider;
    private PeopleViewContext context;

    private Sprite peopleIcon;
    private Sprite backIcon;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        ImageContainer imageContainer = persistent.ResourceManager.ImageContainer;
        peopleIcon = imageContainer.Get("menupeople");
        backIcon = imageContainer.Get("menubackarrow");

        UIManager uiManager = uIManager as UIManager;
        this.context = new PeopleViewContext();
        ContextHolder.Context = context;
        uiManager.MasterContext.MenuViewContext.PeopleViewContext = context;

        peopleMaskSlider.onStart += OnStartSlide;
        profileMaskSlider.onStart += OnStartSlide;

        context.onClickBack += OnClickBack;
        context.onClickClose += OnClickClose;
        context.SetValue("IsAlbe", false);

        Set(false);
    }

    private void OnClickClose()
    {
        context.onClickClose -= OnClickClose;
        UIManager.Pop("", false, false, true, null, () => { context.onClickClose += OnClickClose; });

        if (navigation.Current is PeopleAboutView || navigation.Current is ProfileAvatarView)
        {
            Get<ProfileAvatarView>().SetAvatarActive(false);
        }
    }

    private void OnClickBack()
    {
        context.onClickBack -= OnClickBack;

        if (navigation.Current is PeopleListView)
        {
            Pop(false, false, true, null, () => { context.onClickBack += OnClickBack; });
        }
        else
        {
            context.onClickBack += OnClickBack;
            (UIManager as UIManager).PopToRoot(navigation, 1, false, false, true);
            if (navigation.Current is PeopleAboutView || navigation.Current is ProfileAvatarView)
            {
                Get<ProfileAvatarView>().SetAvatarActive(false);
            }
        }
        Set(false);
    }

    public override void OnStartShow()
    {
        base.OnStartShow();
        if (navigation.history.Count == 0)
        {
            peopleMaskSlider.current = 7;
            context.onClickSlider.Invoke(0);
        }
        else
        {
            //peopleMaskSlider.OnMove(0);
        }
    }

    public override void OnFinishHide()
    {
        if (context.IsTitleInteract)
        {
            Set(false);
        }
        peopleMaskSlider.current = 7;
        profileMaskSlider.current = 7;
        base.OnFinishHide();
        navigation.PopToRoot();
    }

    private void OnStartSlide(int Current, int next)
    {
        if (Current == next)
        {
            return;
        }

        UIView[] views = new UIView[] { Get<FriendView>(), Get<FollowView>(), Get<ExplorerView>(), Get<FriendRequestView>(), Get<PeopleAboutView>(), Get<PeopleAboutView>(), Get<PeopleAboutView>() };

        Push("", false, true, null, views[next]);
    }

    /// <summary>
    /// PeopleView Data Set
    /// </summary>
    /// <param name="isInteractable">PeopleView Button Interactable</param>
    /// <param name="isOpenPofile">Profile Access</param>
    public void Set(bool isInteractable, bool isOpenPofile = false)
    {
        if (isInteractable)
        {
            context.SetValue("TitleIcon", backIcon);
        }
        else
        {
            context.SetValue("TitleIcon", peopleIcon);
        }

        if (isOpenPofile)
        {
            context.SetValue("TitleText", "User profile");

            context.onClickSlider -= peopleMaskSlider.OnMove;
            context.onClickSlider += profileMaskSlider.OnMove;
        }
        else
        {
            context.onClickSlider += peopleMaskSlider.OnMove;
            context.onClickSlider -= profileMaskSlider.OnMove;
            context.SetValue("TitleText", "People");
        }

        context.SetValue("PeopleSliderActive", !isInteractable);
        context.SetValue("ProfileSliderActive", isOpenPofile);

        context.SetValue("IsActiveDivider", isInteractable);
        context.SetValue("IsTitleInteract", isInteractable);
        context.SetValue("IsActiveInvite", !isOpenPofile);
    }
}

