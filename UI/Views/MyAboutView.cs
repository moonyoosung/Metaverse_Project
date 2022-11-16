using MindPlus.Contexts.Master.ProfileView;
using Newtonsoft.Json.Linq;
using MindPlus;
using UnityEngine;
using UnityEngine.UI;

public class MyAboutView : UIFlatView, GetLocationCommand.IEventHandler
{
    private Persistent persistent;
    private MyAboutViewContext context;
    public ScrollRect scroll;
    public RectTransform rectTransform;
    public ContentSizeFitter contentSizeFitter;

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.persistent = persistent;
        this.context = new MyAboutViewContext();
        ContextHolder.Context = context;

        UIManager UIManager = (uIManager as UIManager);

        context.SetValue("IsInteractName", false);
        context.onClickEdit = () =>
        {
            context.SetValue("IsActiveEditMode", true);
            context.SetValue("IsActiveReadMode", false);

            //context.SetValue("IsInteractName", true);
            context.SetValue("IsInteractDesc", true);
        };
        context.onClickCance = () =>
        {
            context.SetValue("IsActiveEditMode", false);
            context.SetValue("IsActiveReadMode", true);

            //context.SetValue("NameText", persistent.AccountManager.PlayerData.userName);
            context.SetValue("DescText", persistent.AccountManager.PlayerData.description);
            //context.SetValue("IsInteractName", false);
            context.SetValue("IsInteractDesc", false);
        };
        context.onClickSave += OnClickSave;


        //친구가 아닌 상태에서 버튼 상호작용
        //context.OnNameToggleChanged += OnNameToggleChanged;
        //context.OnDescToggleChanged += OnDescToggleChanged;

        //기본 Room Image를 Loading 상태로 초기화
        context.SetValue("ThumbnailIcon", persistent.ResourceManager.ImageContainer.Get("loadingcontent"));
        context.SetValue("RoomName", "Loading");
        
    }

    private void OnClickSave()
    {
        context.onClickSave -= OnClickSave;

        if (Util.LengthCheck(4, 10, context.NameText))
        {
            persistent.UIManager.PushNotify("Nickname must be between 4 and 10 characters.", 2f);
            context.onClickSave += OnClickSave;
            Debug.Log("OnClickSave LengthCheck");
            return;
        }

        if (Util.NameCheck(context.NameText))
        {
            persistent.UIManager.PushNotify("Nickname contains characters that are not allowed.", 2f);
            context.onClickSave += OnClickSave; 
            Debug.Log("OnClickSave NameCheck");
            return;
        }

    persistent.AccountManager.ModifyUserData(persistent.AccountManager.PlayerData.userId,
        new JObject {/* { "userName", context.NameText },*/ { "description", context.DescText } },
        () =>
        {
            //체크?
            context.onClickSave += OnClickSave;
            //persistent.AccountManager.PlayerData.userName = context.NameText;
            persistent.AccountManager.PlayerData.description = context.DescText;
            //NetworkManager.Instance.currentRoomManager.player.ChangePlayerNickNameText(context.NameText);
        },
        (message) =>
        {
            context.onClickSave += OnClickSave;
        });

        context.SetValue("IsActiveEditMode", false);
        context.SetValue("IsActiveReadMode", true);

        //context.SetValue("IsInteractName", false);
        context.SetValue("IsInteractDesc", false);
    }

    public override void OnStartHide()
    {
        //persistent.AccountManager.ModifyUserData(persistent.AccountManager.PlayerData.userId, new JObject { { "userName", context.NameText } });
        //persistent.AccountManager.GetUser(persistent.AccountManager.PlayerData.userId);

        //정보 입력 못하도록 상호작용 막기
        context.SetValue("IsInteractDesc", false);
        //context.SetValue("IsInteractName", false);

        context.SetValue("IsActiveEditMode", false);
        context.SetValue("IsActiveReadMode", true);

        this.persistent.APIManager.UnResisterEvent(this);
        base.OnStartHide();
    }

    public void Set(LocalPlayerData data)
    {
        this.persistent.APIManager.ResisterEvent(this);
        persistent.PeopleManager.GetLocation(data.userId);
        //기본 Room Image를 Loading 상태로 초기화
        context.SetValue("ThumbnailIcon", persistent.ResourceManager.ImageContainer.Get("loadingcontent")); 
        context.SetValue("RoomName", "None");

        context.SetValue("NameText", data.userName);
        context.SetValue("DescText", data.description);
        context.SetValue("FollowerCountText", "0 followers");

        float height = 600 + rectTransform.sizeDelta.y;
        scroll.content.sizeDelta = new Vector2(scroll.content.sizeDelta.x, height);
        scroll.content.localPosition = Vector3.zero;

        contentSizeFitter.SetLayoutHorizontal();
    }

    public void OnGetLocationSuccess(NetworkMessage message)
    {
        JObject jObject = JObject.Parse(message.body);
        if (jObject == null || string.IsNullOrEmpty(jObject.ToString()))
        {
            //context.SetValue("ThumbnailIcon", persistent.ResourceManager.ImageContainer.Get("loadingcontent"));
            //context.SetValue("RoomName", "None");
            return;
        }

        if (jObject == null || jObject.Count == 0)
        {
            return;
        }

        ContentData contentData = persistent.RoomDataBaseManager.RoomAPIHandler.CreateContentData(jObject);

        if (contentData != null && contentData.roomId != null)
        {
            if (!string.IsNullOrEmpty(contentData.thumbnail))
            {
                persistent.APIManager.DownLoadTexture(contentData.thumbnail, (sprite) =>
                {
                    if (sprite)
                    {
                        context.SetValue("ThumbnailIcon", sprite);
                    }
                });
            }
            else if (!string.IsNullOrEmpty(contentData.sceneName))
            {
                string sceneName = contentData.sceneName.ToLower();
                persistent.APIManager.DownLoadTexture("location/" + sceneName + ".jpg", (sprite) =>
                {
                    if (sprite)
                    {
                        context.SetValue("ThumbnailIcon", sprite);
                    }
                    else
                    {
                        context.SetValue("ThumbnailIcon", persistent.ResourceManager.ThumbnailData.Get(sceneName).thumbnail);
                    }
                });
            }
            //context.SetValue("ThumbnailIcon", persistent.ResourceManager.ThumbnailData.Get(contentData.sceneName).thumbnail);
            Debug.Log("  context.SetValue(RoomName, contentData.roomName);  " + contentData.roomName);
            context.SetValue("RoomName", contentData.roomName);
        }
    }

    public void OnGetLocationFailed(NetworkMessage message)
    {
    }

    public void OnFreindToggleChanged(bool prev, bool next)
    {
    }
}
