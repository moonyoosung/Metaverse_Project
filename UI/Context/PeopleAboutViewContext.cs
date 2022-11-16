namespace MindPlus.Contexts.Master.ProfileView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PeopleAboutViewContext : Context
    {
        #region "Object"
        private readonly Property<bool> _isActiveMoreProperty = new Property<bool>();
        public bool IsActiveMore
        {
            get => _isActiveMoreProperty.Value;
            set => _isActiveMoreProperty.Value = value;
        }
        private readonly Property<bool> _isActiveFriendProperty = new Property<bool>();
        public bool IsActiveFriend
        {
            get => _isActiveFriendProperty.Value;
            set => _isActiveFriendProperty.Value = value;
        }
        private readonly Property<bool> _isActiveNotFriendProperty = new Property<bool>();
        public bool IsActiveNotFriend
        {
            get => _isActiveNotFriendProperty.Value;
            set => _isActiveNotFriendProperty.Value = value;
        }
        #endregion

        #region "Button"
        public Action onClickGift;
        public void OnClickGift()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickGift?.Invoke();
        }
        public Action onClickMore;
        public void OnClickMore()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickMore?.Invoke();
        }
        public Action onClickUnfriend;
        public void OnClickUnfriend()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickUnfriend?.Invoke();
        }
        public Action onClickFollow;
        public void OnClickFollow()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickFollow?.Invoke();
        }
        public Action onClickBlock;
        public void OnClickBlock()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickBlock?.Invoke();
        }
        #region"아직 미사용"
        public Action onClickReport;
        public void OnClickReport()
        {
            onClickReport?.Invoke();
        }
        public Action onClickMsg;
        public void OnClickMsg()
        {
            onClickMsg?.Invoke();
        }
        public Action onClickInvite;
        public void OnClickInvite()
        {
            onClickInvite?.Invoke();
        }
        public Action onClickMute;
        public void OnClickMute()
        {
            onClickMute?.Invoke();
        }
        #endregion

        #endregion

        #region"Toggle"
        public Action<bool, bool> OnFollowingToggleChanged;

        private readonly Property<bool> _onFollowingToggleProperty = new Property<bool>();
        public bool FollowingToggle
        {
            get => _onFollowingToggleProperty.Value;
            set
            {
                bool prev = _onFollowingToggleProperty.Value;
                _onFollowingToggleProperty.Value = value;
                OnFollowingToggleChanged?.Invoke(prev, value);
            }
        }

        public Action<bool, bool> OnFreindToggleChanged;

        private readonly Property<bool> _onFreindToggleProperty = new Property<bool>();
        public bool FreindToggle
        {
            get => _onFreindToggleProperty.Value;
            set
            {
                bool prev = _onFreindToggleProperty.Value;
                _onFreindToggleProperty.Value = value;
                OnFreindToggleChanged?.Invoke(prev, value);
            }
        }
        #endregion

        #region "Sprite"
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
        private readonly Property<Sprite> _onFollowingIconProperty = new Property<Sprite>();
        public Sprite OnFollowingIcon
        {
            get => _onFollowingIconProperty.Value;
            set => _onFollowingIconProperty.Value = value;
        }
        private readonly Property<Sprite> _onFriendIconProperty = new Property<Sprite>();
        public Sprite OnFriendIcon
        {
            get => _onFriendIconProperty.Value;
            set => _onFriendIconProperty.Value = value;
        }
        private readonly Property<Sprite> _onInviteIconProperty = new Property<Sprite>();
        public Sprite OnInviteIcon
        {
            get => _onInviteIconProperty.Value;
            set => _onInviteIconProperty.Value = value;
        }
        #endregion

        #region "String"
        private readonly Property<string> _nameProperty = new Property<string>();
        public string RoomName
        {
            get => _nameProperty.Value;
            set
            {
                _nameProperty.Value = value;
            }
        }
        private readonly Property<string> _descTextProperty = new Property<string>();
        public string DescText
        {
            get => _descTextProperty.Value;
            set
            {
                _descTextProperty.Value = value;
            }
        }
        private readonly Property<string> _userNameProperty = new Property<string>();
        public string UserNameText
        {
            get => _userNameProperty.Value;
            set => _userNameProperty.Value = value;
        }
        private readonly Property<string> _followerCountProperty = new Property<string>();
        public string FollowerCountText
        {
            get => _followerCountProperty.Value;
            set => _followerCountProperty.Value = value;
        }
        private readonly Property<string> _friendTextProperty = new Property<string>();
        public string FriendText
        {
            get => _friendTextProperty.Value;
            set => _friendTextProperty.Value = value;
        }
        private readonly Property<string> _inviteTextProperty = new Property<string>();
        public string InviteText
        {
            get => _inviteTextProperty.Value;
            set => _inviteTextProperty.Value = value;
        }
        #endregion

        #region"Color"
        private readonly Property<Color> _followColorProperty = new Property<Color>();
        public Color FollowColor
        {
            get => _followColorProperty.Value;
            set => _followColorProperty.Value = value;
        }
        #endregion

        private readonly Property<bool> _isInteractableProperty = new Property<bool>();
        public bool IsInteractable
        {
            get => _isInteractableProperty.Value;
            set => _isInteractableProperty.Value = value;
        }
    }
}