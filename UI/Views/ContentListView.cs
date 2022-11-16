using MindPlus.Contexts.Master.Menus.WorldView;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentListView : UIView
{
    public ScrollRect scroll;
    private ContentListContext context;
    private UIPool groupPool;
    private UIPool contentPool;
    private Persistent persistent;
    private List<UIContent> uIContents = new List<UIContent>();
    private List<UIContentGroup> uIContentGroups = new List<UIContentGroup>();
    private VerticalLayoutGroup verticalLayout;
    private Queue<string> queue = new Queue<string>();

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.context = new ContentListContext();
        ContextHolder.Context = context;
        UIManager UIManager = (uIManager as UIManager);
        this.groupPool = UIManager.GetPool(StringTable.UIContentGroupPool);
        this.contentPool = UIManager.GetPool(StringTable.UIContentPool);
        this.persistent = persistent;
        this.verticalLayout = scroll.content.GetComponent<VerticalLayoutGroup>();
    }
    public override void OnStartShow()
    {
        CallAPI(queue.Peek());
        base.OnStartShow();
    }
    public override void OnFinishHide()
    {
        base.OnFinishHide();
        if (uIContents.Count > 0)
        {
            foreach (var uIContent in uIContents)
            {
                uIContent.InActivePool();
            }

            foreach (var uIContentGroup in uIContentGroups)
            {
                uIContentGroup.InActivePool();
            }

            uIContents.Clear();
            uIContentGroups.Clear();
        }
    }

    private void OnGetContentLists(string message)
    {
        List<ContentData> contentDatas = persistent.RoomDataBaseManager.RoomAPIHandler.ConvertContents(JObject.Parse(message));
        int groupCnt = 0;
        groupCnt = contentDatas.Count / 4;
        if (contentDatas.Count % 4 != 0)
        {
            groupCnt++;
        }


        for (int i = 0; i < groupCnt; i++)
        {
            UIContentGroup contentGroup = groupPool.Get<UIContentGroup>(scroll.content.transform);
            contentGroup.Set();
            uIContentGroups.Add(contentGroup);
        }

        int idx = 0;
        foreach (var group in uIContentGroups)
        {
            for (int i = idx; i < contentDatas.Count; i++)
            {

                if (!group.IsADDAvailable())
                {
                    idx = i;
                    break;
                }

                UIContent uicontent = contentPool.Get<UIContent>(group.group.transform);
                uicontent.Set(persistent, contentDatas[i], persistent.ResourceManager.ThumbnailData);
                uIContents.Add(uicontent);
            }
        }

        scroll.content.localPosition = Vector3.zero;
    }

    public void Set(string titleCategory)
    {
        queue.Enqueue(titleCategory);
        context.SetValue("Title", ConvertCateogry(titleCategory));
    }
    public void ClearQueue()
    {
        queue.Clear();
    }

    private string ConvertCateogry(string category)
    {
        switch (category)
        {
            case "upcoming":
                return "UpComing";
            case "live":
                return "Live";
            case "myevent":
                return "MyEvent";
            case "rooms":
                return "Rooms";
            case "popular":
                return "Popular";
            case "favorite":
                return "favorite";
        }

        return "Not Found";
    }
    private void CallAPI(string category)
    {
        switch (category)
        {
            case "upcoming":
                persistent.RoomDataBaseManager.RoomAPIHandler.GetContentsList(ContentTypes.Event, "upcoming", OnGetContentLists);
                break;
            case "live":
                persistent.RoomDataBaseManager.RoomAPIHandler.GetContentsList(ContentTypes.Event, "live", OnGetContentLists);
                break;
            case "myevent":
                persistent.RoomDataBaseManager.RoomAPIHandler.GetContentsList(ContentTypes.Event, "myevent", OnGetContentLists);
                break;
            case "rooms":
                persistent.RoomDataBaseManager.RoomAPIHandler.GetContentsList(ContentTypes.Location, "rooms", OnGetContentLists);
                break;
            case "popular":
                persistent.RoomDataBaseManager.RoomAPIHandler.GetContentsList(ContentTypes.Play, "popular", OnGetContentLists);
                break;
            case "favorite":
                persistent.RoomDataBaseManager.RoomAPIHandler.GetContentsList(ContentTypes.Play, "favorite", OnGetContentLists);
                break;
        }

    }
}
