
namespace MindPlus.Contexts.TitleView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class LoginViewContext : Context
    {
        #region"Button"
        public Action onClickForgot;
        public void OnClickForgot()
        {
            onClickForgot?.Invoke();
        }
        public Action onClickSignIn;
        public void OnSignIn()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickSignIn?.Invoke();
        }
        public Action onClickMembership;
        public void OnClickMembership()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickMembership?.Invoke();
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
        #endregion

        #region"string"
        private readonly Property<string> _idProperty = new Property<string>();
        public string ID
        {
            get => _idProperty.Value;
            set
            {
                _idProperty.Value = value;
                if ((Color)GetValue("IDColor") != Color.white)
                {
                    SetValue("IDColor", Color.white);
                }
            }
        }
        private readonly Property<string> _passwordProperty = new Property<string>();
        public string Password
        {
            get => _passwordProperty.Value;
            set
            {
                _passwordProperty.Value = value;
                if ((Color)GetValue("PWColor") != Color.white)
                {
                    SetValue("PWColor", Color.white);
                }
            }
        }
        private readonly Property<string> _notifyProperty = new Property<string>();
        public string NotifyText
        {
            get => _notifyProperty.Value;
            set => _notifyProperty.Value = value;
        }
        #endregion

        #region "Bool"
        private readonly Property<bool> _toggleProperty = new Property<bool>();
        public bool RememberToggle
        {
            get => _toggleProperty.Value;
            set => _toggleProperty.Value = value;
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
        private readonly Property<bool> _activeNotifyProperty = new Property<bool>();
        public bool IsActiveNotify
        {
            get => _activeNotifyProperty.Value;
            set => _activeNotifyProperty.Value = value;
        }
        #endregion

        private readonly Property<InputField.ContentType> _passwordTypeProperty = new Property<InputField.ContentType>();
        public InputField.ContentType PasswordType
        {
            get => _passwordTypeProperty.Value;
            set => _passwordTypeProperty.Value = value;
        }
        private readonly Property<Sprite> _notifyImageProperty = new Property<Sprite>();
        public Sprite NotifyIcon
        {
            get => _notifyImageProperty.Value;
            set => _notifyImageProperty.Value = value;
        }

        private readonly Property<Color> _notifyColorProperty = new Property<Color>();
        public Color NotifyColor
        {
            get => _notifyColorProperty.Value;
            set => _notifyColorProperty.Value = value;
        }
        private readonly Property<Color> _IDColorProperty = new Property<Color>();
        public Color IDColor
        {
            get => _IDColorProperty.Value;
            set => _IDColorProperty.Value = value;
        }
        private readonly Property<Color> _PWColorProperty = new Property<Color>();
        public Color PWColor
        {
            get => _PWColorProperty.Value;
            set => _PWColorProperty.Value = value;
        }

        public void SetNotify(string message, Sprite icon, Color color)
        {
            if (!(bool)GetValue("IsActiveNotify"))
            {
                SetValue("IsActiveNotify", true);
            }
            SetValue("NotifyColor", color);
            SetValue("NotifyIcon", icon);
            SetValue("NotifyText", message);
        }
    }
}

