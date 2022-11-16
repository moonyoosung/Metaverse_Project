using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MindPlus;
using Slash.Unity.DataBind.Core.Presentation;
using MindPlus.Contexts;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;
using System;

public class UIManager : BaseUIManager, RoomDataBaseManager.IEventHandler
{
    public interface IEventHandler
    {
        public void OnADDContent(ContentData roomData, UIPool pool, string category);
        public void OnRemovedContent(ContentData roomData, UIPool pool);
        public void OnStartJoin(ContentData next);
    }

    private List<IEventHandler> eventHandlers = new List<IEventHandler>();
    [TitleGroup("[Option]")]
    [GUIColor(0.94f, 0.95f, 0.36f, 1f)]
    public UIPool[] pools;
    [TitleGroup("[Option]")]
    [GUIColor(0.94f, 0.95f, 0.36f, 1f)]
    public Volume depthVolume;

    public MasterContext MasterContext { private set; get; }
    [TitleGroup("[Option]")]
    [GUIColor(0.94f, 0.95f, 0.36f, 1f)]
    [SerializeField]
    private GameObject loading;
    private ChallengeManager challengeManager;
    public void ResisterEvent(IEventHandler eventHandler)
    {
        eventHandlers.Add(eventHandler);
    }
    public void UnResisterEvent(IEventHandler eventHandler)
    {
        eventHandlers.Remove(eventHandler);
    }
    public override IEnumerator Initialize(Persistent persistent)
    {
        yield return base.Initialize(persistent);
        this.challengeManager = persistent.ChallengeManager;
        loading.SetActive(false);
        MasterContext = new MasterContext();
        ContextHolder.Context = MasterContext;
        foreach (var pool in pools)
        {
            pool.Initialize();
        }

        persistent.RoomDataBaseManager.ResistEventHandler(this);

        UIView[] UIViews = GetComponentsInChildren<UIView>();
        foreach (var view in UIViews)
        {
            view.Initialize(persistent, this);
        }

        bool isInit = false;

        Hide("main" + StringTable.Init, true, () => { isInit = true; }, UIViews);

        depthVolume.enabled = false;

        while (!isInit)
        {
            yield return null;
        }
    }

    public void OnStartJoin(ContentData next)
    {
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnStartJoin(next);
        }
        depthVolume.enabled = false;
        if (next is EventContentData)
        {
            challengeManager.PostAchievementActivity(ActivityID.JoinAEventRoom, DateTime.Now);
        }
    }

    public void OnADDContent(ContentData data, string category)
    {
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnADDContent(data, GetPool(StringTable.UIContentPool), category);
        }
    }

    public void OnRemovedContent(ContentData data)
    {
        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnRemovedContent(data, GetPool(StringTable.UIContentPool));
        }
    }

    public void PushNotify(string text, float time = 3f)
    {
        NotifyView notify = UIView.Get<NotifyView>();
        //notify.PushNotify(new NotifyData(time, "Title", text));
    }
    public void Push(UIView view)
    {
        Push(view.name, false, false, null, view);
    }
    public void Pop()
    {
        Pop("", false, false);
    }
    public UIPool GetPool(string ID)
    {
        foreach (var pool in pools)
        {
            if (pool.ID == ID)
            {
                return pool;
            }
        }
        Debug.LogError("Not Found " + ID + " UIPool");
        return null;
    }
    public void ActiveIndicator(bool isOn)
    {
        if (isOn)
        {
            UIAnimManager.Push(new UIStreamShow(true, ()=> { loading.gameObject.SetActive(isOn); },UIView.Get("FullPanelView")));
        }
        else
        {
            UIAnimManager.Push(new UIStreamHide(true, ()=> { loading.gameObject.SetActive(isOn); }, UIView.Get("FullPanelView")));
        }
    }
}
