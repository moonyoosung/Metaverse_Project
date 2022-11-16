using Slash.Unity.DataBind.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Contexts.Pool
{
    public class UIRecordContext : Context
    {
        public Action onClickRecord;
        public void OnClickRecord()
        {
            onClickRecord?.Invoke();
        }
        private readonly Property<Sprite> _iconProperty = new Property<Sprite>();
        public Sprite Icon
        {
            get => _iconProperty.Value;
            set => _iconProperty.Value = value;
        }
        private readonly Property<string> _titleTextProperty = new Property<string>();
        public string Title
        {
            get => _titleTextProperty.Value;
            set => _titleTextProperty.Value = value;
        }
        private readonly Property<string> _heatTextProperty = new Property<string>();
        public string Heart
        {
            get => _heatTextProperty.Value;
            set => _heatTextProperty.Value = value;
        }
        private readonly Property<string> _coinTextProperty = new Property<string>();
        public string Coin
        {
            get => _coinTextProperty.Value;
            set => _coinTextProperty.Value = value;
        }
        private readonly Property<bool> _isActiveLine = new Property<bool>();
        public bool IsActiveLine
        {
            get => _isActiveLine.Value;
            set => _isActiveLine.Value = value;
        }
        private readonly Property<Color> _textColorProperty = new Property<Color>();
        public Color TitleTextColor
        {
            get => _textColorProperty.Value;
            set => _textColorProperty.Value = value;
        }
    }
}
