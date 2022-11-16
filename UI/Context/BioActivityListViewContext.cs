using Slash.Unity.DataBind.Core.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Contexts.Master.Menus.WellnessView
{
    public class BioActivityListViewContext : Context
    {
        private readonly Property<string> _dayTextProperty = new Property<string>();
        public string Day
        {
            get => _dayTextProperty.Value; set => _dayTextProperty.Value = value;
        }
    }
}
