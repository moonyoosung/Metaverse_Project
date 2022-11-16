using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MindPlus;
using System;

public class RoomAPIHandler : JoinRoomCommand.IEventHandler, GetRoomCommand.IEventHandler, GetContentListCommand.IEventHandler,
    LeaveRoomCommand.IEventHandler, CreateRoomCommand.IEventHandler, JoinRoomAutoCommand.IEventHandler
{
    private APIManager apiManager;
    private AccountManager accountManager;
    private RoomDataBaseManager roomDataBaseManager;
    public RoomAPIHandler(RoomDataBaseManager roomDataBaseManager, APIManager apiManager, AccountManager accountManager)
    {
        this.apiManager = apiManager;
        this.accountManager = accountManager;
        this.roomDataBaseManager = roomDataBaseManager;

        apiManager.ResisterEvent(this);
    }
    public void GetLikeRoom(string roomID, Action<string> onSuccess = null)
    {
        apiManager.GetAsync(new GetLikeCommand(onSuccess), string.Format("/rooms/{0}/like?user_id={1}", roomID, accountManager.PlayerData.userId));
    }
    public void PostLikeRoom(string roomID)
    {
        apiManager.PostAsync(new PostLikeCommand(), string.Format("/rooms/{0}/like", roomID),
            new JObject { { "userId", accountManager.PlayerData.userId } });
    }
    public void DeleteLikeRoom(string roomID)
    {
        apiManager.DeleteAsync(new DeleteLikeCommand(), string.Format("/rooms/{0}/like", roomID),
            new JObject { { "userId", accountManager.PlayerData.userId } });
    }
    public void GetContentsList(string roomType, string category = "", Action<string> onExcute = null)
    {
        if (category != "")
        {
            if (category.Equals("myevent"))
            {
                apiManager.GetAsync(new GetContentListCommand(category, onExcute),
                    string.Format("/rooms?type={0}&category={1}&userid={2}", roomType, category, accountManager.PlayerData.userId));
            }
            else
            {
                apiManager.GetAsync(new GetContentListCommand(category, onExcute),
                    string.Format("/rooms?type={0}&category={1}", roomType, category));
            }
        }
        else
        {
            apiManager.GetAsync(new GetContentListCommand(), string.Format("/rooms?type={0}", roomType));
        }
    }
    public void GetRoom(string roomID)
    {
        apiManager.GetAsync(new GetRoomCommand(), string.Format("/rooms/{0}", roomID));
    }
    public void ModifyRoom(string roomID, JObject requestBody)
    {
        apiManager.PutAsync(new ModifyRoomCommand(), string.Format("/rooms/{0}", roomID), requestBody);
    }

    public void JoinRoom(string roomID, string roomType, Action onSuccess = null, Action<string> onError = null)
    {
        apiManager.PostAsync(new JoinRoomCommand(onSuccess, onError), string.Format("/rooms/{0}/join", roomID),
            new JObject {
                { "userId", accountManager.PlayerData.userId },
                { "connectionId", accountManager.PlayerData.connectionId },
                { "roomType", roomType }
            });
    }
    public void JoinRoomAuto(ContentData roomData, Action onSuccess = null, Action<string> onError = null)
    {
        Debug.Log("JoinRoomAuto : " + roomData.roomId);
        apiManager.PostAsync(new JoinRoomAutoCommand(onSuccess, onError), string.Format("/rooms/{0}/join", roomData.roomId),
            new JObject
            {
                { "userId", accountManager.PlayerData.userId },
                { "connectionId", accountManager.PlayerData.connectionId },
                { "roomType", roomData.GetRoomType(roomData.roomId) },
                { "roomName", roomData.roomName },
                { "maxPlayers", roomData.maxPlayers.ToString() },
                { "sceneName", roomData.sceneName }
            });
    }
    public void LeaveRoom(ContentData contentData, Action onSuccess = null)
    {
        Debug.Log("LeaveRoom");
        apiManager.DeleteAsync(new LeaveRoomCommand(onSuccess), string.Format("/rooms/{0}/join", contentData.roomId),
            new JObject
            {
                { "userId", accountManager.PlayerData.userId },
                { "roomType", contentData.roomType }
            });
    }

    public void CreateRoom(string roomName, string type, int maxPlayers, string sceneName, string description,
        string openRoomTime = null, Action<string> onComplete = null, Action<string> OnFail = null)
    {
        JObject requestBody = new JObject();
        requestBody.Add("roomName", roomName);
        requestBody.Add("roomType", "event#public");
        requestBody.Add("hostUserId", accountManager.PlayerData.userId);
        requestBody.Add("sceneName", sceneName);
        requestBody.Add("maxPlayers", maxPlayers);
        requestBody.Add("description", description);
        if (openRoomTime != null)
            requestBody.Add("openRoomTime", openRoomTime);

        apiManager.PostAsync(new CreateRoomCommand(onComplete, OnFail), "/rooms", requestBody);
    }

    public void OnGetRoomSuccess(NetworkMessage message)
    {
        //Debug.Log("OnGetRoomSuccess : " + message.body);
        JObject jObject = JObject.Parse(message.body);

        roomDataBaseManager.OnStartJoin(CreateContentData(jObject));
    }

    public void OnGetRoomFailed(NetworkMessage message)
    {
        //Debug.Log("OnGetRoomFailed : " + message.body);
    }

    public void OnGetContentListSuccess(NetworkMessage message, string category)
    {
        //Debug.Log(category + ", " + "OnGetRoomListSuccess : " + message.body);
        JObject jObject = JObject.Parse(message.body);

        // 카테고리에 중복되는 ContentData가 있을 때 다른쪽 카테고리에는 들어가지 않음
        foreach (var contentData in ConvertContents(jObject))
        {
            roomDataBaseManager.OnADDContent(contentData, category);
        }
    }
    public List<ContentData> ConvertContents(JObject jobject)
    {
        List<ContentData> contents = new List<ContentData>();
        foreach (var item in jobject["Items"])
        {
            ContentData roomData = CreateContentData((JObject)item);
            contents.Add(roomData);
        }
        return contents;
    }

    public void OnGetContentListFailed(NetworkMessage message, string category)
    {
        //Debug.Log("OnGetRoomListFailed : " + message.body);
    }

    public void OnLeaveRoomSuccess(NetworkMessage message)
    {
        //Debug.Log("OnLeaveRoomSuccess : " + message.body);
        roomDataBaseManager.Current = null;
    }

    public void OnLeaveRoomFailed(NetworkMessage message)
    {
        //Debug.Log("OnLeaveRoomFailed : " + message.body);
    }

    public void OnCreateRoomSuccess(NetworkMessage message)
    {
        Debug.Log("OnCreateRoomSuccess : " + message.body);
    }

    public void OnCreateRoomFailed(NetworkMessage message)
    {
        //Debug.Log("OnCreateRoomFailed : " + message.body);
    }


    public ContentData CreateContentData(JObject data)
    {
        string roomType = (string)data["roomType"];
        Debug.Log("CreateContentDataCreateContentDataCreateContentData2" + roomType);
        if (roomType.Contains(ContentTypes.Location))
        {
            Debug.Log("CreateContentDataCreateContentDataCreateContentData3" + roomType);
            return new LocationContentData(data);
        }
        else if (roomType.Contains(ContentTypes.Play))
        {
            return new PlayContentData(data);
        }
        else if (roomType.Contains(ContentTypes.Event))
        {
            return new EventContentData(data);
        }
        else
        {
            return new ContentData(data);
        }
    }

    public void OnJoinRoomSuccess(NetworkMessage message)
    {
        //Debug.Log("OnJoinRoomSuccess : " + message.body);

        var jObject = JObject.Parse(message.body);
        GetRoom((string)jObject["roomId"]);
    }
    public void OnJoinRoomFailed(NetworkMessage message)
    {
        //Debug.Log("OnJoinRoomFailed : " + message.body);
    }

    public void OnJoinRoomAutoSuccess(NetworkMessage message)
    {
        //Debug.Log("OnJoinRoomAutoSuccess : " + message.body);

        var jObject = JObject.Parse(message.body);
        GetRoom((string)jObject["roomId"]);
    }

    public void OnJoinRoomAutoFailed(NetworkMessage message)
    {
        //Debug.Log("OnJoinRoomAutoFailed : " + message.body);
    }
}
