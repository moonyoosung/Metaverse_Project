using MindPlus.Contexts.Master;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ProfileView : UIFlatView, GetOhterUserCommand.IEventHandler
{
    public UIMaskSlider otherProfileMaskSlider;
    public UIMaskSlider myProfileMaskSlider;
    private Persistent persistent;
    private ProfileViewContext context;
    private bool isMyProfile;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.persistent = persistent;

        UIManager uiManager = uIManager as UIManager;
        this.context = new ProfileViewContext();
        ContextHolder.Context = context;
        uiManager.MasterContext.ProfileViewContext = context;
        context.onClickClose += () =>
        {
            UIManager.Pop();
            Get<ProfileAvatarView>().SetAvatarActive(false);
        };
        myProfileMaskSlider.onStart += OnStartSlide;
        otherProfileMaskSlider.onStart += OnStartSlide;
    }

    public override void OnStartShow()
    {
        base.OnStartShow();
        if (navigation.history.Count == 0)
        {
            if (isMyProfile)
            {
                context.onClickSlider.Invoke(0);
            }
            else
            {
                context.onClickSlider.Invoke(4);
            }
        }
        else
        {
            if (isMyProfile)
            {
                otherProfileMaskSlider.OnMove(0);
            }
            else
            {
                myProfileMaskSlider.OnMove(4);
            }
        }
    }

    public override void OnFinishHide()
    {
        persistent.APIManager.UnResisterEvent(this);
        otherProfileMaskSlider.current = 7;
        myProfileMaskSlider.current = 7;
        base.OnFinishHide();
        navigation.PopToRoot();
    }

    private void OnStartSlide(int Current, int next)
    {
        if (Current == next)
        {
            return;
        }

        UIView[] views = new UIView[] {  Get<MyAboutView>(), Get<MyAboutView>(), Get<MyAboutView>(), Get<MyAboutView>(),
                                         Get<PeopleAboutView>(), Get<PeopleAboutView>(), Get<PeopleAboutView>(),
                                       };
        ProfileAvatarView avatarView = Get<ProfileAvatarView>();
        if ((UIFlatView)views[next])
        {
            UIFlatView flatView = (UIFlatView)views[next];
            if (!flatView.AlreadyExist(avatarView))
            {
                flatView.PushNavigation(avatarView);
            }
        }

        Push("", false, true, null, views[next]);
    }

    public void Set(string id, bool isMyProfile = false)
    {
        this.isMyProfile = isMyProfile;

        context.SetValue("IsActiveInfo", isMyProfile);
        context.onClickSlider -= myProfileMaskSlider.OnMove;
        context.onClickSlider -= otherProfileMaskSlider.OnMove;

        if (isMyProfile)
        {
            context.SetValue("TitleIcon", persistent.ResourceManager.ImageContainer.Get("myprofile"));
            context.SetValue("TitleText", "My profile");
            context.SetValue("MyProfileSliderActive", isMyProfile);
            context.onClickSlider += myProfileMaskSlider.OnMove;
        }
        else
        {
            context.SetValue("TitleIcon", persistent.ResourceManager.ImageContainer.Get("userprofile"));
            context.SetValue("TitleText", "User profile");
            context.SetValue("OtherProfileSliderActive", !isMyProfile);
            context.onClickSlider += otherProfileMaskSlider.OnMove;
        }
        persistent.APIManager.ResisterEvent(this);
        persistent.PeopleManager.GetOhterUser(id);
    }

    public void SetCost(int coin, int heart)
    {
        context.SetValue("costCoinText", coin == 0 ? "0" : string.Format(Format.Money, coin));
        context.SetValue("costHeartText", heart == 0 ? "0" : string.Format(Format.Money, heart));
    }

    public void OnGetOhterUserSuccess(NetworkMessage message)
    {
        LocalPlayerData playerData = JsonUtility.FromJson<LocalPlayerData>(message.body);

        if (isMyProfile)
        {
            Get<ProfileAvatarView>().Set(true, playerData.GetCustomAvatar().GetCustomPlusParts());
            Get<MyAboutView>().Set(playerData);
            SetCost(playerData.coin, playerData.heart);
        }
        else
        {
            Get<ProfileAvatarView>().Set(false, playerData.GetCustomAvatar().GetCustomPlusParts());
            Get<PeopleAboutView>().Set(playerData);
        }
        persistent.APIManager.UnResisterEvent(this);
    }

    public void OnGetOhterUserFailed(NetworkMessage message)
    {
    }
}
