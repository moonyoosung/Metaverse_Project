
namespace MindPlus.Contexts.Master.Menus.PeopleView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    public class FriendRequestViewContext : Context
    {
        #region Text
        private readonly Property<string> _requestFriendCountProperty = new Property<string>();
        public string RequestFriendCountText
        {
            get => _requestFriendCountProperty.Value;
            set => _requestFriendCountProperty.Value = value;
        }
        #endregion
    }
}