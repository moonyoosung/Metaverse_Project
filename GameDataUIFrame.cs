using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MindPlus.Game
{
    [System.Serializable]
    public class GameDataUIFrame<T> : MonoBehaviour where T: GameDataFrame, new()
    {
        [Header("GameDataUIFrame")]
        public T gameData;
        public Image imageProfile;
        public Text tmpNickName;
        Sprite sprite;
        string profileUserId = "";

        public virtual void Initialize()
        {
            SetNickName("");
            imageProfile.sprite = null;
            this.gameObject.SetActive(false);
        }
        public virtual void SetNickName(string nick)
        {
            tmpNickName.text = nick;
            this.gameObject.SetActive(!string.IsNullOrEmpty(nick));
        }

        public virtual void RenewUI()
        {
            SetNickName(gameData.nickName);
        }

        public virtual void SetProfile()
        {
            if (imageProfile == null)
                return;

            if (profileUserId == gameData.userId)
            {
                imageProfile.sprite = sprite;
                return;
            }
            profileUserId = gameData.userId;

            string thumbnail = string.Format("users/{0}/{0}.jpg", gameData.userId);

            MindPlus.GameManager.Instance.Persistent.APIManager.DownLoadTexture(thumbnail, (sprite) =>
            {
                imageProfile.sprite = sprite;
                this.sprite = sprite;
            });
        }
    }
}
