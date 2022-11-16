using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthNetCommand : NetCommand { }

public class PostHealthDataNetCommand : HealthNetCommand
{
    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnSuccessPostHealth(NetworkMessage message);
        void OnFailedPostHealth(NetworkMessage message);
    }
    public override string ID => "PostHealthDataNetCommand";

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnFailedPostHealth(message);
        }
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnSuccessPostHealth(message);
        }
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
}
