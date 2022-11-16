namespace MindPlus.Contexts.TitleView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MembershipViewContext : Context
    {
        #region "Button"
        public Action onClickSignUp;
        public void OnSignUp()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickSignUp?.Invoke();
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
        public Action onClickPasswordOpen;
        public void OnClickPasswordOpen()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickPasswordOpen?.Invoke();
        }
        public Action onClickPasswordClose;
        public void OnClickPasswordClose()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickPasswordClose?.Invoke();
        }
        public Action onClickLink;
        public void OnClickLink()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickLink?.Invoke();
        }
        #endregion

        #region "string"
        private readonly Property<string> _idProperty = new Property<string>();
        public string ID
        {
            get => _idProperty.Value;
            set => _idProperty.Value = value;
        }
        private readonly Property<string> _passwordProperty = new Property<string>();
        public string Password
        {
            get => _passwordProperty.Value;
            set => _passwordProperty.Value = value;
        }
        private readonly Property<string> _idNotiTextProperty = new Property<string>();
        public string IDNotify
        {
            get => _idNotiTextProperty.Value;
            set => _idNotiTextProperty.Value = value;
        }
        private readonly Property<string> _pwNotiTextProperty = new Property<string>();
        public string PWNotify
        {
            get => _pwNotiTextProperty.Value;
            set => _pwNotiTextProperty.Value = value;
        }
        #endregion

        #region "Color"
        private readonly Property<Color> _idnotiTextColorProperty = new Property<Color>();
        public Color IDNotiColor
        {
            get => _idnotiTextColorProperty.Value;
            set => _idnotiTextColorProperty.Value = value;
        }
        private readonly Property<Color> _pwnotiTextColorproperty = new Property<Color>();
        public Color PWNotiColor
        {
            get => _pwnotiTextColorproperty.Value;
            set => _pwnotiTextColorproperty.Value = value;
        }
        #endregion

        #region "bool"
        private readonly Property<bool> _toggleProperty = new Property<bool>();
        public bool AgreeToggle
        {
            get => _toggleProperty.Value;
            set => _toggleProperty.Value = value;
        }
        private readonly Property<bool> _idNotiActiveProperty = new Property<bool>();
        public bool IsActiveIDNotify
        {
            get => _idNotiActiveProperty.Value;
            set => _idNotiActiveProperty.Value = value;
        }
        private readonly Property<bool> _pwNotiActiveProperty = new Property<bool>();
        public bool IsActivePWNotify
        {
            get => _pwNotiActiveProperty.Value;
            set => _pwNotiActiveProperty.Value = value;
        }
        private readonly Property<bool> _activeOpenProperty = new Property<bool>();
        public bool IsActivePasswordOpen
        {
            get => _activeOpenProperty.Value;
            set => _activeOpenProperty.Value = value;
        }
        private readonly Property<bool> _activeCloseProperty = new Property<bool>();
        public bool IsActivePasswordClose
        {
            get => _activeCloseProperty.Value;
            set => _activeCloseProperty.Value = value;
        }
        #endregion
        private readonly Property<InputField.ContentType> _passwordTypeProperty = new Property<InputField.ContentType>();
        public InputField.ContentType PasswordType
        {
            get => _passwordTypeProperty.Value;
            set => _passwordTypeProperty.Value = value;
        }
        #region "Sprite"
        private readonly Property<Sprite> _idNotiImageProperty = new Property<Sprite>();
        public Sprite IDNotiImage
        {
            get => _idNotiImageProperty.Value;
            set => _idNotiImageProperty.Value = value;
        }
        private readonly Property<Sprite> _pwNotiImageProperty = new Property<Sprite>();
        public Sprite PWNotiImage
        {
            get => _pwNotiImageProperty.Value;
            set => _pwNotiImageProperty.Value = value;
        }
        #endregion

        public void SetIDNotify(string message, Sprite icon, Color color)
        {
            if (!(bool)GetValue("IsActiveIDNotify"))
            {
                SetValue("IsActiveIDNotify", true);
            }
            SetValue("IDNotiColor", color);
            SetValue("IDNotiImage", icon);
            SetValue("IDNotify", message);
        }
        public void SetPWNotify(string message, Sprite icon, Color color)
        {
            if (!(bool)GetValue("IsActivePWNotify"))
            {
                SetValue("IsActivePWNotify", true);
            }
            SetValue("PWNotiColor", color);
            SetValue("PWNotiImage", icon);
            SetValue("PWNotify", message);
        }
    }
}

