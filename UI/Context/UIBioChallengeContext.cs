using Slash.Unity.DataBind.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Contexts.Pool
{
    public class UIBioChallengeContext : Context
    {
        private readonly Property<bool> _isActiveIconProperty = new Property<bool>();
        public bool IsActiveIcon
        {
            get => _isActiveIconProperty.Value;set => _isActiveIconProperty.Value = value;
        }
        private readonly Property<string> _titleTextProperty = new Property<string>();
        public string Title
        {
            get => _titleTextProperty.Value; set => _titleTextProperty.Value = value;
        }
        private readonly Property<Sprite> _iconProperty = new Property<Sprite>();
        public Sprite Icon
        {
            get => _iconProperty.Value; set => _iconProperty.Value = value;
        }
    }
}
