
using MindPlus;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class AccountManager : SignInCommand.IEventHandler, SignUpCommand.IEventHandler, GetUserCommand.IEventHandler, PutUserDataCommand.IEventHandler, ModifyPasswordCommand.IEventHandler
{
    public interface IEventHandler
    {
        void OnSuccessLogin(LocalPlayerData playerData);
        void OnFailLogin(string error);
        void OnSuccessSignUp();
        void OnFailSignUp(string error);
        void OnSuccessGetUser(LocalPlayerData playerData);
        void OnFailGetUser(string error);
    }
    private List<IEventHandler> eventHandlers = new List<IEventHandler>();
    public LocalPlayerData PlayerData { private set; get; }

    public RoomDataBaseManager roomDataManager;
    private APIManager apiManager;
    private WebSocket socket;
    public DateTime LastSockSendTime { private set; get; }
    public void Initialize(APIManager apiManager, RoomDataBaseManager roomDataManager)
    {
        //Debug.Log("ttt Initialize AccountManager");
        this.PlayerData = new LocalPlayerData("DefaultName");

        this.apiManager = apiManager;
        apiManager.ResisterEvent(this);
        this.roomDataManager = roomDataManager;
    }


    public void ResisterEvent(IEventHandler eventHandler)
    {
        this.eventHandlers.Add(eventHandler);
    }
    public void UnResisterEvent(IEventHandler eventHandler)
    {
        this.eventHandlers.Remove(eventHandler);
    }

    bool VerificationAscii(int min, int max, int ascii)
    {
        if (ascii >= min && ascii <= max)
            return true;
        return false;
    }

    public void SignUp(string id, string pw, Action onComplte = null)
    {
        string encryptedPassword = apiManager.CalcHMACSHA256(pw);

        apiManager.PostAsync(new SignUpCommand(onComplte), "/users",
            new JObject { { "userId", id }, { "password", encryptedPassword } });
    }

    public void SignIn(string id, string pw, Action onComplete = null)
    {
        string encryptedPassword = apiManager.CalcHMACSHA256(pw);
        apiManager.PostAsync(new SignInCommand(onComplete), string.Format("/users/{0}/login", id),
            new JObject {
                { "password", encryptedPassword },
                { "deviceModel", SystemInfo.deviceModel },
                { "deviceName", SystemInfo.deviceName },
                { "deviceType", SystemInfo.deviceType.ToString() },
                { "operatingSystem", SystemInfo.operatingSystem },
                { "batteryLevel", SystemInfo.batteryLevel },
                { "version", Application.version }
            });
    }

    public void SignInAuto(string id, string pw, Action onComplete = null)
    {
        string encryptedPassword = apiManager.CalcHMACSHA256(pw);
        apiManager.PostAsync(new SignInCommand(onComplete), string.Format("/users/{0}/login", id),
            new JObject {
                { "password", encryptedPassword },
                { "deviceModel", SystemInfo.deviceModel },
                { "deviceName", SystemInfo.deviceName },
                { "deviceType", SystemInfo.deviceType.ToString() },
                { "operatingSystem", SystemInfo.operatingSystem },
                { "batteryLevel", SystemInfo.batteryLevel },
                { "version", Application.version }
            });
    }

    public void GetUser(string userId)
    {
        apiManager.GetAsync(new GetUserCommand(), string.Format("/users/{0}", userId));
    }

    public void ModifyUserData(string id, JObject objects, Action onSuccess = null, Action<string> onFail = null)
    {
        apiManager.PutAsync(new PutUserDataCommand(onSuccess, onFail), string.Format("/users/{0}", id), objects);
    }

    public void ModifyPassword(string id, string pw)
    {
        string encryptedPassword = apiManager.CalcHMACSHA256(pw);
        apiManager.PutAsync(new ModifyPasswordCommand(), string.Format("/users/{0}/login", id),
            new JObject
            {
                { "password", encryptedPassword }
            });
    }
    private void ConnectSocket()
    {
        socket = new WebSocket(Address.DEVSOCKET + PlayerData.userId);
        socket.OnOpen += Socket_OnOpen;
        socket.OnClose += Socket_OnClose;
        socket.OnMessage += Socket_OnMessage;
        socket.OnError += Socket_OnError;
        socket.Connect();
    }
    public void DisconnectSocket()
    {
        try
        {
            if (socket == null) return;
            if (socket.IsAlive) socket.Close();
        }
        catch (Exception) { }
    }

    public bool CheckSocket()
    {
        return socket != null;
    }

    public void OnSignInSuccess(NetworkMessage message)
    {
        //Debug.Log("OnSignInSuccess : " + message.body);
        //
        string userID = (string)(JObject.Parse(message.body)["userId"]);

        PlayerData.userId = userID;
        //웹소켓 접속
        ConnectSocket();

        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnSuccessLogin(PlayerData);
        }
    }

    public void OnSignInFailed(NetworkMessage message)
    {
        //Debug.Log("OnSignInFailed : " + message.body);
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnFailLogin(message.body.ToString());
        }
    }

    public void OnSignUpSuccess(NetworkMessage message)
    {
        //Debug.Log("OnSignUpSuccess : " + message.body);

        PlayerData.userId = JObject.Parse(message.body)["userId"].ToString();
        ConnectSocket();

        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnSuccessSignUp();
        }
    }

    public void OnSignUpFailed(NetworkMessage message)
    {
        //Debug.Log("OnSignUpFailed : " + message.body);
        JObject values = JObject.Parse(message.body);

        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnFailSignUp(values["error"].ToString());
        }
    }

    public void OnGetUserSuccess(NetworkMessage message)
    {
        LocalPlayerData playerData = JsonUtility.FromJson<LocalPlayerData>(message.body);
        this.PlayerData.userId = playerData.userId;
        this.PlayerData.userName = playerData.userName;
        this.PlayerData.profile = playerData.profile;
        this.PlayerData.SetCustom(playerData.GetCustomAvatar());
        this.PlayerData.coin = playerData.coin;
        this.PlayerData.heart = playerData.heart;
        this.PlayerData.myRoomId = playerData.myRoomId;
        this.PlayerData.connectionId = playerData.connectionId;
        this.PlayerData.description = playerData.description;
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnSuccessGetUser(PlayerData);
        }
    }

    public void OnGetUserFailed(NetworkMessage message)
    {
        //Debug.Log("OnGetUserFailed : " + message.body);
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnFailGetUser(message.body);
        }
    }

    private void Socket_OnOpen(object sender, EventArgs e)
    {
        Debug.Log("Socket_OnOpen");
        GetUser(PlayerData.userId);
        LastSockSendTime = DateTime.Now;
        GameManager.Instance.StartCoroutine(CheckLastSocketSend());
    }

    private void Socket_OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("Socket_OnClose");
        DisconnectSocket();

        GameManager.Instance.StopCoroutine(CheckLastSocketSend());
    }

    private void Socket_OnMessage(object sender, MessageEventArgs e)
    {
        string msg = e.Data;
        if (msg.IsNullOrEmpty()) return;
        string action = (string)(JObject.Parse(msg)["action"]);
        InviteDats invite = JsonUtility.FromJson<InviteDats>(msg);

        if (!invite.action.IsNullOrEmpty() && invite.action == "invite")
        {
            ReceiveInvite(invite);
        }

        if(action == "pushAlarm")
        {
            string pushAlarms = (string)(JObject.Parse(msg)["pushAlarms"]);
            Alarm alarm = JsonUtility.FromJson<Alarm>(pushAlarms);
            NotifyView notify = UIView.Get<NotifyView>();
            foreach (var notifyData in alarm.pushAlarms)
            {
                notifyData.showingTime = 3f;
                notify.PushNotify(notifyData);
            }
        }

        /*
         {
            "action": "invite",
            "roomId": "...."
         }

         {
            "action": "pushAlarm",
            "pushAlarms":[ .. ]
         }
        */
    }

    private void ReceiveInvite(InviteDats invite)
    {
        Debug.Log("ReceiveInvite1 // invite.roomId :" + invite.roomId + "invite.roomName :" + invite.roomName);
        apiManager.GetAsync(new GetOhterUserCommand
        ((message) =>
          {
              string userName = JsonUtility.FromJson<LocalPlayerData>(message).userName;
              string thumbnail = string.Format("users/{0}/{0}.jpg", invite.userId);
              apiManager.DownLoadTexture(thumbnail, (sprite) =>
              {
                  NotifyData notifyData = new NotifyData(5f, userName, invite.roomName, sprite, null, null,
                  () =>
                  {
                      Debug.Log("ReceiveInvite2 // invite.roomId :" + invite.roomId + "invite.roomName :" + invite.roomName);
                      roomDataManager.RoomAPIHandler.JoinRoom(invite.roomId, invite.action, null, FailedJoinRoom);
                  }
                  );

                  NotifyView notify = UIView.Get<NotifyView>();
                  notify.PushNotify(notifyData);
              });
          },
            null), string.Format("/users/{0}", invite.userId)
        );
    }

    private void FailedJoinRoom(string message)
    {
        GameManager.Instance.Persistent.UIManager.PushNotify(message, 3f);
    }

    private void Socket_OnError(object sender, ErrorEventArgs e)
    {
        //Debug.Log("Socket_OnError: " + e.Message);
    }

    public void OnPutUserNameSuccess(NetworkMessage message)
    {
        //Debug.Log("ttt OnPutUserNameSuccess");
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnSuccessGetUser(PlayerData);
        }
    }

    public void OnPutUserNameFailed(NetworkMessage message)
    {
        //Debug.Log("ttt OnPutUserNameFailed");
    }

    public void OnModifyCommandSuccess(NetworkMessage message)
    {

    }

    public void OnModifyCommandFailed(NetworkMessage message)
    {

    }

    public class InviteDats
    {
        public string userId;
        public string roomId;
        public string roomName;
        public string connectionId;
        public string action;
    }

    public void RequestInvite(PeopleData responseData)
    {
        Debug.Log("RequestInvite // MyData.roomId :" + PlayerData.myRoomId + "invite.roomName :" + roomDataManager.Current.roomName);

        JObject inviteData = new JObject();
        inviteData["userId"] = PlayerData.userId;  // (자신)초대 보내는 사람의 userId
        inviteData["roomId"] = roomDataManager.Current.roomId;  // (자신)초대 보내는 사람이 속한 방의 roomId
        inviteData["roomName"] = roomDataManager.Current.roomName;  // (자신)초대 보내는 사람이 속한 방의 roomName
        inviteData["connectionId"] = responseData.connectionId;  // 초대 받는 사람의 connectionId

        JObject request = new JObject();
        request["action"] = "invite";
        request["data"] = inviteData.ToString();
        SendSocket(request.ToString());
    }

    public void SendSocket(string data)
    {
        LastSockSendTime = DateTime.Now;
        socket.Send(data);
    }
    private IEnumerator CheckLastSocketSend()
    {
        while (true)
        {
            if ((DateTime.Now - LastSockSendTime).TotalMinutes >= 1)
            {
                SendSocket(PlayerData.userId);
            }
            yield return new WaitForSeconds(1);
        }
    }
}
