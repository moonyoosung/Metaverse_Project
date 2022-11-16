using MindPlus.Contexts.Master.Menus.WorldView;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AboutView : UIView
{
    [System.NonSerialized]
    public ContentData data;
    public ScrollRect scroll;
    private AboutViewContext context;
    private ThumbnailData thumbnailData;
    private APIManager aPIManager;
    private RoomAPIHandler roomApi;
    private bool isLike = false;
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.context = new AboutViewContext();
        ContextHolder.Context = context;
        this.thumbnailData = persistent.ResourceManager.ThumbnailData;
        this.aPIManager = persistent.APIManager;
        this.roomApi = persistent.RoomDataBaseManager.RoomAPIHandler;
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();
        data = null;
        isLike = false;
        context.OnLikeToggleChanged -= OnToggleValueChanged;
        context.SetValue("LikeToggle", false);
    }
    public void Set(ContentData data)
    {
        roomApi.GetLikeRoom(data.roomId, (result) =>
        {
            var item = JObject.Parse(result);
            var value = item["item"];
            isLike = value != null ? true : false;
            context.SetValue("LikeToggle", isLike);
            context.OnLikeToggleChanged += OnToggleValueChanged;
        });
        scroll.content.localPosition = Vector3.zero;
        this.data = data;
        if (!string.IsNullOrEmpty(data.thumbnail))
        {
            aPIManager.DownLoadTexture(data.thumbnail, (sprite) =>
            {
                context.SetValue("MainImage", sprite);
            });
        }
        else
        {
            context.SetValue("MainImage", thumbnailData.Get(data.sceneName).thumbnail);
        }

        EventContentData eventData = data as EventContentData;
        if (eventData != null)
        {
            context.SetValue("WatchingText", string.Format("In this Room: {0}/{1}", eventData.numPlayers, eventData.maxPlayers));
        }
        else
        {
            context.SetValue("WatchingText", string.Format("Max Player: {0}", data.maxPlayers));
        }
        context.SetValue("LikeText", data.numLikes.ToString());
        context.SetValue("AboutText", data.description);
    }
    private void OnToggleValueChanged(bool prev, bool next)
    {
        if (data == null || prev == next)
        {
            return;
        }

        if (!prev && next)
        {
            roomApi.PostLikeRoom(data.roomId);
            data.numLikes += 1;
            context.SetValue("LikeText", data.numLikes.ToString());
        }
        else if (prev && !next)
        {
            roomApi.DeleteLikeRoom(data.roomId);
            data.numLikes -= 1;
            context.SetValue("LikeText", data.numLikes.ToString());
        }
    }
}
