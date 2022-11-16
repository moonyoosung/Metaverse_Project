namespace MindPlus.Contexts.Master.Menus
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PeopleViewContext : Context
    {
        #region "Object"
        private readonly Property<bool> _peopleSliderProperty = new Property<bool>();

        public bool PeopleSliderActive
        {
            get => _peopleSliderProperty.Value;
            set => _peopleSliderProperty.Value = value;
        }
        private readonly Property<bool> _profileSliderProperty = new Property<bool>();

        public bool ProfileSliderActive
        {
            get => _profileSliderProperty.Value;
            set => _profileSliderProperty.Value = value;
        }
        private readonly Property<bool> _activeDividerPropertyIs = new Property<bool>();
        public bool IsActiveDivider
        {
            get => _activeDividerPropertyIs.Value;
            set => _activeDividerPropertyIs.Value = value;
        }
        private readonly Property<bool> _activeInviteProperty = new Property<bool>();
        public bool IsActiveInvite
        {
            get => _activeInviteProperty.Value;
            set => _activeInviteProperty.Value = value;
        }
        private readonly Property<bool> _activeSearchProperty = new Property<bool>();
        public bool IsActiveSearch
        {
            get => _activeSearchProperty.Value;
            set => _activeSearchProperty.Value = value;
        }
        private readonly Property<bool> _albeProperty = new Property<bool>();
        public bool IsAlbe
        {
            get => _albeProperty.Value;
            set => _albeProperty.Value = value;
        }
        #endregion

        #region Interact
        private readonly Property<bool> _titleInteractProperty = new Property<bool>();
        public bool IsTitleInteract
        {
            get => _titleInteractProperty.Value;
            set => _titleInteractProperty.Value = value;
        }
        #endregion

        #region String
        private readonly Property<string> _titleTextProperty = new Property<string>();
        public string TitleText
        {
            get => _titleTextProperty.Value;
            set => _titleTextProperty.Value = value;
        }
        #endregion

        #region Sprite
        private readonly Property<Sprite> _titleIconProperty = new Property<Sprite>();
        public Sprite TitleIcon
        {
            get => _titleIconProperty.Value;
            set => _titleIconProperty.Value = value;
        }
        #endregion

        #region Button
        public Action<int> onClickSlider;
        public void OnClickSlider(float idx)
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickSlider?.Invoke((int)idx);
        }
        public Action onClickBack;
        public void OnClickBack()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickBack?.Invoke();
        }
        public Action onClickClose;
        public void OnClickClose()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickClose?.Invoke();
        }
        #endregion
    }
}
