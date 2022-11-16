using Slash.Unity.DataBind.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Contexts.Master.Menus.WellnessView
{
    public class WellnessBiosViewContext : Context
    {
        private readonly Property<string> _weekTextProperty = new Property<string>();
        public string WeekText
        {
            get => _weekTextProperty.Value;
            set => _weekTextProperty.Value = value;
        }
    }
}

