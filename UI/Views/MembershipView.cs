using MindPlus;
using MindPlus.Contexts.TitleView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class MembershipView : UIView, AccountManager.IEventHandler
{
    public InputField IDInput;
    public InputField PWInput;

    private MembershipViewContext context;
    private AccountManager accountManager;

    private Sprite veriError;
    private Sprite veriCheck;

    private Color errorColor = new Color(1, 122f / 255f, 48f / 255f);
    private Color checkColor = new Color(0.35f, 0.9f, 0.21f);
    public override void OnDestroy()
    {
        accountManager.UnResisterEvent(this); 
        base.Remove();
    }
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.accountManager = persistent.AccountManager;
        veriError = persistent.ResourceManager.ImageContainer.Get("verierror");
        veriCheck = persistent.ResourceManager.ImageContainer.Get("vericheck");
        this.context = new MembershipViewContext();
        ContextHolder.Context = this.context;

        ResistSignUpButton(true);
        context.onClickPasswordClose += OnClickPasswordClose;
        context.onClickPasswordOpen += OnClickPasswordOpen;
        context.onClickLink += OnClickLink;
        ResistBackButton(true);

        accountManager.ResisterEvent(this);
        OnClickPasswordClose();
    }
    private void ResistBackButton(bool isResist)
    {
        if (isResist)
            context.onClickBack += OnClickBack;
        else
            context.onClickBack -= OnClickBack;
    }
    private void ResistSignUpButton(bool isResist)
    {
        if (isResist)
            context.onClickSignUp += OnClickSignUp;
        else
            context.onClickSignUp -= OnClickSignUp;
    }
    private void OnClickBack()
    {
        ResistBackButton(false);

        Get<TitleMainPanelView>().Pop(false, false, true, null, () => ResistBackButton(true));
    }

    private void OnClickLink()
    {
#if PLATFORM_IOS
         Application.OpenURL(Address.Link);
#else
        System.Diagnostics.Process.Start(Address.Link);
#endif
    }

    public override void OnStartShow()
    {
        context.SetValue("IsActiveIDNotify", false);
        context.SetValue("IsActivePWNotify", false);
        context.SetValue("AgreeToggle", false);
        context.SetValue("ID", string.Empty);
        context.SetValue("Password", string.Empty);
        IDInput.onValueChanged.AddListener(OnIDFieldChanged);
        PWInput.onValueChanged.AddListener(OnPasswordFieldChanged);
        base.OnStartShow();
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();
        IDInput.onValueChanged.RemoveListener(OnIDFieldChanged);
        PWInput.onValueChanged.RemoveListener(OnPasswordFieldChanged);
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
    public void OnIDFieldChanged(string value)
    { 
        if (IdCheck(context.ID)|| LengthCheck(4, 10, context.ID)|| value == string.Empty)
        {
            context.SetIDNotify("Must contain 4-10 characters without symbols.", veriError, errorColor);
            return;
        }
        context.SetIDNotify("Looks good!", veriCheck, checkColor);
    }
    public void OnPasswordFieldChanged(string value)
    {
        if (PwCheck(context.Password)||LengthCheck(8, 16, context.Password)|| value == string.Empty)
        {
            context.SetPWNotify("Must contain 8-16 charcters.", veriError, errorColor);
            return;
        }

        context.SetPWNotify("Looks good!", veriCheck, checkColor);
    }
    private void OnClickSignUp()
    {
        ResistSignUpButton(false);
        if (context.ID == string.Empty)
        {
            context.SetIDNotify("Please enter your ID.", veriError, errorColor);
        }
        if (context.Password == string.Empty)
        {
            context.SetPWNotify("Please enter your password.", veriError, errorColor);
            ResistSignUpButton(true);
            return;
        }
        if (!context.AgreeToggle)
        {
            ResistSignUpButton(true);
            return;
        }
        GameManager.Instance.Persistent.UIManager.ActiveIndicator(true);

        accountManager.SignUp(context.ID, context.Password, () =>
        {
            GameManager.Instance.Persistent.UIManager.ActiveIndicator(false);
        });
    }
    private bool LengthCheck(int min, int max, string text)
    {
        if (text.Length < min || text.Length > max)
            return true;
        else
            return false;
    }
    private bool IdCheck(string text)
    {
        return Regex.IsMatch(text, @"[^0-9a-zA-Z_-]");
    }
    private bool PwCheck(string text)
    {
        return Regex.IsMatch(text, @"[^]0-9a-zA-Z-!@#$%^&*()_+={[}|;:;''<,>./?]");
    }

    public void OnSuccessLogin(LocalPlayerData playerData)
    {
    }

    public void OnFailLogin(string error)
    {
    }

    public void OnSuccessSignUp()
    {
    }

    public void OnFailSignUp(string error)
    {
        if (error == "KeyAlreadyExists")
            context.SetIDNotify(context.ID + " is an ID that already exists.", veriError, errorColor);
    }

    public void OnSuccessGetUser(LocalPlayerData playerData)
    {
    }

    public void OnFailGetUser(string error)
    {
    }
}
