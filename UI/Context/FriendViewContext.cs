namespace MindPlus.Contexts.Master.Menus.PeopleView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;

    public class FriendViewContext : Context
    {
        #region Text
        private readonly Property<string> _friendCountProperty = new Property<string>();
        public string FriendCountText
        {
            get => _friendCountProperty.Value;
            set => _friendCountProperty.Value = value;
        }
        #endregion
    }
}