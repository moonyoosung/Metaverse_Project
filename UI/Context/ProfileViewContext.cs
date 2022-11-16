namespace MindPlus.Contexts.Master
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using UnityEngine;

    public class ProfileViewContext : Context
    {
        #region "Object"        
        //IsInteract
        private readonly Property<bool> _otherProfileSliderProperty = new Property<bool>();

        public bool OtherProfileSliderActive
        {
            get => _otherProfileSliderProperty.Value;
            set => _otherProfileSliderProperty.Value = value;
        }
        private readonly Property<bool> _myProfileSliderProperty = new Property<bool>();

        public bool MyProfileSliderActive
        {
            get => _myProfileSliderProperty.Value;
            set => _myProfileSliderProperty.Value = value;
        }

        private readonly Property<bool> _propertyIsActiveInfo = new Property<bool>();
        public bool IsActiveInfo
        {
            get => _propertyIsActiveInfo.Value;
            set => _propertyIsActiveInfo.Value = value;
        }

        private readonly Property<string> _costCoinTextProperty = new Property<string>();
        public string costCoinText
        {
            get => _costCoinTextProperty.Value;
            set => _costCoinTextProperty.Value = value;
        }
        private readonly Property<string> _costHeartTextProperty = new Property<string>();
        public string costHeartText
        {
            get => _costHeartTextProperty.Value;
            set => _costHeartTextProperty.Value = value;
        }
        #endregion

        #region "Button"
        public Action<int> onClickSlider;
        public void OnClickSlider(float idx)
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickSlider?.Invoke((int)idx);
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

        #region "Sprite"
        private readonly Property<Sprite> _propertyTitleIcon = new Property<Sprite>();
        public Sprite TitleIcon
        {
            get => _propertyTitleIcon.Value;
            set => _propertyTitleIcon.Value = value;
        }
        #endregion

        #region "String"
        private readonly Property<string> _propertyTitleText = new Property<string>();
        public string TitleText
        {
            get => _propertyTitleText.Value;
            set => _propertyTitleText.Value = value;
        }
        #endregion
    }
}