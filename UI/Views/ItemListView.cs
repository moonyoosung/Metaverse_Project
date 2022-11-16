using MindPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListView : UIView, GetItemListCommnad.IEventHandler
{
    public ToggleGroup headGroup;
    public ToggleGroup topGroup;
    public ToggleGroup bottomGroup;

    private ToggleGroup toggleGroup;
    public ScrollRect scroll;
    public System.Action onGetItemList;
    private GridLayoutGroup layoutGroup;
    private Vector2 baseCellSize;
    private Vector2 baseCellSpacing;
    private UIPool itemPool;
    private List<UIItem> uIItems = new List<UIItem>();
    [SerializeField]
    private UIItemListData.Item[] itemDatas;
    private APIManager apiManager;
    private LocalPlayerData localPlayerData;
    private AvatarPartsType type = AvatarPartsType.Hair;
    private Option itemOption = Option.None;
    public enum Option
    {
        None = 0,
        Owned = 1,
        Favorite = 2,
        Option2 = 4,
        Option3 = 8,
        Option4 = 16,
        Option5 = 32,
        Option6 = 64
    }

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.localPlayerData = persistent.AccountManager.PlayerData;
        this.itemPool = (uIManager as UIManager).GetPool(StringTable.UIItemPool);
        this.layoutGroup = scroll.content.GetComponent<GridLayoutGroup>();
        this.apiManager = persistent.APIManager;
        this.toggleGroup = GetComponent<ToggleGroup>();
        apiManager.ResisterEvent(this);
        //baseCellSize = layoutGroup.cellSize;
        //baseCellSpacing = layoutGroup.spacing;

        //Vector2 standardSize = new Vector2(1920f, 1080f);
        //Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        //layoutGroup.cellSize = (screenSize / standardSize) * baseCellSize;
        //layoutGroup.spacing = (screenSize / standardSize) * baseCellSpacing;

        //GetItemList();
    }

    void OnEnable()
    {
      
    }

    public override void OnDestroy()
    {
        apiManager.UnResisterEvent(this);
        base.OnDestroy();
    }

    public void GetItemList()
    {
        try
        {
            //apiManager.GetAsync(new GetItemListCommnad(), string.Format("/users/{0}/custom", localPlayerData.userId));

            apiManager.GetAsync(new GetItemListCommnad(), string.Format("/users/{0}/custom?head={1}", localPlayerData.userId, localPlayerData.GetCustomAvatar().GetObjectString(AvatarPartsType.Head)));
        }catch(System.Exception e)
        {
            Debug.Log("exception " + e.ToString());
        }
    }

    public override void OnFinishHide()
    {
        base.OnFinishHide();
        RestPool();
    }

    public void RestPool()
    {
        foreach (var item in uIItems)
        {
            item.InActivePool();
        }
        uIItems.Clear();
    }
    public void Set(AvatarPartsType type = AvatarPartsType.Hair, Option itemOption = Option.None)
    {
        this.type = type;
        this.itemOption = itemOption;

        if (itemDatas == null || itemDatas.Length == 0 /*|| GameManager.isLoggingOut*/)
        {
            GetItemList();
            return;
        }
        RestPool();

        MindPlusPlayer player = null;
        if(NetworkManager.Instance != null && NetworkManager.Instance.currentRoomManager != null)
            player = NetworkManager.Instance.currentRoomManager.player;
        SelectCharacterView selectCharacterView = null;
        ProfileAvatarView profileAvatarView = null;
        if (player == null)
        {
            selectCharacterView = Get<SelectCharacterView>();
            player = selectCharacterView.MindPlusPlayer;
        }

        if (player == null)
        {
            return;
        }

        profileAvatarView = Get<ProfileAvatarView>();
        foreach (var item in itemDatas)
        {
            if (CustomAvatar.GetPartType(item.part) == type)
            {
                bool isShow = true;
                if (type == AvatarPartsType.Hair)
                {
                    Customization customization = player.GetPart<PlayerRig>().customization;
                    int itemColor = GetObjectColor(item.customId);
                    int playerColor = GetObjectColor(customization.avatar.customAvatar.GetWholeString(AvatarPartsType.Hair));
                    isShow = itemColor == 0 || playerColor == itemColor || (playerColor == 0 && itemColor == 1);
                }

                if (itemOption.HasFlag(Option.Owned) && item.owned == 0)
                {
                    isShow = false;
                }
                //if(itemOption.HasFlag(Option.Favorite) && item.)

                if (isShow)
                {
                    UIItem uiItem = itemPool.Get<UIItem>(scroll.content.transform);
                    uiItem.Set(item, toggleGroup, selectCharacterView, profileAvatarView);
                    uIItems.Add(uiItem);
                }
            }
        }
        foreach (var item in uIItems)
        {
            if (item.SetOn())
                break;
        }
        onGetItemList?.Invoke();
    }

    int GetObjectColor(string data)
    {
        string[] s = data.Split("_");
        int value = System.Convert.ToInt32(s[2]);
        return value;
    }


    private ToggleGroup GetGroup(AvatarPartsType type)
    {
        switch (type)
        {
            case AvatarPartsType.Head:
                return headGroup;
            case AvatarPartsType.Top:
                return topGroup;
            case AvatarPartsType.Bottom:
                return bottomGroup;
            default:
                return null;
        }
    }

    public UIItemListData.Item[] GetItemDatas()
    {
        return itemDatas;
    }

    public void OnGetItemListSuccess(NetworkMessage message)
    {
        itemDatas = JsonHelper.FromJson<UIItemListData.Item>(message.body);
        Set(type, itemOption);
    }

    public void OnGetItemListFailed(NetworkMessage message)
    {
        Debug.Log("ItemListTest : OnGetItemListFailed response : " + message.response);
        Debug.Log("ItemListTest : OnGetItemListFailed body : " + message.body);
    }
}