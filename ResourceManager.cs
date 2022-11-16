using Photon.Pun;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResourceManager 
{
    private AvatarItemObjectListData[] avatarProbs;
    private UIResources[] uIResources;
    public ThumbnailData ThumbnailData { private set; get; }
    public StringFilterData StringFilterData { private set; get; }
    public SettingData SettingData { private set; get; }
    public ImageContainer ImageContainer { private set; get; }
    public ImageContainer ChallengeImages { private set; get; }
    public ImageContainer ChallGrpahImages { private set; get; }
    public HealthActivityPair HealthActivityPair { private set; get; }
    public ChallengeDescriptions ChallengeDescriptions { private set; get; }
    public ArticleDescriptions ArticleDescriptions { private set; get; }
    private Platform type;
    public ResourceManager(Platform type)
    {
        this.type = type;
        avatarProbs = Resources.LoadAll<AvatarItemObjectListData>("Avatar Probs");
        uIResources = Resources.LoadAll<UIResources>("UI/Scriptable");
        this.ChallengeDescriptions = Resources.Load<ChallengeDescriptions>("UI/Scriptable/ChallengeDescriptions");
        this.ArticleDescriptions = Resources.Load<ArticleDescriptions>("UI/Scriptable/ArticleDescriptions");
        this.ImageContainer = Resources.Load<ImageContainer>("UI/Scriptable/ImageContainer");
        this.ChallengeImages = Resources.Load<ImageContainer>("UI/Scriptable/ChallengeImageContainer");
        this.ChallGrpahImages = Resources.Load<ImageContainer>("UI/Scriptable/ChallengeGraphImageContainer");
        this.HealthActivityPair = Resources.Load<HealthActivityPair>("UI/Scriptable/HealthActivityPair");
        this.ThumbnailData = Resources.Load<ThumbnailData>("UI/Scriptable/RoomThumbnailData");
        this.StringFilterData = Resources.Load<StringFilterData>("StringFilterData");
        this.SettingData = Resources.Load<SettingData>("SettingData");
    }

    public AvatarItemObjectListData[] GetAvatarProbs()
    {
        return avatarProbs;
    }
    public UIResources GetUIResources()
    {
        foreach (var uIResource in uIResources)
        {
            if(uIResource.type == type)
            {
                return uIResource;
            }
        }

        Debug.Log("Not Found UIResource" + type.ToString());
        return null;
    }


}
