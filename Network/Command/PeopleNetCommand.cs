using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PeopleNetCommand : NetCommand { }


public class GetFriendListCommand : PeopleNetCommand
{
    public override string ID => "GetFriendListCommand";
    Action<string> onSuccess;
    Action<string> onFail;

    public GetFriendListCommand(Action<string> onSuccess = null, Action<string> onFail = null)
    {
        this.onSuccess = onSuccess;
        this.onFail = onFail;
    }

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnGetFriendListSuccess(NetworkMessage message);
        void OnGetFriendListFailed(NetworkMessage message);
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetFriendListSuccess(message);
        }
        onSuccess?.Invoke(message.body);
    }

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetFriendListFailed(message);
        }
        onFail?.Invoke(message.body);
    }
}

public class GetRequestFriendListCommand : PeopleNetCommand
{
    public override string ID => "GetRequestFriendListCommand";
    Action<string> onSuccess;
    Action<string> onFail;

    public GetRequestFriendListCommand(Action<string> onSuccess = null, Action<string> onFail = null)
    {
        this.onSuccess = onSuccess;
        this.onFail = onFail;
    }

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnGetRequestFriendListSuccess(NetworkMessage message);
        void OnGetRequestFriendListFailed(NetworkMessage message);
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetRequestFriendListSuccess(message);
        }
        onSuccess?.Invoke(message.body);
    }

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetRequestFriendListFailed(message);
        }
        onFail?.Invoke(message.body);
    }
}

public class PostRequestFriendCommand : PeopleNetCommand
{
    public override string ID => "PostRequestFriendCommand";

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnPostRequestFriendSuccess(NetworkMessage message);
        void OnPostRequestFriendFailed(NetworkMessage message);
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnPostRequestFriendSuccess(message);
        }
    }

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnPostRequestFriendFailed(message);
        }
    }
}

public class PostAcceptFriendCommand : PeopleNetCommand
{
    public override string ID => "PostAcceptFriendCommand";

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnPostAcceptFriendSuccess(NetworkMessage message);
        void OnPostAcceptFriendFailed(NetworkMessage message);
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnPostAcceptFriendSuccess(message);
        }
    }

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnPostAcceptFriendFailed(message);
        }
    }
}

public class DelFriendCommand : PeopleNetCommand
{
    public override string ID => "DelFriendCommand";
    private Action onSuccess;
    private Action<string> onFail;

    public DelFriendCommand(Action onSuccess, Action<string> onFail)
    {
        this.onSuccess = onSuccess;
        this.onFail = onFail;
    }
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnDelFriendSuccess(NetworkMessage message);
        void OnDelFriendFailed(NetworkMessage message);
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnDelFriendSuccess(message);
        }
        onSuccess?.Invoke();
    }

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnDelFriendFailed(message);
        }
        onFail?.Invoke(message.body);
    }
}


public class DelRequestFriendCommand : PeopleNetCommand
{
    public override string ID => "DelRequestFriendCommand";

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnDelRequestFriendSuccess(NetworkMessage message);
        void OnDelRequestFriendFailed(NetworkMessage message);
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnDelRequestFriendSuccess(message);
        }
    }

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnDelRequestFriendFailed(message);
        }
    }
}

public class GetLocationCommand : PeopleNetCommand
{
    public override string ID => "GetLocationCommand";
    private Action<string> onSuccess;
    private Action<string> onFail;

    public GetLocationCommand(Action<string> onSuccess = null, Action<string> onFail = null)
    {
        this.onSuccess = onSuccess;
        this.onFail = onFail;
    }

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnGetLocationSuccess(NetworkMessage message);
        void OnGetLocationFailed(NetworkMessage message);
    }
    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetLocationFailed(message);
        }
        onFail?.Invoke(message.body);
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetLocationSuccess(message);
        }
        onSuccess?.Invoke(message.body);
    }
}