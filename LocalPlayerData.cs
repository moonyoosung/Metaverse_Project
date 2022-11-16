using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class LocalPlayerData
{
    public string userId;
    public string userName;
    public string profile;

    public string bags;
    public string belts;
    public string bottom;
    public string earrings;
    public string glasses;
    public string hair;
    public string hand;
    public string hats;
    public string head;
    public string necklaces;
    public string top;

    public int heart = 0;
    public int coin = 0;
    public string myRoomId;
    public string connectionId;
    public string description;

    public LocalPlayerData(string defaultName)
    {
        this.userName = defaultName;
        this.heart = 0;
        this.coin = 0;
    }

    public LocalPlayerData(string userId, string userName, string profile)
    {
        this.userId = userId;
        this.userName = userName;
        this.profile = profile;
    }

    public void SetCustom(CustomAvatar customAvatar)
    {
        bags = customAvatar.bags;
        belts = customAvatar.belts;
        bottom = customAvatar.bottom;
        earrings = customAvatar.earrings;
        glasses = customAvatar.glasses;
        hair = customAvatar.hair;
        hand = customAvatar.hand;
        hats = customAvatar.hats;
        head = customAvatar.head;
        necklaces = customAvatar.necklaces;
        top = customAvatar.top;
    }

    public CustomAvatar GetCustomAvatar()
    {
        return new CustomAvatar(bags, belts, bottom, earrings, glasses, hair, hand, hats, head, necklaces, top).GetCustomPlusParts();
    }
}

[System.Serializable]
public class CustomAvatar
{
    public string bags;
    public string belts;
    public string bottom;
    public string earrings;
    public string glasses;
    public string hair;
    public string hand;
    public string hats;
    public string head;
    public string necklaces;
    public string top;

    public CustomAvatar()
    {
        this.bags = "bags_000_00";
        this.belts = "belts_000_00";
        this.bottom = "bottom_001_01";
        this.earrings = "earrings_000_00";
        this.glasses = "glasses_000_00";
        this.hair = "hair_000_00";
        this.hand = "hand_000_00";
        this.hats = "hats_000_00";
        this.head = "head_001_01";
        this.necklaces = "necklaces_000_00";
        this.top = "top_001_01";
    }

    public CustomAvatar(string bags, string belts, string bottom, string earrings, string glasses, string hair, string hand, string hats, string head, string neckiaces, string top)
    {
        this.bags = bags;
        this.belts = belts;
        this.bottom = bottom;
        this.earrings = earrings;
        this.glasses = glasses;
        this.hair = hair;
        this.hand = hand;
        this.hats = hats;
        this.head = head;
        this.necklaces = neckiaces;
        this.top = top;
    }

    public CustomAvatar GetCustomPlusParts()
    {
        if (top.Split("_").Length < 3)
        {
            CustomAvatar customAvatar = new CustomAvatar();
            customAvatar.bags = "bags_" + bags;
            customAvatar.belts = "belts_" + belts;
            customAvatar.bottom = "bottom_" + bottom;
            customAvatar.earrings = "earrings_" + earrings;
            customAvatar.glasses = "glasses_" + glasses;
            customAvatar.hair = "hair_" + hair;
            customAvatar.hand = "hand_" + hand;
            customAvatar.hats = "hats_" + hats;
            customAvatar.head = "head_" + head;
            customAvatar.necklaces = "necklaces_" + necklaces;
            customAvatar.top = "top_" + top;
            return customAvatar;
        }
        return this;
    }

    public CustomAvatar GetNew()
    {
        CustomAvatar customAvatar = new CustomAvatar();
        if (top.Split("_").Length < 3)
        {
            customAvatar.bags = "bags_" + bags;
            customAvatar.belts = "belts_" + belts;
            customAvatar.bottom = "bottom_" + bottom;
            customAvatar.earrings = "earrings_" + earrings;
            customAvatar.glasses = "glasses_" + glasses;
            customAvatar.hair = "hair_" + hair;
            customAvatar.hand = "hand_" + hand;
            customAvatar.hats = "hats_" + hats;
            customAvatar.head = "head_" + head;
            customAvatar.necklaces = "necklaces_" + necklaces;
            customAvatar.top = "top_" + top;
        }
        else
        {
            customAvatar.bags = bags;
            customAvatar.belts = belts;
            customAvatar.bottom = bottom;
            customAvatar.earrings = earrings;
            customAvatar.glasses = glasses;
            customAvatar.hair = hair;
            customAvatar.hand = hand;
            customAvatar.hats = hats;
            customAvatar.head = head;
            customAvatar.necklaces = necklaces;
            customAvatar.top = top;
        }
        return customAvatar;
    }

    public void SetHeadMeshString(AvatarCharacter avatarCharacter)
    {
        string[] splits = head.Split("_");
        StringBuilder sb = new StringBuilder();
        sb.Append(splits[0]);
        sb.Append("_");
        sb.Append(ConverIntToString((int)avatarCharacter, 3));
        sb.Append("_");
        sb.Append(splits[2]);
        head = sb.ToString();
    }

    public void SetWholeString(AvatarPartsType type, string s)
    {
        switch (type)
        {
            case AvatarPartsType.Bags:
                bags = s;
                break;
            case AvatarPartsType.Belts:
                belts = s;
                break;
            case AvatarPartsType.Bottom:
                bottom = s;
                break;
            case AvatarPartsType.Earrings:
                earrings = s;
                break;
            case AvatarPartsType.Glasses:
                glasses = s;
                break;
            case AvatarPartsType.Hair:
                hair = s;
                break;
            case AvatarPartsType.Hand:
                hand = s;
                break;
            case AvatarPartsType.Hats:
                hats = s;
                break;
            case AvatarPartsType.Head:
                head = s;
                break;
            case AvatarPartsType.Necklaces :
                necklaces = s;
                break;
            case AvatarPartsType.Top:
                top = s;
                break;
        }
    }
    public void SetColorString(AvatarPartsType type, string color)
    {
        string[] splits = GetWholeString(type).Split("_");
        StringBuilder sb = new StringBuilder();
        sb.Append(splits[0]);
        sb.Append("_");
        sb.Append(splits[1]);
        sb.Append("_");
        sb.Append(color);
        SetWholeString(type, sb.ToString());
    }

    public string ConverIntToString(int value, int maxLength)
    {
        StringBuilder sb = new StringBuilder();

        string s = value.ToString();

        for (int i = 0; i < maxLength - s.Length; i++)
            sb.Append("0");
        sb.Append(s);
        return sb.ToString();
    }

    public string GetObjectString(AvatarPartsType type)
    {
        string s = GetWholeString(type);
        return s.Split("_")[1];
    }

    public int GetObjectIndex(AvatarPartsType type)
    {
        return System.Convert.ToInt32(GetObjectString(type));
    }

    public string GetColorString(AvatarPartsType type)
    {
        string s = GetWholeString(type);
        return s.Split("_")[2];
    }

    public int GetColorIndex(AvatarPartsType type)
    {
        return System.Convert.ToInt32(GetColorString(type));
    }

    public string GetStringWithoutParts(AvatarPartsType type)
    {
        string[] s = GetWholeString(type).Split("_");
        StringBuilder sb = new StringBuilder();
        sb.Append(s[1]);
        sb.Append("_");
        sb.Append(s[2]);
        return sb.ToString();
    }

    public string GetWholeString(AvatarPartsType type)
    {
        string s = "";
        switch (type)
        {
            case AvatarPartsType.Bags:
                s = bags;
                break;
            case AvatarPartsType.Belts:
                s = belts;
                break;
            case AvatarPartsType.Bottom:
                s = bottom;
                break;
            case AvatarPartsType.Earrings:
                s = earrings;
                break;
            case AvatarPartsType.Glasses:
                s = glasses;
                break;
            case AvatarPartsType.Hair:
                s = hair;
                break;
            case AvatarPartsType.Hand:
                s = hand;
                break;
            case AvatarPartsType.Hats:
                s = hats;
                break;
            case AvatarPartsType.Head:
                s = head;
                break;
            case AvatarPartsType.Necklaces :
                s = necklaces;
                break;
            case AvatarPartsType.Top:
                s = top;
                break;
        }
        return s;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("bags : " + bags);
        sb.Append("belts : " + belts);
        sb.Append("\nbottom : " + bottom);
        sb.Append("\ntop : " + top);
        sb.Append("\nearrings : " + earrings);
        sb.Append("\nglasses : " + glasses);
        sb.Append("\nhair : " + hair);
        sb.Append("\nhand : " + hand);
        sb.Append("\nhats : " + hats);
        sb.Append("\nhead : " + head);
        sb.Append("\nnecklaces : " + necklaces);
        return sb.ToString();
    }

    public static CustomAvatar GetInitialSetting(AvatarCharacter avatarCharacter)
    {
        CustomAvatar avatar = new CustomAvatar();

        switch (avatarCharacter)
        {
            case AvatarCharacter.Character0:
                avatar.bottom = "bottom_001_01";
                avatar.hair = "hair_000_00";
                avatar.head = "head_001_01";
                avatar.top = "top_001_01";
                break;
            case AvatarCharacter.Character1:
                avatar.bottom = "bottom_002_01";
                avatar.earrings = "earrings_001_01";
                avatar.hair = "hair_002_01";
                avatar.head = "head_002_01";
                avatar.top = "top_002_01";
                break;
            case AvatarCharacter.Character2:
                avatar.bottom = "bottom_004_01";
                avatar.hair = "hair_003_03";
                avatar.head = "head_003_01";
                avatar.top = "top_005_01";
                break;
            case AvatarCharacter.Character3:
                avatar.bottom = "bottom_004_01";
                avatar.hair = "hair_004_09";
                avatar.head = "head_004_01";
                avatar.top = "top_004_01";
                break;
        }
        return avatar;
    }
    public static AvatarPartsType GetPartType(string part)
    {
        AvatarPartsType t = AvatarPartsType.Belts;

        switch (part)
        {
            case "bags":
                t = AvatarPartsType.Bags;
                break;
            case "belts":
                t = AvatarPartsType.Belts;
                break;
            case "bottom":
                t = AvatarPartsType.Bottom;
                break;
            case "earrings":
                t = AvatarPartsType.Earrings;
                break;
            case "glasses":
                t = AvatarPartsType.Glasses;
                break;
            case "hair":
                t = AvatarPartsType.Hair;
                break;
            case "hand":
                t = AvatarPartsType.Hand;
                break;
            case "hats":
                t = AvatarPartsType.Hats;
                break;
            case "head":
                t = AvatarPartsType.Head;
                break;
            case "necklaces":
                t = AvatarPartsType.Necklaces;
                break;
            case "top":
                t = AvatarPartsType.Top;
                break;
            default:
                break;
        }

        return t;
    }
    public static bool IsCustomChanged(CustomAvatar a, CustomAvatar b)
    {
        AvatarPartsType t = (AvatarPartsType)0;
        for (int i = 0; i < LooxidExtensions.EnumLength<AvatarPartsType>(t); i++)
        {
            if(string.Compare(a.GetWholeString(t), b.GetWholeString(t)) != 0)
            {
                return true;
            }
        }
        return false;
    }
    public static List<string> GetCustomDifferences(CustomAvatar originCustom, CustomAvatar newCustom)
    {
        List<string> list = new List<string>();
        AvatarPartsType compareType = (AvatarPartsType)0;
        int length = LooxidExtensions.EnumLength<AvatarPartsType>(compareType);
        for (int i = 0; i < length; i++)
        {
            compareType = LooxidExtensions.GetEnumAt<AvatarPartsType>(compareType, i);
            if (string.Compare(originCustom.GetWholeString(compareType), newCustom.GetWholeString(compareType)) != 0)
            {
                list.Add(newCustom.GetWholeString(compareType));
            }
        }
        return list;
    }
}
