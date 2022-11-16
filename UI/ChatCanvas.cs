using Photon.Chat;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using DG.Tweening;
using MindPlus.Contexts.Master;
using MindPlus;
using UnityEngine.EventSystems;

public class ChatCanvas : MonoBehaviour, PhotonChat.IChatClientListener
{
    [Header("UI")]
    public InputField inputField;
    public RectTransform contentRect;
    public Scrollbar scrollbar;
    public Toggle toggle;

    [Header("Alpha")]
    public CanvasGroup[] FadeGroup;
    public Image background;

    [NonSerialized]
    private Action onAction;
    private PhotonChat photonChat;
    private StringFilterData stringFilterData;
    private bool isFocus = false;
    public UnityEvent onPrint;
    public ChatFocus[] chatFocus;

    private MainViewContext context;
    public void AddLine(string lineString) => context.SetValue("ChatListText", context.ChatListText += lineString + "\r\n");

    public void Connect()
    {
        context.SetValue("ChatListText", photonChat.GetStrFomat(PhotonChat.MSGKIND.SYSTEM, " Start your Conversation "));

        onAction?.Invoke();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (!isFocus)
            onPrint?.Invoke();

        for (int i = 0; i < messages.Length; i++)
        {
            string[] splits = messages[i].ToString().Split('/');
            string result = "";
            if (splits[0] == GameManager.Instance.Persistent.AccountManager.PlayerData.userName)
            {
                result = photonChat.GetStrFomat(PhotonChat.MSGKIND.MINE, splits[0], splits[1]);
            }
            else
            {
                result = photonChat.GetStrFomat(PhotonChat.MSGKIND.PUBLIC, splits[0], splits[1]);
            }
            AddLine(result);
        }

        onAction?.Invoke();
    }

    private void ContentPivotChange()
    {
        if (!scrollbar.IsActive() && contentRect.pivot.y < 1)
        {
            contentRect.pivot = new Vector2(contentRect.pivot.x, 1);
        }
        else if (scrollbar.IsActive() && contentRect.pivot.y > 0)
        {
            contentRect.pivot = new Vector2(contentRect.pivot.x, 0);
        }
    }

    private bool Command(string text)
    {
        //테스트를 위해 임시로 추가 제거 예정
        switch (text.ToLower())
        {
#if UNITY_IOS
            //설정 창 이동
            case "open1":
                Application.OpenURL("app-settings:");
                return true;
            case "open2":
                Application.OpenURL("app-settings:root = Privacy");
                return true;
            case "open3":
                Application.OpenURL("app-settings = Privacy");
                return true;
            case "open4":
                Application.OpenURL("app-settings:Privacy");
                return true;
            case "open5":
                Application.OpenURL("app-prefs:root = Privacy");
                return true;
#endif
            //사용중인 마이크 확인을 위해서 임시로 추가
            case "/getmic":
                if (!GameManager.Instance.Persistent.VoiceManager.pvv || !GameManager.Instance.Persistent.VoiceManager.pvv.RecorderInUse) return false;
                var devices = GameManager.Instance.Persistent.VoiceManager.pvv.RecorderInUse.MicrophonesEnumerator.Devices;
                foreach (var item in devices)
                {
                    string temp = "String: " + item.IDString + " int: " + item.IDInt + " Pos: " + Microphone.GetPosition(item.IDString);
                    AddLine(temp.ToString());
                    //photonChat.PublishMessage(photonChat.channelName, temp);
                    //Debug.Log(temp);
                }
                var device = GameManager.Instance.Persistent.VoiceManager.pvv.RecorderInUse.MicrophoneDevice;
                string temp2 = "Use String: " + device.IDString + " int: " + device.IDInt + " Pos: " + Microphone.GetPosition(device.IDString);
                AddLine(temp2.ToString());
                return true;

            case "/getch":
                AddLine(photonChat.channelName.ToString());
                return true;
            case "/micup":
                GameManager.Instance.Persistent.VoiceManager.MicAmplifierUp();
                return true;
            case "/micdown":
                GameManager.Instance.Persistent.VoiceManager.MicAmplifierDown();
                return true;
            case "/?":
                AddLine("getmic : DeviceMicInfo" + "\r\n" + "getch : ChatChannelName");
                return true;
        }
        return false;
    }

    public void Submit(string text)
    {
        if (text == string.Empty) return;

        //임시로 추가
        //if (!Command(text))
        {
            string msg = stringFilterData.TextFilter(text);

            if (photonChat.chatClient.CanChat)
            {
                #region 귓속말
                //if (msg.Length > 4 && msg.Substring(0, 3).Equals("/w "))
                //{
                //    //TODO : 귓속말
                //    string[] str = msg.Split("/t");
                //    if (str.Count() > 2)
                //    {
                //        photonChat.chatClient.SendPrivateMessage
                //        (
                //            photonChat.userName,
                //            photonChat.GetStrFomat(PhotonChat.MSGKIND.PRIVATE, "PRIVATE")
                //        ); ;
                //    }
                //}
                //else
                #endregion
                {
                    photonChat.SendMessage
                    (
                        photonChat.channelName,
                        msg
                    );
                }
            }
            else if (photonChat.chatClient.State == ChatState.Disconnecting || photonChat.chatClient.State == ChatState.Disconnected)
            {
                photonChat.Reconnect();
                AddLine(photonChat.GetStrFomat(PhotonChat.MSGKIND.FAIL));
            }
            else
            {
                photonChat.EnqueueMessage(msg);
                //StartCoroutine(photonChat.WaitSendMessage(photonChat.channelName, msg));

            }
        }
        inputField.text = string.Empty;
        EventSystem.current.SetSelectedGameObject(null);
    }


    void OnDestroy()
    {
        if (photonChat != null) photonChat.UnResistEventHandler(this);
        toggle.onValueChanged.RemoveListener(OnClickChate);
    }

    public void Initialize(Persistent persistent, MainViewContext context)
    {
        this.context = context;
        this.photonChat = persistent.PhotonChat;
        photonChat.ResistEventHandler(this);

        toggle.onValueChanged.AddListener(OnClickChate);
        context.SetValue("MaxLine", 500);
        inputField.shouldHideMobileInput = true;
        inputField.characterLimit = 72;
        inputField.onSubmit.AddListener(delegate { Submit(inputField.text); });

        onPrint = new UnityEvent();
        onPrint.AddListener(SetScrollValue);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR    //윈도우에서 채팅 포커싱유지
        onPrint.AddListener(delegate { inputField.ActivateInputField(); });
#endif
        if (!stringFilterData)
        {
            stringFilterData = persistent.ResourceManager.StringFilterData;
            stringFilterData.SetFilterData();
        }

        onAction += ContentPivotChange;

        for (int i = 0; i < chatFocus.Length; i++)
        {
            chatFocus[i].onFadeIn += FadeIn;
            chatFocus[i].onFadeOut += FadeOut;
            chatFocus[i].onDestroy += DelFade;
        }

        gameObject.SetActive(false);
    }

    private void OnClickChate(bool isOn)
    {
        gameObject.SetActive(isOn);

        if (!isFocus)
        {
            FadeIn();
        }
        if (isOn)
        {
            EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        }
    }

    public void SetScrollValue()
    {
        context.SetValue("ScrollValue", 0);
    }

    public void SendMessage(string channel, string text)
    {
    }

    public void FadeIn()
    {
        if (isFocus) return;

        FadeGroup[0].DOKill();
        FadeGroup[0].DOFade(1f, 0.1f).OnPlay(() =>
        {
            background.DOKill();
            background.DOFade(0.6f, 0.1f);
        });
        isFocus = true;
    }

    public void FadeOut()
    {
        if (!isFocus) return;

        FadeGroup[0].DOKill();
        FadeGroup[0].DOFade(0, 0.5f).SetDelay(2f).OnPlay(() =>
        {
            background.DOKill();
            background.DOFade(0, 0.5f);
        });
        isFocus = false;
    }

    void DelFade()
    {
        if (!isFocus) return;

        for (int i = 0; i < FadeGroup.Count(); i++)
        {
            FadeGroup[i].DOKill();
        }
        background.DOKill();
        isFocus = false;
    }
}

