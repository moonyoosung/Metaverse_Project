using MindPlus;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


public class ChallengeManager : GetChallengeCommand.IEventHandler, PostAchieveActivityCommand.IEventHandler, GetRewardCommand.IEventHandler, AccountManager.IEventHandler
{
    public interface IEventHandler
    {
        void OnGetChallenges(ChallengeList data);
        void OnGetActivities(ActivityData data);
    }
    private readonly List<IEventHandler> eventHandlers = new List<IEventHandler>();
    private APIManager aPIManager;
    private LocalPlayerData playerData;
    public ChallengeList ChallangeList;
    public ActivityData activityData;
    private Color inAppColor;
    private Color outDoorColor;
    public void Initialize(APIManager aPIManager, AccountManager accountManager, UIResources uIResources)
    {
        this.aPIManager = aPIManager;
        this.playerData = accountManager.PlayerData;
        this.inAppColor = uIResources.inappColor;
        this.outDoorColor = uIResources.outdoorColor;
        accountManager.ResisterEvent(this);
        aPIManager.ResisterEvent(this);
    }
    public void ResistEvent(IEventHandler eventHandler)
    {
        eventHandlers.Add(eventHandler);
    }
    public void UnResistEvent(IEventHandler eventHandler)
    {
        eventHandlers.Remove(eventHandler);
    }
    public void GetChallenges(bool notEvent = false)
    {
        aPIManager.GetAsync(new GetChallengeCommand(notEvent), string.Format("/users/{0}/activities", playerData.userId));
    }
    public void GetReward(bool notEvent = false)
    {
        aPIManager.GetAsync(new GetRewardCommand(notEvent), string.Format("/users/{0}/rewards", playerData.userId));
    }
    public void PostAchievementActivity(string activityId, DateTime time)
    {
        string date = time.ToString(Format.DateFormat);
        aPIManager.PostAsync(new PostAchieveActivityCommand(), string.Format("/users/{0}/activities", playerData.userId),
            new JObject
            {
                { "activityId" , activityId },
                { "activityTime", date}
            });
    }
    public Color GetColor(string part)
    {
        switch (part)
        {
            case "inapp":
                return inAppColor;
            case "outdoor":
                return outDoorColor;
            default:
                return Color.white;
        }
    }

    #region "Response NetCommand Event"
    public void OnSuccessGetChallenges(NetworkMessage message, bool notEvent)
    {
        ChallengeList convertData = JsonUtility.FromJson<ChallengeList>(message.body);
        ChallangeList = convertData;
        if (notEvent)
        {
            return;
        }
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnGetChallenges(convertData);
        }
    }

    public void OnFailedGetChallenges(NetworkMessage message)
    {
        Debug.LogError(message.body);
    }

    public void OnSuccessArchieveActivity(NetworkMessage message)
    {
        Debug.Log(message.body);
        Alarm alarm = JsonUtility.FromJson<Alarm>(message.body);
        NotifyView notify = UIView.Get<NotifyView>();
        foreach (var notifyData in alarm.pushAlarms)
        {
            notifyData.showingTime = 3f;
            notify.PushNotify(notifyData);
        }
    }


    public void OnFailedArchieveActivity(NetworkMessage message)
    {
        Debug.LogError(message.body);
    }

    public void OnSuccessReward(NetworkMessage message, bool notEvent)
    {
        ActivityData data = JsonUtility.FromJson<ActivityData>(message.body);
        activityData = data;
        if (notEvent)
        {
            return;
        }

        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnGetActivities(data);
        }
    }

    public void OnFailedReward(NetworkMessage message)
    {
        Debug.LogError(message.body);
    }

    public void OnSuccessLogin(LocalPlayerData playerData)
    {
        GetChallenges(true);
        GetReward(true);
    }

    public void OnFailLogin(string error)
    {
    }

    public void OnSuccessSignUp()
    {
        GetChallenges(true);
        GetReward(true);
    }

    public void OnFailSignUp(string error)
    {
    }

    public void OnSuccessGetUser(LocalPlayerData playerData)
    {
    }

    public void OnFailGetUser(string error)
    {
    }
    #endregion

    [System.Serializable]
    public class ChallengeList
    {
        public string userId;
        public int userClass = 0;
        public int outdoorLevel = 0;
        public int inappLevel = 0;
        public int healthLevel = 0;
        public float overallProgress = 0f;
        public Challenge[] challenges;

        public Challenge Get(string challengeId)
        {
            foreach (var challenge in challenges)
            {
                if (challenge.challengeId == challengeId)
                {
                    return challenge;
                }
            }
            Debug.LogError("Not Found Challenge : " + challengeId);
            return null;
        }
    }
    public string GetChallengeName(string challengeId)
    {
        foreach (var challenge in ChallangeList.challenges)
        {
            if (challenge.challengeId == challengeId)
            {
                return challenge.name;
            }
        }
        Debug.LogError("Not Found ChallengeName : " + challengeId);
        return "";
    }
    public string GetChallengeId(string activityId)
    {
        foreach (var activity in activityData.activities)
        {
            if (activity.activityId == activityId)
            {
                return activity.challengeId;
            }
        }

        Debug.LogError("Not Found ChallengeID");
        return "";
    }
    public List<string> GetChallengeIds(DateTime date)
    {
        List<string> results = new List<string>();
        foreach (var challenge in ChallangeList.challenges)
        {
            foreach (var record in challenge.records)
            {
                DateTime recordTime = Convert.ToDateTime(record);
                if(date.Day == recordTime.Day && date.Month == recordTime.Month && date.Year == recordTime.Year)
                {
                    results.Add(challenge.challengeId);
                    break;
                }
            }
        }
        return results;
    }
    public string GetPart(string activityId)
    {
        foreach (var activity in activityData.activities)
        {
            if (activity.activityId == activityId)
            {
                return activity.part;
            }
        }

        Debug.LogError("Not Found Part");
        return "";
    }
    public string GetActivityName(string activityId)
    {
        foreach (var activity in activityData.activities)
        {
            if (activity.activityId == activityId)
            {
                return activity.name;
            }
        }

        Debug.LogError("Not Found ActivityName");
        return "";
    }
    [System.Serializable]
    public class Challenge
    {
        public string challengeId;
        public string name;
        public string part;
        public int habits = 0;
        public int level = 0;
        public float progress = 0f;
        public string startDate;
        public string endDate;
        public string[] records;
    }
    [System.Serializable]
    public class ActivityData
    {
        public string userId;
        public Activity[] activities;
        public Record[] records;

        public Activity Get(string challengeId)
        {
            foreach (var activity in activities)
            {
                if (activity.challengeId == challengeId)
                {
                    return activity;
                }
            }
            Debug.LogError("Not Found Activity In Datas " + challengeId);
            return null;
        }
    }
    [System.Serializable]
    public class Activity
    {
        public string activityId;
        public string name;
        public string challengeId;
        public string part;
    }
    [System.Serializable]
    public class Record
    {
        public string activityId;
        public string createdTime;
        public int heart;
        public int coin;
    }
}
