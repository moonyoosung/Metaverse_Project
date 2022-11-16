using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using MindPlus;
using UnityEngine.UI;
using MindPlus.Contexts.Master;
using System;
using UnityEngine.SceneManagement;

public class MainView : UIView
{
    public MobileRotateInput mobileRotateInput;

    public MobileMoveInput mobileMoveInput;

    public ChatCanvas chatCanvas;
    public PlayerVoice playerVoice;
    public AppSettingPopup appSettingPopup;

    public GameObject emojiList;

    public MainViewContext Context { private set; get; }
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);

        UIManager mainUiManager = uIManager as UIManager;
        this.Context = new MainViewContext();
        ContextHolder.Context = Context;
        mainUiManager.MasterContext.MobileControllerContext = Context;

        chatCanvas.Initialize(persistent, Context);
        playerVoice.Initialize(persistent, Context);
        appSettingPopup.Initialize(persistent, Context);

        Context.onClickEmotion += OnClickEmotion;
        Context.onClickMenu += OnClickMenu;
        Context.onClickGesture += OnClickGesture;
        Context.SetValue("IsActiveMute", true);

    }
  
    private void OnClickMenu()
    {
        Context.onClickMenu -= OnClickMenu;

        UIManager.Push("", false, false, ()=> { Context.onClickMenu += OnClickMenu; }, Get<MenuView>());
    }
    private void OnClickEmotion()
    {
        emojiList.SetActive(!emojiList.activeSelf);
    }
    private void OnClickGesture()
    {
        UIManager.Push("", false, false, null, Get<GestureView>());
    }

    public void Reset()
    {
        mobileRotateInput.gameObject.SetActive(true);
        mobileMoveInput.gameObject.SetActive(true);
    }

    public void ResistInputAction(System.Action<Vector2> moveBody, System.Action onEndDrag, System.Action<Vector2> rotateCamera)
    {
        Debug.Log("InputAction ResistInputAction");
        mobileMoveInput.InputAction += moveBody;
        mobileMoveInput.onEndDrag += onEndDrag;
        mobileRotateInput.InputAction += rotateCamera;
    }

    public void UnResistInputAction(System.Action<Vector2> moveBody, System.Action onEndDrag, System.Action<Vector2> rotateCamera)
    {
        Debug.Log("InputAction UnResistInputAction");
        mobileMoveInput.InputAction -= moveBody;
        mobileMoveInput.onEndDrag -= onEndDrag;
        mobileRotateInput.InputAction -= rotateCamera;
    }
    public void ResistMobileRotate(System.Action<Vector2> beginDrag, System.Action endDrag)
    {
        mobileRotateInput.onBeginDrag += beginDrag;
        mobileRotateInput.onEndDrag += endDrag;
    }
    public void UnResistMobileRotate(System.Action<Vector2> beginDrag, System.Action endDrag)
    {
        mobileRotateInput.onBeginDrag -= beginDrag;
        mobileRotateInput.onEndDrag -= endDrag;
    }
}
