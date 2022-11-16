using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomNetCommand : NetCommand { }

public class LeaveRoomCommand : RoomNetCommand
{
    public override string ID => "LeaveRoomCommand"; 
    private Action onSuccess;

    public LeaveRoomCommand(Action onSuccess)
    {
        this.onSuccess = onSuccess;
    }

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnLeaveRoomSuccess(NetworkMessage message);
        void OnLeaveRoomFailed(NetworkMessage message);
    }
    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnLeaveRoomFailed(message);
        }
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnLeaveRoomSuccess(message);
        }
        onSuccess?.Invoke();
    }
}

public class JoinRoomAutoCommand : RoomNetCommand
{
    public override string ID => "JoinRoomAutoCommand";
    private Action<string> onError;
    private Action onSuccess;
    public JoinRoomAutoCommand(Action onSuccess, Action<string> onError)
    {
        this.onError = onError;
        this.onSuccess = onSuccess;
    }

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnJoinRoomAutoSuccess(NetworkMessage message);
        void OnJoinRoomAutoFailed(NetworkMessage message);
    }
    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnJoinRoomAutoFailed(message);
        }
        onError?.Invoke(message.body);
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnJoinRoomAutoSuccess(message);
        }
        onSuccess?.Invoke();
    }
}
public class JoinRoomCommand : RoomNetCommand
{
    public override string ID => "JoinRoomCommand";
    private Action<string> onError;
    private Action onSuccess;
    public JoinRoomCommand(Action onSuccess, Action<string> onError)
    {
        this.onError = onError;
        this.onSuccess = onSuccess;
    }
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnJoinRoomSuccess(NetworkMessage message);
        void OnJoinRoomFailed(NetworkMessage message);
    }
    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnJoinRoomFailed(message);
        }
        onError?.Invoke(message.body);
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnJoinRoomSuccess(message);
        }
        onSuccess?.Invoke();
    }
}
public class GetContentListCommand : RoomNetCommand
{
    public override string ID => "GetContentListCommand";
    public string category = "";
    private Action<string> onExcute;

    public GetContentListCommand(string category = "", Action<string> onExcute = null)
    {
        this.category = category;
        this.onExcute = onExcute;
    }
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnGetContentListSuccess(NetworkMessage message, string category);
        void OnGetContentListFailed(NetworkMessage message, string category);
    }
    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetContentListFailed(message, category);
        }
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetContentListSuccess(message, category);
        }
        onExcute?.Invoke(message.body);
    }
}
public class GetRoomCommand : RoomNetCommand
{
    public override string ID => "GetRoomCommand";

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnGetRoomSuccess(NetworkMessage message);
        void OnGetRoomFailed(NetworkMessage message);
    }
    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetRoomFailed(message);
        }
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetRoomSuccess(message);
        }
    }
}
public class CreateRoomCommand : RoomNetCommand
{
    public override string ID => "GetRoomCommand";
    private Action<string> onSuccess;
    private Action<string> onFail;

    public CreateRoomCommand(Action<string> onSuccess, Action<string> onFail)
    {
        this.onSuccess = onSuccess;
        this.onFail = onFail;
    }
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnCreateRoomSuccess(NetworkMessage message);
        void OnCreateRoomFailed(NetworkMessage message);
    }
    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnCreateRoomFailed(message);
        }
        onFail?.Invoke(message.body);
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnCreateRoomSuccess(message);
        }
        onSuccess?.Invoke(JObject.Parse(message.body)["roomId"].ToString());
    }
}

public class GetLikeCommand : RoomNetCommand
{
    public override string ID => "GetLikeCommand";
    private Action<string> onSuccess;
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnGetLikeFailed(NetworkMessage message);
        void OnGetLikeSuccess(NetworkMessage message);
    }
    public GetLikeCommand(Action<string> onSuccess = null)
    {
        this.onSuccess = onSuccess;
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetLikeFailed(message);
        }
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetLikeSuccess(message);
        }
        onSuccess?.Invoke(message.body);
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
}

public class PostLikeCommand : RoomNetCommand
{
    public override string ID => "PostLikeCommand";
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnPostLikeSuccess(NetworkMessage message);
        void OnPostLikeFailed(NetworkMessage message);
    }

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnPostLikeFailed(message);
        }
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnPostLikeSuccess(message);
        }
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
}

public class DeleteLikeCommand : RoomNetCommand
{
    public override string ID => "DeleteLikeCommand";
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnDeleteLikeFailed(NetworkMessage message);
        void OnDeleteLikeSuccess(NetworkMessage message);
    }

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnDeleteLikeFailed(message);
        }
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnDeleteLikeSuccess(message);
        }
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
}

public class ModifyRoomCommand : RoomNetCommand
{
    public override string ID => "ModifyRoomCommand";
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnModifyRoomSuccess(NetworkMessage networkMessage);
        void OnModifyRoomFailed(NetworkMessage networkMessage);
    }


    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnModifyRoomFailed(message);
        }
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnModifyRoomSuccess(message);
        }
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;

    }
}
