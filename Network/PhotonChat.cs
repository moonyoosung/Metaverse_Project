using Assets.A_MindPlus.Scripts.Network;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using MindPlus;
using System.Collections;

public class PhotonChat : MonoBehaviourPunChatCallbacks
{
    public interface IChatClientListener
    {
        void Connect();
        void OnGetMessages(string channelName, string[] senders, object[] messages);
        void SendMessage(string channel, string text);
    }

    public enum MSGKIND
    {
        SUBSCRIBE,
        EXIT,
        DISCONNECT,
        FAIL,
        PRIVATE,
        PUBLIC,
        SYSTEM,
        MINE
    }

    List<IChatClientListener> clientListeners = new List<IChatClientListener>();

    [NonSerialized]
    public ChatClient chatClient;
    public string id;
    public string channelName;
    public Queue<string> msgQueue;
    private string appIdChat;

    public void OnApplicationQuit()
    {
        if (chatClient != null)
        {
            chatClient.Disconnect();
        }
    }

    void Start()
    {
        msgQueue = new Queue<string>();
    }

    void Update()
    {
        if (chatClient == null) return;

        chatClient.Service();

        if (msgQueue != null && msgQueue.Count > 0)
        {
            if (chatClient.CanChat)
            {
                SendMessage(channelName, msgQueue.Dequeue());
            }
        }
    }

    public void ResistEventHandler(IChatClientListener eventHandler)
    {

        for (int i = 0; i < clientListeners.Count; i++)
        {
            if (clientListeners[i].Equals(null))
                clientListeners.RemoveAt(i);

        }
        clientListeners.Add(eventHandler);
    }

    public void UnResistEventHandler(IChatClientListener eventHandler)
    {
        clientListeners.Remove(eventHandler);
    }

    public void ClearEventHandler()
    {
        clientListeners.Clear();
    }

    void OnDestroy()
    {
        ClearEventHandler();
    }

    public void Connect(string id, string channelName)
    {
        if (!AppIdValid()) return;

        Application.runInBackground = true;

        this.id = id;
        this.channelName = channelName;

        if (chatClient != null)
        {
            chatClient.Disconnect();
            chatClient = null;
        }
        chatClient = new ChatClient(this);

        foreach (var clientListener in clientListeners)
        {
            clientListener.Connect();
        }

        chatClient.Connect(appIdChat, "1.0", new AuthenticationValues(this.id));
    }

    public void Reconnect()
    {
        if (!AppIdValid()) return;
        Connect(PhotonNetwork.LocalPlayer.UserId, this.channelName);
    }

    public void Disconnect()
    {
        if (chatClient == null) return;
        chatClient.Disconnect();
    }

    bool AppIdValid()
    {
        appIdChat = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat;

        //TODO
        if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat)) return false;
        return true;
    }
    public void Subscribe()
    {
        chatClient.Subscribe(new string[] { channelName }, -1);
        chatClient.SetOnlineStatus(ChatUserStatus.Online, null);
    }

    public override void DebugReturn(DebugLevel level, string message)
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public override void OnDisconnected()
    {
        Application.runInBackground = false;
    }

    public override void OnConnected()
    {
        Subscribe();
    }

    public override void OnChatStateChange(ChatState state)
    {
        Debug.Log("Chat :: OnChatStateChange = " + state);
    }

    public override void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        foreach (var clientListener in clientListeners)
        {
            clientListener.OnGetMessages(channelName, senders, messages);
        }
    }

    public override void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("OnPrivateMessage : " + message);
    }

    public override void OnSubscribed(string[] channels, bool[] results)
    {
    }

    public override void OnUnsubscribed(string[] channels)
    {
    }

    public override void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
    }

    public virtual void PublishMessage(string channel, string text)
    {
    }

    public virtual void SendMessage(string channel, string text)
    {
        foreach (var clientListener in clientListeners)
        {
            clientListener.SendMessage(channel, text);
        }

        chatClient.PublishMessage(channel, GameManager.Instance.Persistent.AccountManager.PlayerData.userName + "/" + text);
    }

    public string GetStrFomat(MSGKIND chatkind, string str1 = "", string str2 = "")
    {
        switch (chatkind)
        {
            case MSGKIND.PRIVATE: return string.Format("<color=red>[{0}] {1}</color>", str1, str2);
            case MSGKIND.PUBLIC: return string.Format("<color=#FFFFFF>[{0}]{1}</color>", str1, str2);
            case MSGKIND.SUBSCRIBE: return string.Format("<b>[{0}] 님이 {1} 방에 입장하셨습니다.</b>", str1, str2);
            case MSGKIND.EXIT: return string.Format("<b>채널 퇴장({0})<b>", str1);
            case MSGKIND.FAIL: return "<b>Failed to send message.<b>";
            case MSGKIND.DISCONNECT: return "<b>Terminate the connection.< b>";
            case MSGKIND.SYSTEM: return string.Format("<color=#E4E4E4><i>[{0}]</i></color>\n", str1);
            case MSGKIND.MINE: return string.Format("<color=#A3FF8C>[{0}]{1}</color>", str1, str2);
        }
        return string.Format("<color=red>[{0}]:</color>{1}", str1, str2);
    }

    public void EnqueueMessage(string msg)
    {
        msgQueue.Enqueue(msg);
    }
    //public IEnumerator WaitSendMessage(string channel, string msg)
    //{
    //    if (chatClient == null) yield break;

    //    if(chatClient.State == ChatState.Disconnecting || chatClient.State == ChatState.Disconnected)
    //    {
    //        Reconnect();
    //        yield break;
    //    }

    //    yield return new WaitUntil(() => chatClient != null && chatClient.CanChat);
    //    Debug.Log("SendMessageSendMessage");
    //    SendMessage(channel, msg);
    //}
}
