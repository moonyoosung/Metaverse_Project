namespace MindPlus.Contexts.Master.Menus.PeopleView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using UnityEngine;
    public class ExplorerViewContext : Context
    {
        #region String
        private readonly Property<string> _recentCountProperty = new Property<string>();
        public string RecentCountText
        {
            get => _recentCountProperty.Value;
            set => _recentCountProperty.Value = value;
        }

        private readonly Property<string> _inTheRoomCountProperty = new Property<string>();
        public string InTheRoomCountText
        {
            get => _inTheRoomCountProperty.Value;
            set => _inTheRoomCountProperty.Value = value;
        }
        #endregion
    }
}
