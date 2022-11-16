using MindPlus;
using MindPlus.Contexts.Pool;
using Slash.Unity.DataBind.Core.Presentation;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UIItem : UIPoolObject
{
    public ContextHolder holder;
    public Toggle toggle;
    public Sprite[] costIcon;
    private UIItemContext context;
    private UIItemListData.Item item;
    private CustomAvatar customAvatar;

    public override void Initialize(Transform parent)
    {
        base.Initialize(parent);
        this.context = new UIItemContext();
        holder.Context = context;
    }

    public void Set(UIItemListData.Item item, ToggleGroup group = null, SelectCharacterView selectCharacterView = null, ProfileAvatarView profileAvatarView = null)
    {
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);

        this.item = item;

        if(selectCharacterView != null)
        {
            customAvatar = selectCharacterView.MindPlusPlayer.GetPart<PlayerRig>().customization.avatar.customAvatar;
        }
        else if(profileAvatarView != null)
        {
            customAvatar = profileAvatarView.rig.customization.avatar.customAvatar;
        }

        SetThumbnail(item.thumbnail);

        bool isOwned = item.owned == 0 ? false : true;
        context.SetValue("IsOwned", isOwned);
        //context.SetValue("IsActiveFavorite", true);
        context.SetValue("IsActiveFavorite", false);
        //context.SetValue("IsFavorite", item.isFavorite);
        if (item.GetCost() != 0 && !isOwned)
        {
            context.SetValue("IsActiveValue", true);
            context.SetValue("ValueIcon", item.isCoin() ? costIcon[0] : costIcon[1]);
            context.SetValue("ValueText", string.Format(Format.Money, item.GetCost()));
        }
        else
        {
            context.SetValue("IsActiveValue", false);
        }
        SetID(item.customId);
        if (group != null)
        {
            toggle.group = group;
        }
    }

    public void SetThumbnail(string thumbnail)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Application.persistentDataPath);
        sb.Append("/");
        sb.Append(thumbnail);
        //FileStream f = new FileStream(Application.streamingAssetsPath + thumbnail, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        if (File.Exists(sb.ToString()))
        {
            byte[] pngBytes = File.ReadAllBytes(sb.ToString());
            context.SetValue("ItemIcon", Util.ConvertBytes(pngBytes));
        }
        else
        {
            GameManager.Instance.Persistent.APIManager.DownLoadTextureBytes(thumbnail, (pngBytes) =>
            {
                if (!File.Exists(sb.ToString()))
                {
                    string[] sprits = sb.ToString().Split("/");
                    StringBuilder directory = new StringBuilder();
                    for(int i=0;i<sprits.Length - 1; i++)
                    {
                        directory.Append(sprits[i]);
                        if(i < sprits.Length - 2)
                            directory.Append("/");
                    }
                    
                    if (!Directory.Exists(directory.ToString()))
                    {
                        Directory.CreateDirectory(directory.ToString());
                    }
                    
                    context.SetValue("ItemIcon", Util.ConvertBytes(pngBytes));
                    File.WriteAllBytes(sb.ToString(), pngBytes);
                }
            });
        }
    }

    public override void InActivePool()
    {
        base.InActivePool();
        toggle.group = null;
    }

    public bool SetOn()
    {
        string wornItem = GetCustomAvatar().GetWholeString(CustomAvatar.GetPartType(item.part));
        if (string.Compare(wornItem, item.customId) == 0)
        {
            toggle.isOn = true;
            return true;
        }
        return false;
    }

    void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {

            var customization = GetCustom();
            string wornItem = customization.avatar.customAvatar.GetWholeString(CustomAvatar.GetPartType(item.part));
            if (string.Compare(wornItem, item.customId) != 0)
            {
                customization.avatar.ChangePart(item.part, item.meshId, item.textureId);
            }
        }
    }

    Customization GetCustom()
    {
        MindPlusPlayer player = NetworkManager.Instance.currentRoomManager.player;
        if (player == null)
        {
            player = GameObject.FindObjectOfType<SelectCharacterView>().MindPlusPlayer;
        }
        return player.GetPart<PlayerRig>().customization;
    }

    CustomAvatar GetCustomAvatar()
    {
        return customAvatar;
    }
}
