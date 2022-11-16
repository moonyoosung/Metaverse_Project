using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using MindPlus;
using Newtonsoft.Json.Linq;
/*
myroom#public: 마이룸                      -> myroompublic           [location]
commcenter#public: 커뮤니티센터 공용맵      -> commcenterpublic       [location]
park#public: 공원 공용맵                   -> parkpublic             [location]
event#public: 이벤트룸 (공개방)            -> eventpublic            [event]
event#private: 이벤트룸 (비공개방)         -> eventprivate           [event]
{gameId}#public: 플레이 룸 (공개방)        -> playerpublic           [play]  
플레이 룸의 경우, 각 게임 Id가 Type이 됨    -> playerprivate          [play]
{gameId}#public: 플레이 룸 (비공개방)
*/


[System.Serializable]
public class ContentData : IEquatable<ContentData>
{
    public string roomType;
    public int numLikes;
    public string roomName;
    public string roomId;
    public string sceneName;
    public string thumbnail;
    public int maxPlayers;
    public bool isOpen;
    public string description;

    public ContentData(JObject jObject)
    {
        this.roomType = (string)jObject["roomType"];
        this.roomName = (string)jObject["roomName"];
        this.roomId = (string)jObject["roomId"];
        if (roomId != "myroom" && jObject["numLikes"] != null)
        {
            this.numLikes = (int)jObject["numLikes"];
        }
        this.sceneName = (string)jObject["sceneName"];
        this.thumbnail = (string)jObject["thumbnail"];
        this.maxPlayers = (int)jObject["maxPlayers"];
        this.isOpen = jObject["isOpen"] == null ? true : (bool)jObject["isOpen"];
        this.description = jObject["description"] == null ? "" : (string)jObject["description"];
    }

    //#Private 옵션 생기면 다시 정의

    public string GetRoomType(string roomId = "")
    {
        //location
        if (roomType.Contains(ContentTypes.Location))
        {
            return sceneName.ToLower() + "#public";
        }

        //play
        if (roomType.Contains(ContentTypes.Play))
        {
            return roomId + "#public";
        }

        //event
        return roomType;

    }

    public bool Equals(ContentData other)
    {
        return this.roomId == other.roomId;
    }
    public virtual void CompareData(ContentData target)
    {

    }

    public virtual bool IsAvailable()
    {
        return true;
    }
}
