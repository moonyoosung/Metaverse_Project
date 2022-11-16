using MindPlus;
using MindPlus.Contexts.Master.Menus.WorldView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CreateView : UIView
{
    public LayoutGroup[] groups;
    public ToggleGroup locationToggleGroup;
    public ToggleGroup typeToggleGroup;
    public ScrollRect scroll;
    public DialScroll[] dialScrolls;
    private CreateViewContext context;
    private List<UIToggle> eventToggles = new List<UIToggle>();
    private List<UIImageToggle> locationToggles = new List<UIImageToggle>();
    private UIPool typeTogglePool;
    private UIPool locationTogglePool;
    private ThumbnailData thumbnailData;
    private RoomDataBaseManager roomDataManager;
    private APIManager aPIManager;
    private List<FileStream> fileStreams = new List<FileStream>();

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.context = new CreateViewContext();
        ContextHolder.Context = context;
        this.typeTogglePool = (uIManager as UIManager).GetPool(StringTable.UITypeTogglePool);
        this.locationTogglePool = (uIManager as UIManager).GetPool(StringTable.UILocationTogglePool);
        this.thumbnailData = persistent.ResourceManager.ThumbnailData;
        this.roomDataManager = persistent.RoomDataBaseManager;
        this.aPIManager = persistent.APIManager;

        Dial dial = persistent.ResourceManager.GetUIResources().dial;

        foreach (var dialScroll in dialScrolls)
        {
            dialScroll.Initialize(dial, DateTime.Now);
        }

        (uIManager as UIManager).MasterContext.MenuViewContext.WorldViewContext.onClickCreate += OnClickCreate;
        context.onClickUploadCover += OnClickUploadCover;
    }

    private void OnClickUploadCover()
    {
        context.onClickUploadCover -= OnClickUploadCover;

        NativeGallery.GetImageFromGallery((path) =>
        {
            context.onClickUploadCover += OnClickUploadCover;
            if (path == null || path == string.Empty)
            {
                return;
            }

            byte[] bytes = File.ReadAllBytes(path);

            Sprite sprite = Util.ConvertBytes(bytes);
            context.SetValue("IsActiveMainCover", true);
            context.SetValue("MainCover", sprite);
            context.SetValue("UploadText", "Upload cover image (1/1)");

            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            fileStreams.Add(stream);
        });
    }

    private void OnClickCreate()
    {
        UIManager masterUIManager = UIManager as UIManager;
        masterUIManager.MasterContext.MenuViewContext.WorldViewContext.onClickCreate -= OnClickCreate;

        string roomName = context.GetValue("TitleInputText").ToString();
        string maxPlayer = context.GetValue("CapacityText").ToString();
        string description = context.GetValue("DescriptionInputText").ToString();

        ThumbnailData.RoomThumbnail data = thumbnailData.Get(locationToggleGroup.GetFirstActiveToggle().GetComponent<UIImageToggle>().uiText.text);

        List<int> bigMonth = new List<int>() { 1, 3, 5, 7, 8, 10, 12 };
        int year = int.Parse(GetDial("year"));
        int month = int.Parse(GetDial("month"));
        int day = int.Parse(GetDial("day"));
        if (!bigMonth.Contains(month) && day == 31)
        {
            (UIManager as UIManager).PushNotify("Check Open Time Month&Day", 2f);
            masterUIManager.MasterContext.MenuViewContext.WorldViewContext.onClickCreate += OnClickCreate;
            return;
        }
        int hour = int.Parse(GetDial("hour"));
        int minute = int.Parse(GetDial("minute"));
        if (hour >= 24 && minute > 0)
        {
            (UIManager as UIManager).PushNotify("Check Open Time Hour&Minute", 2f);
            masterUIManager.MasterContext.MenuViewContext.WorldViewContext.onClickCreate += OnClickCreate;
            return;
        }

        DateTime eventTime = new DateTime(year, month, day, hour, minute, 0);

        roomDataManager.RoomAPIHandler.CreateRoom(roomName, data.type, int.Parse(maxPlayer), data.sceneName, description,
            DateTime.Now < eventTime ? eventTime.ToString(Format.DateFormat) : null, OnSuccessCreateRoom, OnFailCreateRoom);
    }
    private void OnFailCreateRoom(string message)
    {
        UIManager masterUI = UIManager as UIManager;
        masterUI.MasterContext.MenuViewContext.WorldViewContext.onClickCreate += OnClickCreate;
        masterUI.PushNotify(message, 5f);
    }
    /// <summary>
    /// PUT으로 방에 있는 Thumbnail 경로 수정하기
    /// </summary>
    /// <param name="roomID"></param>
    private void OnSuccessCreateRoom(string roomID)
    {
        (UIManager as UIManager).MasterContext.MenuViewContext.WorldViewContext.onClickCreate += OnClickCreate;

        foreach (var stream in fileStreams)
        {
            string path = string.Format("events/{0}/{0}.jpg", roomID);
            aPIManager.UploadTextrue(stream, path,
                                       (result) =>
                                       {
                                           Debug.Log("Upload Complete");
                                       });
            roomDataManager.RoomAPIHandler.ModifyRoom(roomID, new Newtonsoft.Json.Linq.JObject { { "thumbnail", path } });
        }
    }

    private void Set()
    {
        scroll.content.localPosition = Vector3.zero;
        context.SetValue("TitleInputText", string.Empty);
        context.SetValue("CapacityText", "30");
        context.SetValue("DescriptionInputText", string.Empty);
        context.SetValue("IsActiveMainCover", false);
        context.SetValue("MainCover", null);
        context.SetValue("UploadText", "Upload cover image (0/1)");

        foreach (var toggle in new string[] { "Public", "Private" })
        {
            UIToggle uIToggle = typeTogglePool.Get<UIToggle>(typeToggleGroup.transform);
            uIToggle.Set(typeToggleGroup, (result) => { Debug.Log(result); }, toggle);
            eventToggles.Add(uIToggle);
        }

        eventToggles[0].toggle.isOn = true;

        foreach (var data in thumbnailData.GetThumbnails(ContentTypes.Event))
        {
            UIImageToggle uIImageToggle = locationTogglePool.Get<UIImageToggle>(locationToggleGroup.transform);
            uIImageToggle.Set(locationToggleGroup, (result) =>
            {
                context.SetValue("MainCover", data.thumbnail);
            }, data.sceneName, data.thumbnail);
            locationToggles.Add(uIImageToggle);
        }

        locationToggles[0].toggle.isOn = true;

        DateTime now = DateTime.Now;
        Debug.Log(now);
        foreach (var dialScroll in dialScrolls)
        {
            dialScroll.SetPage(now);
        }

        foreach (var group in groups)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)group.transform);
        }
    }
    public override void OnStartShow()
    {
        Set();
        base.OnStartShow();
    }

    public override void OnFinishHide()
    {
        base.OnFinishHide();
        foreach (var toggle in eventToggles)
        {
            toggle.InActivePool();
        }
        eventToggles.Clear();
        foreach (var toggle in locationToggles)
        {
            toggle.InActivePool();
        }
        locationToggles.Clear();
        fileStreams.Clear();


    }

    private string GetDial(string ID)
    {
        foreach (var dialScroll in dialScrolls)
        {
            if (dialScroll.ID == ID)
            {
                return dialScroll.GetSelect();
            }
        }
        return string.Empty;
    }

}
