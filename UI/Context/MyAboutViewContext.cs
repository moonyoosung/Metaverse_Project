namespace MindPlus.Contexts.Master.ProfileView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MyAboutViewContext : Context
    {
        #region"String"
        private readonly Property<string> _descTextProperty = new Property<string>();
        public string DescText
        {
            get => _descTextProperty.Value;
            set => _descTextProperty.Value = value;
        }
        private readonly Property<string> _nameProperty = new Property<string>();
        public string RoomName
        {
            get => _nameProperty.Value;
            set
            {
                _nameProperty.Value = value;
            }
        }
        private readonly Property<string> _nameTextProperty = new Property<string>();
        public string NameText
        {
            get => _nameTextProperty.Value;
            set => _nameTextProperty.Value = value;
        }
        private readonly Property<string> _followerCountProperty = new Property<string>();
        public string FollowerCountText
        {
            get => _followerCountProperty.Value;
            set => _followerCountProperty.Value = value;
        }
        #endregion

        #region "Button"
        public Action onClickEdit;
        public void OnClickEdit()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickEdit?.Invoke();
        }
        public Action onClickSave;
        public void OnClickSave()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickSave?.Invoke();
        }
        public Action onClickCance;
        public void OnClickCance()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickCance?.Invoke();
        }
        #endregion

        #region activate
        private readonly Property<bool> _activeEditModeProperty = new Property<bool>();
        public bool IsActiveEditMode
        {
            get => _activeEditModeProperty.Value;
            set => _activeEditModeProperty.Value = value;
        }
        private readonly Property<bool> _activeReadModeProperty = new Property<bool>();
        public bool IsActiveReadMode
        {
            get => _activeReadModeProperty.Value;
            set => _activeReadModeProperty.Value = value;
        }
        #endregion

        #region Interactable
        private readonly Property<bool> _isInteractNameProperty = new Property<bool>();
        public bool IsInteractName
        {
            get => _isInteractNameProperty.Value;
            set => _isInteractNameProperty.Value = value;
        }
        private readonly Property<bool> _isInteractDescProperty = new Property<bool>();
        public bool IsInteractDesc
        {
            get => _isInteractDescProperty.Value;
            set => _isInteractDescProperty.Value = value;
        }
        #endregion

        #region"Sprite"
        private readonly Property<Sprite> _thumbnailProperty = new Property<Sprite>();
        public Sprite ThumbnailIcon
        {
            get => _thumbnailProperty.Value;
            set => _thumbnailProperty.Value = value;
        }
        #endregion
    }
}
