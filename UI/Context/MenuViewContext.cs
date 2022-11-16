namespace MindPlus.Contexts.Master
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using MindPlus.Contexts.Master.Menus;
    using UnityEngine;

    public class MenuViewContext : Context
    {
        #region"Menus"
        private readonly Property<WorldViewContext> _worldViewProperty = new Property<WorldViewContext>();
        public WorldViewContext WorldViewContext
        {
            get => _worldViewProperty.Value;
            set => _worldViewProperty.Value = value;
        }
        private readonly Property<PeopleViewContext> _peopleViewProperty = new Property<PeopleViewContext>();
        public PeopleViewContext PeopleViewContext
        {
            get => _peopleViewProperty.Value;
            set => _peopleViewProperty.Value = value;
        }
        private readonly Property<SettingViewContext> _settingViewProperty = new Property<SettingViewContext>();
        public SettingViewContext settingViewContext
        {
            get => _settingViewProperty.Value;
            set => _settingViewProperty.Value = value;
        }
        #endregion
        #region"Buttons"
        public Action onClickWorld;
        public void OnClickWorld()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickWorld?.Invoke();
        }
        public Action onClickPeople;
        public void OnClickPeople()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickPeople?.Invoke();
        }
        public Action onClickChallenge;
        public void OnClickChallenge()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickChallenge?.Invoke();
        }
        public Action onClickStore;
        public void OnClickStore()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickStore?.Invoke();
        }
        public Action onClickWellness;
        public void OnClickWellness()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickWellness?.Invoke();
        }
        public Action onClickSetting;
        public void OnClickSetting()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickSetting?.Invoke();
        }
        public Action onClickMyProfile;
        public void OnClickMyProfile()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickMyProfile?.Invoke();
        }
        #endregion

        #region Sprite
        private readonly Property<Sprite> _thumbnailIconProperty = new Property<Sprite>();
        public Sprite ThumbnailIcon
        {
            get => _thumbnailIconProperty.Value;
            set => _thumbnailIconProperty.Value = value;
        }
        #endregion

        #region Object
        private readonly Property<bool> _albeProperty = new Property<bool>();
        public bool IsAlbe
        {
            get => _albeProperty.Value;
            set => _albeProperty.Value = value;
        }
        #endregion

        #region Text
        private readonly Property<string> _nameTextProperty = new Property<string>();
        public string NameText
        {
            get => _nameTextProperty.Value;
            set => _nameTextProperty.Value = value;
        }
        private readonly Property<string> _idTextProperty = new Property<string>();
        public string IdText
        {
            get => _idTextProperty.Value;
            set => _idTextProperty.Value = value;
        }
        #endregion
    }
}

