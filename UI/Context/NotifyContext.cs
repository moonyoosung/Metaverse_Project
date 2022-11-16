
using Slash.Unity.DataBind.Core.Data;
using System;
using UnityEngine;

namespace MindPlus.Contexts.Master
{
    public class NotifyContext : Context
    {
        private readonly Property<string> _textProperty = new Property<string>();
        public string ToastMessage
        {
            get => _textProperty.Value;
            set => _textProperty.Value = value;
        }
        private readonly Property<string> _titleTextProperty = new Property<string>();
        public string TitleText
        {
            get => _titleTextProperty.Value;
            set => _titleTextProperty.Value = value;
        }
        private readonly Property<Sprite> _iconProperty = new Property<Sprite>();
        public Sprite Icon
        {
            get => _iconProperty.Value;
            set => _iconProperty.Value = value;
        }

        private readonly Property<bool> _activeHeartIconProperty = new Property<bool>();
        public bool IsActiveHeartIcon
        {
            get => _activeHeartIconProperty.Value;
            set => _activeHeartIconProperty.Value = value;
        }
        private readonly Property<bool> _activeCoinIconProperty = new Property<bool>();
        public bool IsActiveCoinIcon
        {
            get => _activeCoinIconProperty.Value;
            set => _activeCoinIconProperty.Value = value;
        }
        private readonly Property<bool> _activeHeartTextProperty = new Property<bool>();
        public bool IsActiveHeartText
        {
            get => _activeHeartTextProperty.Value;
            set => _activeHeartTextProperty.Value = value;
        }
        private readonly Property<bool> _activeCoinTextPorperty = new Property<bool>();
        public bool IsActiveCoinText
        {
            get => _activeCoinTextPorperty.Value;
            set => _activeCoinTextPorperty.Value = value;
        }
        private readonly Property<bool> _activeButtonPorperty = new Property<bool>();
        public bool IsActiveButton
        {
            get => _activeButtonPorperty.Value;
            set => _activeButtonPorperty.Value = value;
        }

        private readonly Property<Sprite> _heartIconSpriteProperty = new Property<Sprite>();
        public Sprite HeartIcon
        {
            get => _heartIconSpriteProperty.Value;
            set => _heartIconSpriteProperty.Value = value;
        }
        private readonly Property<Sprite> _coinIconSpriteProperty = new Property<Sprite>();
        public Sprite CoinIcon
        {
            get => _coinIconSpriteProperty.Value;
            set => _coinIconSpriteProperty.Value = value;
        }
        private readonly Property<string> _heartTextProperty = new Property<string>();
        public string HeartText
        {
            get => _heartTextProperty.Value;
            set => _heartTextProperty.Value = value;
        }
        private readonly Property<string> _coinTextProperty = new Property<string>();
        public string CoinText
        {
            get => _coinTextProperty.Value;
            set => _coinTextProperty.Value = value;
        }
        public Action onClickButton;
        public void OnClickButton()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickButton?.Invoke();
        }
    }
}

