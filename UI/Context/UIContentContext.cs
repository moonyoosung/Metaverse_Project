
namespace MindPlus.Contexts.Pool
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIContentContext : Context
    {
        #region"Text"
        private readonly Property<string> _nameProperty = new Property<string>();
        public string RoomName
        {
            get => _nameProperty.Value;
            set
            {
                _nameProperty.Value = value;
            }
        }
        private readonly Property<string> _infotextProperty = new Property<string>();
        public string InfoText
        {
            get => _infotextProperty.Value;
            set => _infotextProperty.Value = value;
        }
        #endregion

        #region"Image"

        private readonly Property<Sprite> _thumbnailProperty = new Property<Sprite>();
        public Sprite ThumbnailIcon
        {
            get => _thumbnailProperty.Value;
            set => _thumbnailProperty.Value = value;
        }
        private readonly Property<Sprite> _liveIconProperty = new Property<Sprite>();
        public Sprite LiveIcon
        {
            get => _liveIconProperty.Value;
            set => _liveIconProperty.Value = value;
        }
        private readonly Property<Sprite> _infoIconProperty = new Property<Sprite>();
        public Sprite InfoIcon
        {
            get => _infoIconProperty.Value;
            set => _infoIconProperty.Value = value;
        }
        #endregion

        #region"Object"
        private readonly Property<bool> _infoIconActiveProperty = new Property<bool>();
        public bool InfoIconActive
        {
            get => _infoIconActiveProperty.Value;
            set => _infoIconActiveProperty.Value = value;
        }
        private readonly Property<bool> _liveIconActiveProperty = new Property<bool>();
        public bool LiveIconActive
        {
            get => _liveIconActiveProperty.Value;
            set => _liveIconActiveProperty.Value = value;
        }
        private readonly Property<bool> _infoTextActiveProperty = new Property<bool>();
        public bool InfoTextActive
        {
            get => _infoTextActiveProperty.Value;
            set => _infoTextActiveProperty.Value = value;
        }
        #endregion

        #region"Button"
        public Action onClickAbout;
        #endregion

        public void OnClickAbout()
        {
            onClickAbout?.Invoke();
        }

    }
}