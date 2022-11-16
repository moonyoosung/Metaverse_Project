
namespace MindPlus.Contexts.Player
{
    using UnityEngine;
    using Slash.Unity.DataBind.Core.Data;
    public class PlayerContext : Context
    {
        private readonly Property<PlayerNickContext> _playerNickContextProperty = new Property<PlayerNickContext>();
        public PlayerNickContext PlayerNickContext
        {
            get => _playerNickContextProperty.Value;
            set => _playerNickContextProperty.Value = value;
        }
        private readonly Property<string> _userIdProperty = new Property<string>();
        public string UserID
        {
            get => _userIdProperty.Value;
            set => _userIdProperty.Value = value;
        }
        private readonly Property<string> _profileProperty = new Property<string>();
        public string Profile
        {
            get => _profileProperty.Value;
            set => _profileProperty.Value = value;
        }
        private readonly Property<Sprite> _profileSpriteProperty = new Property<Sprite>();
        public Sprite ProfileSprite
        {
            get => _profileSpriteProperty.Value;
            set => _profileSpriteProperty.Value = value;
        }
    }
}
