using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayView : UIView, UIManager.IEventHandler
{
    private UILayoutGroupContainer groupContainer;
    private Persistent persistent;
    private ThumbnailData thumbnailData;
    private List<UIContent> poolObjects = new List<UIContent>();
    private RoomAPIHandler roomAPI;
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.persistent = persistent;
        this.thumbnailData = persistent.ResourceManager.ThumbnailData;
        this.roomAPI = persistent.RoomDataBaseManager.RoomAPIHandler;

        (uIManager as UIManager).ResisterEvent(this);
        UIHorizontalButtonGroup[] uIHorizontalLists = GetComponentsInChildren<UIHorizontalButtonGroup>(true);
        List<UILayoutGroup> lists = new List<UILayoutGroup>();
        foreach (var uIHorizontalList in uIHorizontalLists)
        {
            uIHorizontalList.Set();
            lists.Add(uIHorizontalList);
        }
        groupContainer = new UILayoutGroupContainer(lists);
    }
    public override void OnStartShow()
    {
        foreach (var group in groupContainer.groups)
        {
            roomAPI.GetContentsList(ContentTypes.Play, group.ID);
        }
        base.OnStartShow();
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();
        foreach (var poolObject in poolObjects)
        {
            poolObject.InActivePool();
        }
        poolObjects.Clear();
        groupContainer.SetActiveGroup();
    }
    public void OnADDContent(ContentData roomData, UIPool pool, string category)
    {
        //roomdata의 타입 별로 Set

        if (!groupContainer.TryGetUILayourGroup<UIHorizontalButtonGroup>(category, out UIHorizontalButtonGroup targetList))
        {
            return;
        }

        if (!targetList.IsADDAvailable())
        {
            return;
        }

        UIContent content = pool.Get<UIContent>(targetList.group.transform);
        content.Set(persistent, roomData, thumbnailData);
        poolObjects.Add(content);
        groupContainer.SetActiveGroup();
    }

    public void OnRemovedContent(ContentData roomData, UIPool pool)
    {
    }

    public void OnStartJoin(ContentData next)
    {
    }
}
