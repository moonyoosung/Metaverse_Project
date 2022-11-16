namespace MindPlus.Contexts.Master.Menus.PeopleView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIFriendRequestContext : Context
    {
        #region"Text"
        private readonly Property<string> _nameProperty = new Property<string>();
        public string UserName
        {
            get => _nameProperty.Value;
            set => _nameProperty.Value = value;
        }
        #endregion

        #region"Text"
        private readonly Property<string> _descTextProperty = new Property<string>();
        public string DescText
        {
            get => _descTextProperty.Value;
            set => _descTextProperty.Value = value;
        }

        #endregion

        private readonly Property<Color> _acceptTextProperty = new Property<Color>();
        public Color AcceptText
        {
            get => _acceptTextProperty.Value;
            set => _acceptTextProperty.Value = value;
        }

        #region"Image"
        private readonly Property<Sprite> _thumbnailProperty = new Property<Sprite>();
        public Sprite ThumbnailIcon
        {
            get => _thumbnailProperty.Value;
            set => _thumbnailProperty.Value = value;
        }

        private readonly Property<Sprite> _buttonIconProperty = new Property<Sprite>();
        public Sprite ButtonIcon
        {
            get => _buttonIconProperty.Value;
            set => _buttonIconProperty.Value = value;
        }
        #endregion

        #region"Button"
        public Action onClickDelete;
        public void OnClickDelete()
        {
            onClickDelete?.Invoke();
        }

        public Action onClickAccept;
        public void OnClickAccept()
        {
            onClickAccept?.Invoke();
        }
        #endregion
        private readonly Property<bool> _interactableProperty = new Property<bool>();
        public bool IsInteractable
        {
            get => _interactableProperty.Value;
            set => _interactableProperty.Value = value;
        }

        public UIFriendRequestContext(bool isInteract)
        {
            this.IsInteractable = isInteract;
        }
    }
}