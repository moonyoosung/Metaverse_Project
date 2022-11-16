using MindPlus;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class EventContentData : ContentData
{
    public string hostUserId;
    public int numPlayers;
    public Action<int> onNumPlayerChanged;
    public string openRoomTime;

    public EventContentData(JObject jObject) : base(jObject)
    {
        this.hostUserId = (string)jObject["hostUserId"];
        this.numPlayers = (int)jObject["numPlayers"];
        this.openRoomTime = DateTime.Parse((string)jObject["openRoomTime"]).ToString(Format.DateFormat);
    }
    public override bool IsAvailable()
    {
        if (numPlayers >= maxPlayers)
        {
            return false;
        }

        if (!IsOpen())
        {
            return false;
        }

        return base.IsAvailable();
    }
    public bool IsOpen()
    {
        DateTime now = DateTime.Now;
        DateTime openTime = GetTime();

        TimeSpan timeCal = openTime - now;

        if (timeCal.TotalMinutes > 0)
        {
            return false;
        }

        return true;
    }
    public DateTime GetTime()
    {
        return DateTime.ParseExact(openRoomTime, Format.DateFormat, GameManager.Instance.CultureInfo);
    }
    public override void CompareData(ContentData target)
    {
        base.CompareData(target);
        EventContentData eventTarget = target as EventContentData;
        if (this.numPlayers != eventTarget.numPlayers)
        {
            this.numPlayers = eventTarget.numPlayers;
            onNumPlayerChanged?.Invoke(numPlayers);
        }
    }
}
