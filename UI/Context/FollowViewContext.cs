namespace MindPlus.Contexts.Master.Menus.PeopleView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using UnityEngine;
    public class FollowViewContext : Context
    {
        #region String
        private readonly Property<string> _followingCountProperty = new Property<string>();
        public string FollowingCountText
        {
            get => _followingCountProperty.Value;
            set => _followingCountProperty.Value = value;
        }

        private readonly Property<string> _followerCountProperty = new Property<string>();
        public string FollowerCountText
        {
            get => _followerCountProperty.Value;
            set => _followerCountProperty.Value = value;
        }
        #endregion
    }
}
