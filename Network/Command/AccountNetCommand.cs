using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AccountNetCommand : NetCommand { }

public class GetUserCommand : AccountNetCommand
{
    public override string ID => "GetUserCommand";

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnGetUserSuccess(NetworkMessage message);
        void OnGetUserFailed(NetworkMessage message);
    }
    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetUserFailed(message);
        }
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetUserSuccess(message);
        }
    }
}
public class GetOhterUserCommand : AccountNetCommand
{
    public override string ID => "GetUserCommand";
    private Action<string> onSuccess;
    private Action<string> onFail;

    public GetOhterUserCommand(Action<string> onSuccess, Action<string> onFail)
    {
        this.onSuccess = onSuccess;
        this.onFail = onFail;
    }

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnGetOhterUserSuccess(NetworkMessage message);
        void OnGetOhterUserFailed(NetworkMessage message);
    }
    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetOhterUserFailed(message);
        }
        onFail?.Invoke(message.body);
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetOhterUserSuccess(message);
        }
        onSuccess?.Invoke(message.body);
    }
}
public class SignUpCommand : AccountNetCommand
{
    public override string ID => "SignUpCommand";
    private Action onComplete;
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnSignUpSuccess(NetworkMessage message);
        void OnSignUpFailed(NetworkMessage message);
    }
    public SignUpCommand(Action onComplete = null)
    {
        this.onComplete = onComplete;
    }
    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnSignUpSuccess(message);
        }
        onComplete?.Invoke();
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnSignUpFailed(message);
        }
        onComplete?.Invoke();
    }
}
public class SignInCommand : AccountNetCommand
{
    public override string ID => "SignUpCommand";
    private Action onComplete;
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnSignInSuccess(NetworkMessage message);
        void OnSignInFailed(NetworkMessage message);
    }
    public SignInCommand(Action onComplete = null)
    {
        this.onComplete = onComplete;
    }
    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnSignInSuccess(message);
        }
        onComplete?.Invoke();
    }

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnSignInFailed(message);
        }
        onComplete?.Invoke();
    }
}
public class PutUserDataCommand : AccountNetCommand
{
    public override string ID => "PutUserNameCommand";
    private Action onSuccess;
    private Action<string> onFail;

    public PutUserDataCommand(Action onSuccess, Action<string> onFail)
    {
        this.onSuccess = onSuccess;
        this.onFail = onFail;
    }

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnPutUserNameSuccess(NetworkMessage message);
        void OnPutUserNameFailed(NetworkMessage message);
    }
    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnPutUserNameSuccess(message);
        }
        onSuccess?.Invoke();
    }

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnPutUserNameFailed(message);
        }
        onFail?.Invoke(message.body);
    }
}

public class ModifyPasswordCommand : AccountNetCommand
{
    public override string ID => "ModifyPasswordCommand";
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnModifyCommandSuccess(NetworkMessage message);
        void OnModifyCommandFailed(NetworkMessage message);
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnModifyCommandFailed(message);
        }
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnModifyCommandSuccess(message);
        }
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
}