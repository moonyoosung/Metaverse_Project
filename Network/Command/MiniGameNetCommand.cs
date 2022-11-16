using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGameNetCommand : NetCommand { }

public class PostRewardsCommand : MiniGameNetCommand
{
    public override string ID => "PostRewardsCommand";
    private Action onComplete;
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnSignInSuccess(NetworkMessage message);
        void OnSignInFailed(NetworkMessage message);
    }
    public PostRewardsCommand(Action onComplete = null)
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