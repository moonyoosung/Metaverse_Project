


namespace MindPlus.Contexts.Master.Menus.WorldView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class CreateViewContext : Context
    {

        #region"CoverImages"
        private Property<Sprite> _propertyMainCover = new Property<Sprite>();
        public Sprite MainCover
        {
            get => _propertyMainCover.Value;
            set => _propertyMainCover.Value = value;
        }
        private Property<bool> _propertyIsActiveMainCover = new Property<bool>();
        public bool IsActiveMainCover
        {
            get => _propertyIsActiveMainCover.Value;
            set => _propertyIsActiveMainCover.Value = value;
        }
        private Property<string> _propertyUploadText = new Property<string>();
        public string UploadText
        {
            get => _propertyUploadText.Value;
            set => _propertyUploadText.Value = value;
        }
        public Action onClickUploadCover;
        public void OnClickUploadCover()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickUploadCover?.Invoke();
        }
        #endregion

        #region"EventTitleSetting"
        private Property<string> _propertyTitleText = new Property<string>();
        public string TitleText
        {
            get => _propertyTitleText.Value;
            set => _propertyTitleText.Value = value;
        }
        private Property<string> _propertyTitleInputField = new Property<string>();
        public string TitleInputText
        {
            get => _propertyTitleInputField.Value;
            set
            {
                _propertyTitleInputField.Value = value;
                SetValue("TitleText", string.Format(Format.CreatTitle, value.Length));
            }
        }
        #endregion

        #region"Capacity"
        private Property<string> _propertyCapacityText = new Property<string>();
        public string CapacityText
        {
            get => _propertyCapacityText.Value;
            set => _propertyCapacityText.Value = value;
        }

        public void OnClickPlus()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            int capacity = int.Parse(CapacityText) + 1;
            SetValue("CapacityText", capacity);
        }
        public void OnClickMinus()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            int capacity = int.Parse(CapacityText) - 1;
            SetValue("CapacityText", capacity);
        }
        #endregion

        #region"Description"
        private Property<string> _propertydescriptionTitleText = new Property<string>();
        public string DescriptionTitleText
        {
            get => _propertydescriptionTitleText.Value;
            set => _propertydescriptionTitleText.Value = value;
        }
        private Property<string> _propertyDecriptionInputText = new Property<string>();
        public string DescriptionInputText
        {
            get => _propertyDecriptionInputText.Value;
            set
            {
                _propertyDecriptionInputText.Value = value;
                SetValue("DescriptionTitleText", string.Format(Format.DescriptionTitle, value.Length));
            }
        }
        #endregion
    }
}

