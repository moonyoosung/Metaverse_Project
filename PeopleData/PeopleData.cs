using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using MindPlus;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class PeopleData : IEquatable<PeopleData>
{
    public string userId;
    public string userName;
    public string profile;
    public string thumbnail;
    [SerializeField]
    public string heart;
    public string coin;
    public string myRoomId;
    public string connectionId;
    public string description;

    public PeopleData(JObject jObject)
    {
        this.userId = (string)jObject["userId"];
        this.myRoomId = (string)jObject["myRoomId"];
        this.heart = (string)jObject["heart"];
        this.userName = (string)jObject["userName"];
        this.coin = (string)jObject["coin"];
        this.connectionId = (string)jObject["connectionId"];
        this.description = (string)jObject["description"]; 
        this.thumbnail = !string.IsNullOrEmpty(this.userId) ? string.Format("users/{0}/{0}.jpg", this.userId) : null;
        //this.custom = new CustomAvatar();
        //this.custom = JsonUtility.FromJson<CustomAvatar>(jObject["custom"].ToString());
    }

    public PeopleData(string userId)
    {
        this.userId = userId;
    }

    public PeopleData(LocalPlayerData data)
    {
        this.userId = data.userId;
        this.myRoomId = data.myRoomId;
        this.heart = data.heart.ToString();
        this.userName = data.userName;
        this.coin = data.coin.ToString();
        this.connectionId = data.connectionId;
        this.description = data.description;
        this.thumbnail = !string.IsNullOrEmpty(this.userId) ? string.Format("users/{0}/{0}.jpg", this.userId) : null;
        //this.custom = new CustomAvatar();
        //this.custom = data.custom;
    }

    public void SetPeopleData(JObject jObject, bool isMySelf = false)
    {
        string.IsNullOrEmpty((string)jObject["userId"]);
        this.userId = (string)jObject["userId"] != null ? (string)jObject["userId"] : this.userId;
        this.myRoomId = (string)jObject["myRoomId"] != null ? (string)jObject["myRoomId"] : this.myRoomId;
        this.heart = (string)jObject["heart"] != null ? (string)jObject["heart"] : this.heart;
        this.userName = (string)jObject["userName"] != null ? (string)jObject["userName"] : this.myRoomId;
        this.coin = (string)jObject["coin"] != null ? (string)jObject["coin"] : this.heart;
        this.connectionId = (string)jObject["connectionId"] != null ? (string)jObject["connectionId"] : this.connectionId;
        this.description = (string)jObject["description"] != null ? (string)jObject["description"] : this.description;
        this.thumbnail = !string.IsNullOrEmpty(this.userId) ? string.Format("users/{0}/{0}.jpg", this.userId) : null;
        //this.custom = new CustomAvatar();
        //this.custom = !string.IsNullOrEmpty(jObject["custom"].ToString()) ? JsonUtility.FromJson<CustomAvatar>(jObject["custom"].ToString()) : this.custom;
    }

    public bool Equals(JObject other)
    {
        return this.userId == (string)other["userId"];
    }

    public bool Equals(PeopleData other)
    {
        return this.userId == other.userId;
    }

    public bool IsOnline()
    {
        return !(string.IsNullOrEmpty(this.connectionId));
    }

    public virtual bool IsAvailable()
    {
        return true;
    }
}
