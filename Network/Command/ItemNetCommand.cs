using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemNetCommand : NetCommand{}

public class GetItemListCommnad : ItemNetCommand
{
    public override string ID => "GetItemListCommnad";

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnGetItemListSuccess(NetworkMessage message);
        void OnGetItemListFailed(NetworkMessage message);
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetItemListSuccess(message);
        }
    }

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetItemListFailed(message);
        }
    }
}

public class PutUserCustomDataCommand : ItemNetCommand
{
    public override string ID => "PutUserCustomCommand";

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnPutUserCustomSuccess(NetworkMessage message);
        void OnPutUserCustomFailed(NetworkMessage message);
    }
    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnPutUserCustomSuccess(message);
        }
    }

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnPutUserCustomFailed(message);
        }
    }
}

public class GetUserCustomCommand : ItemNetCommand
{
    public override string ID => "GetUserCommand";

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnGetUserCustomSuccess(NetworkMessage message);
        void OnGetUserCustomFailed(NetworkMessage message);
    }
    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }
    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetUserCustomFailed(message);
        }
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnGetUserCustomSuccess(message);
        }
    }
}
public class PostPurchaseItemCommand : ItemNetCommand
{
    public override string ID => "PostPurchaseItemCommand";

    public interface IEventHandler : APIManager.IEventHandler
    {
        void OnPostPurchaseItemSuccess(NetworkMessage message);
        void OnPostPurchaseItemFailed(NetworkMessage message);
    }

    public override bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler)
    {
        return eventHandler is IEventHandler;
    }

    public override void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnPostPurchaseItemSuccess(message);
        }
    }

    public override void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message)
    {
        foreach (var eventHandler in eventHandlers)
        {
            (eventHandler as IEventHandler)?.OnPostPurchaseItemFailed(message);
        }
    }
}
