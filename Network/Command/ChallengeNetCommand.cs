using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChallengeNetCommand : NetCommand { }

public class GetChallengeCommand : ChallengeNetCommand
{
    private bool notEvent;
    public override string ID => "GetChallengeCommand";

    public GetChallengeCommand(bool notEvent = false)
    {
        this.notEvent = notEvent;
    }
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnSuccessGetChallenges(NetworkMessage message, bool notEvent);
        void OnFailedGetChallenges(NetworkMessage message);
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnFailedGetChallenges(message);
        }
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnSuccessGetChallenges(message, notEvent);
        }
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
}
public class GetRewardCommand : ChallengeNetCommand
{
    private bool notEvent;
    public GetRewardCommand(bool notEvent = false)
    {
        this.notEvent = notEvent;
    }
    public override string ID => "GetActivityCommand";
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnSuccessReward(NetworkMessage message, bool notEvent);
        void OnFailedReward(NetworkMessage message);
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnFailedReward(message);
        }
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnSuccessReward(message, notEvent);
        }
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
}
public class PostAchieveActivityCommand : ChallengeNetCommand
{
    public override string ID => "PostActivityCommand";

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnSuccessArchieveActivity(NetworkMessage message);
        void OnFailedArchieveActivity(NetworkMessage message);
    }

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnFailedArchieveActivity(message);
        }
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnSuccessArchieveActivity(message);
        }
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
}