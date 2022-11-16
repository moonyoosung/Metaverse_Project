using MindPlus.Contexts.TitleView;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using MindPlus;

public class NickNameView : UIView, PutUserDataCommand.IEventHandler
{
    private NickNameViewContext context;
    private AccountManager accountManager;
    private UIManager masterUIManager;
    private APIManager aPIManager;

    public override void OnDestroy()
    {
        aPIManager.UnResisterEvent(this);
        base.OnDestroy();
    }
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        TitleUIManager titleUIManager = uIManager as TitleUIManager;

        this.accountManager = persistent.AccountManager;
        this.masterUIManager = persistent.UIManager;
        this.aPIManager = persistent.APIManager;

        aPIManager.ResisterEvent(this);

        this.context = new NickNameViewContext();
        ContextHolder.Context = context;
        titleUIManager.Context.TitleContext = context;
        context.onClickNext += OnClickNext;

        context.nickNameChanged += OnNickNameChanged;
        context.SetValue("IsPlayInteract", true);
    }
    private void OnNickNameChanged(string nickName)
    {
        //1. PlayerData�� �÷��̾� ����
        accountManager.PlayerData.userName = nickName;
        context.SetValue("IsPlayInteract", true);
        //2. Player�� PlayerNick�� �̸�
        //setPlayerNick.Context.SetValue("NickName", nickName);
    }
    private void OnClickNext()
    {
        context.onClickNext -= OnClickNext;

        if (Util.LengthCheck(4, 10, accountManager.PlayerData.userName))
        {
            masterUIManager.PushNotify("Nickname must be between 4 and 10 characters.", 2f);
            context.onClickNext += OnClickNext;
            return;
        }

        if (Util.NameCheck(accountManager.PlayerData.userName))
        {
            masterUIManager.PushNotify("Nickname contains characters that are not allowed.", 2f);
            context.onClickNext += OnClickNext;
            return;
        }
        context.SetValue("IsPlayInteract", false);
        accountManager.ModifyUserData(accountManager.PlayerData.userId, new JObject { { "userName", accountManager.PlayerData.userName } });
    }

    public void OnPutUserNameSuccess(NetworkMessage message)
    {
       
    }

    public void OnPutUserNameFailed(NetworkMessage message)
    {

    }
}
