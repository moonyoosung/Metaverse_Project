using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using MindPlus;
using Newtonsoft.Json.Linq;
/*
myroom#public: ���̷�                      -> myroompublic           [location]
commcenter#public: Ŀ�´�Ƽ���� �����      -> commcenterpublic       [location]
park#public: ���� �����                   -> parkpublic             [location]
event#public: �̺�Ʈ�� (������)            -> eventpublic            [event]
event#private: �̺�Ʈ�� (�������)         -> eventprivate           [event]
{gameId}#public: �÷��� �� (������)        -> playerpublic           [play]  
�÷��� ���� ���, �� ���� Id�� Type�� ��    -> playerprivate          [play]
{gameId}#public: �÷��� �� (�������)
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

    //#Private �ɼ� ����� �ٽ� ����

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
