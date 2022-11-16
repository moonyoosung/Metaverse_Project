using Slash.Unity.DataBind.Core.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Contexts.Master.Menus.ChallengeView
{
    public class ActivityRecordViewContext : Context
    {
        private readonly Property<string> _dayTextProperty = new Property<string>();
        public string DayText
        {
            get => _dayTextProperty.Value;
            set => _dayTextProperty.Value = value;
        }
        private readonly Property<bool> _isActiveNoneProperty = new Property<bool>();
        public bool IsActiveNone
        {
            get => _isActiveNoneProperty.Value;
            set => _isActiveNoneProperty.Value = value;
        }
    }
}

