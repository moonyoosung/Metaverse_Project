using MindPlus;
using MindPlus.Contexts.TitleView;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : UIView, AccountManager.IEventHandler
{
    private AccountManager accountManager;

    private LoginViewContext context;
    private Sprite veriError;
    private Color errorColor = new Color(1, 122f / 255f, 48f / 255f);

    private Persistent persistent;

    [TitleGroup("[Option]")]
    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    public bool isDelete = false;
    public override void OnDestroy()
    {
        accountManager?.UnResisterEvent(this);
        base.Remove();
    }
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        TitleUIManager titleUIManager = uIManager as TitleUIManager;

        this.accountManager = persistent.AccountManager;
        this.persistent = persistent;

        veriError = persistent.ResourceManager.ImageContainer.Get("verierror");

        this.context = new LoginViewContext();
        ContextHolder.Context = context;
        titleUIManager.Context.LoginViewContext = context;
        context.SetValue("IDColor", Color.white);
        context.SetValue("PWColor", Color.white);

        accountManager.ResisterEvent(this);
        OnClickPasswordClose();

        ResistSignInButton(true);
        ResistMembershipButton(true);
        context.onClickPasswordOpen += OnClickPasswordOpen;
        context.onClickPasswordClose += OnClickPasswordClose;
        context.onClickForgot += OnClickForgot;
    }

    private void OnClickForgot()
    {
        Debug.Log("OnClickEmail");
        Application.OpenURL("mailto:careplay_cs@looxidlabs.com");
    }

    public override void OnStartShow()
    {
        ResetField();
        base.OnStartShow();
    }
    public override void OnFinishShow()
    {
        base.OnFinishShow();
        StartCoroutine(WaitForInitalized());
    }

    //???? ?????? ?????? ?? ?????? ?????? ???????? ???? ?????????? ???????? ???????????? ????
    private IEnumerator WaitForInitalized()
    {
        yield return new WaitUntil(() => GameManager.Instance.isInitalized);
        yield return null;
        CheckSaveData();
    }

    private void ResistSignInButton(bool isResist)
    {
        if (isResist)
            context.onClickSignIn += OnClickSignIn;
        else
            context.onClickSignIn -= OnClickSignIn;
    }
    private void ResistMembershipButton(bool isResist)
    {
        if (isResist)
            context.onClickMembership += OnClickMemeberShip;
        else
            context.onClickMembership -= OnClickMemeberShip;
    }
    public void CheckSaveData()
    {
        if (isDelete)
            PlayerPrefs.DeleteKey("userId");
        //??????
        //string userid = string.Empty;//PlayerPrefs.GetString("userId");
        //string pw = string.Empty;//PlayerPrefs.GetString("pw");

        string userid = PlayerPrefs.GetString("userId");
        string pw = PlayerPrefs.GetString("pw");
        //AudioManager.Instance.SetBgmSound(AudioManager.Bgm.TITLE);
        if (GameManager.isLoggingOut)
        {
            context.SetValue("RememberToggle", false);
            PlayerPrefs.DeleteKey("userId");
            PlayerPrefs.DeleteKey("pw");
        }
        else if (string.IsNullOrEmpty(userid) == false && string.IsNullOrEmpty(pw) == false)
        {
            context.SetValue("ID", userid);
            context.SetValue("Password", pw);
            context.SetValue("RememberToggle", true);
            GameManager.Instance.Persistent.UIManager.ActiveIndicator(true);
            accountManager.SignInAuto(userid, pw, () =>
            {
                GameManager.Instance.Persistent.UIManager.ActiveIndicator(false);
            });
        }
        GameManager.isLoggingOut = false;
    }
    private void OnClickPasswordClose()
    {
        context.SetValue("PasswordType", InputField.ContentType.Password);
        context.SetValue("IsActivePasswordClose", true);
        context.SetValue("IsActivePasswordOpen", false);
    }

    private void OnClickPasswordOpen()
    {
        context.SetValue("PasswordType", InputField.ContentType.Standard);
        context.SetValue("IsActivePasswordClose", false);
        context.SetValue("IsActivePasswordOpen", true);
    }

    private void OnClickMemeberShip()
    {
        ResistMembershipButton(false);
        MembershipView view = Get<MembershipView>();
        Get<TitleMainPanelView>().Push(view.name, false, false, () => ResistMembershipButton(true), view);
    }

    private void OnClickSignIn()
    {
        ResistSignInButton(false);

        if (context.ID == string.Empty || context.Password == string.Empty)
        {
            context.SetNotify("Please enter your ID and password.", veriError, errorColor);
            ResistSignInButton(true);
            return;
        }

        if (IdCheck(context.ID))
        {
            context.SetNotify("Your username or password is invaild.", veriError, errorColor);
            context.SetValue("IDColor", errorColor);
            ResistSignInButton(true);
            return;
        }

        GameManager.Instance.Persistent.UIManager.ActiveIndicator(true);

        accountManager.SignIn(context.ID, context.Password, () =>
        {
            GameManager.Instance.Persistent.UIManager.ActiveIndicator(false);
        });
    }


    private void ResetField()
    {
        context.SetValue("ID", "");
        context.SetValue("Password", "");
        context.SetValue("IsActiveNotify", false);
    }

    private bool IdCheck(string text)
    {
        return Regex.IsMatch(text, @"[^0-9a-zA-Z_-]");
    }

    public void OnSuccessLogin(LocalPlayerData playerData)
    {
        if (context.RememberToggle)
        {
            PlayerPrefs.SetString("userId", context.ID);
            PlayerPrefs.SetString("pw", context.Password);
        }

    }

    public void OnFailLogin(string error)
    {
        ResistSignInButton(true);

        if (error == "KeyNotFound")
        {
            context.SetNotify("Username does not exist.", veriError, errorColor);
            context.SetValue("IDColor", errorColor);
        }
        else if (error == "InvalidPassword")
        {
            context.SetNotify("Invalid password.", veriError, errorColor);
            context.SetValue("PWColor", errorColor);
        }
        else
        {
            context.SetNotify("Username and password did not match our records.", veriError, errorColor);
            context.SetValue("PWColor", errorColor);
            context.SetValue("IDColor", errorColor);
        }
    }

    public void OnSuccessSignUp()
    {
    }

    public void OnFailSignUp(string error)
    {

    }

    public void OnSuccessGetUser(LocalPlayerData playerData)
    {
        if (playerData.userName == null || playerData.userName == string.Empty)
        {
            NickNameView view = Get<NickNameView>();
            Get<TitleMainPanelView>().Push(view.gameObject.name, false, false, () => ResistSignInButton(true), view);
        }
        else
        {
            if (string.IsNullOrEmpty(playerData.head) || playerData.head.Length == 5)
            {
                CharacterView view = Get<CharacterView>();
                HalfPanelView panelview = Get<HalfPanelView>();
                SelectCharacterView selectCharacterView = Get<SelectCharacterView>();

                view.PushNavigation(selectCharacterView, panelview);

                UIManager.Push("", false, false, () => ResistSignInButton(true), view);
            }
            else
            {
                persistent.RoomDataBaseManager.RoomAPIHandler.JoinRoom(playerData.myRoomId, "myroom#private");
            }
        }
    }

    public void OnFailGetUser(string error)
    {
        Debug.Log("OnFailGetUseraaaaaaaaaaaaaaaaaaaaa");
    }
}
