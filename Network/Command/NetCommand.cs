using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
public abstract class NetCommand
{
    public abstract string ID { get; }
    public abstract bool IsCompareIEventHandler(APIManager.IEventHandler eventHandler);
    public abstract void Execute(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message);
    public abstract void Error(List<APIManager.IEventHandler> eventHandlers, NetworkMessage message);
}

