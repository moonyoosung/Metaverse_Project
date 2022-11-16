using Slash.Unity.DataBind.Core.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Contexts.Master.Menus.ChallengeView
{
    public class ActivityMapViewContext : Context
    {
        private readonly Property<Sprite> _iconProperty = new Property<Sprite>();
        public Sprite Icon
        {
            get => _iconProperty.Value;
            set => _iconProperty.Value = value;
        }
        private readonly Property<string> _dateTextProperty = new Property<string>();
        public string DDay
        {
            get => _dateTextProperty.Value;
            set => _dateTextProperty.Value = value;
        }
        private readonly Property<string> _levelTextProperty = new Property<string>();
        public string Level
        {
            get => _levelTextProperty.Value;
            set => _levelTextProperty.Value = value;
        }
    }
}

