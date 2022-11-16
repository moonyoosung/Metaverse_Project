namespace MindPlus.Contexts.Master.Menus.PeopleView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPeopleContext : Context
    {
        #region"Object"
        private readonly Property<bool> _onlineIconActiveProperty = new Property<bool>();
        public bool OnlineIconActive
        {
            get => _onlineIconActiveProperty.Value;
            set => _onlineIconActiveProperty.Value = value;
        }
        #endregion

        #region"Interactable"
        private readonly Property<bool> _interactableProperty = new Property<bool>();
        public bool IsInteractable
        {
            get => _interactableProperty.Value;
            set => _interactableProperty.Value = value;
        }
        #endregion

        #region"Text"
        private readonly Property<string> _nameProperty = new Property<string>();
        public string UserName
        {
            get => _nameProperty.Value;
            set => _nameProperty.Value = value;
        }
        #endregion

        #region"Image"
        private readonly Property<Sprite> _thumbnailProperty = new Property<Sprite>();
        public Sprite ThumbnailIcon
        {
            get => _thumbnailProperty.Value;
            set => _thumbnailProperty.Value = value;
        }
        private readonly Property<Sprite> _onlineIconProperty = new Property<Sprite>();
        public Sprite OnlineIcon
        {
            get => _onlineIconProperty.Value;
            set => _onlineIconProperty.Value = value;
        }
        #endregion

        #region"Button"
        public Action onClickProfile;
        public void OnClickProfile()
        {
            onClickProfile?.Invoke();
        }
        #endregion
    }
}