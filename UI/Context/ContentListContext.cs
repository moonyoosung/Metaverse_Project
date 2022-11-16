namespace MindPlus.Contexts.Master.Menus.WorldView
{
    using Slash.Unity.DataBind.Core.Data;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ContentListContext : Context
    {
        private readonly Property<string> _titleProperty = new Property<string>();
        public string Title
        {
            get => _titleProperty.Value;
            set => _titleProperty.Value = value;
        }
    }
}

