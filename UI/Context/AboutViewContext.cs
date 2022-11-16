namespace MindPlus.Contexts.Master.Menus.WorldView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class AboutViewContext : Context
    {
        #region"MainBG"
        private readonly Property<string> _propertyPageText = new Property<string>();
        public string PageText
        {
            get => _propertyPageText.Value;
            set => _propertyPageText.Value = value;
        }
        private readonly Property<Sprite> _propertyMainImage = new Property<Sprite>();
        public Sprite MainImage
        {
            get => _propertyMainImage.Value;
            set => _propertyMainImage.Value = value;
        }
        #endregion

        #region"MainInfo"
        private readonly Property<string> _propertyWatchingText = new Property<string>();
        public string WatchingText
        {
            get => _propertyWatchingText.Value;
            set => _propertyWatchingText.Value = value;
        }
        private readonly Property<string> _propertyLikeText = new Property<string>();
        public string LikeText
        {
            get => _propertyLikeText.Value;
            set => _propertyLikeText.Value = value;
        }
        #endregion

        #region"LikeInfo"
        public Action<bool, bool> OnLikeToggleChanged;

        private readonly Property<bool> _propertyLikeToggle = new Property<bool>();
        public bool LikeToggle
        {
            get => _propertyLikeToggle.Value;
            set
            {
                bool prev = _propertyLikeToggle.Value;
                _propertyLikeToggle.Value = value;
                OnLikeToggleChanged?.Invoke(prev, value);
            }
        }
        #endregion

        #region"Time"
        private readonly Property<string> _propertyTimeText = new Property<string>();
        public string TimeText
        {
            get => _propertyTimeText.Value;
            set => _propertyTimeText.Value = value;
        }
        #endregion

        #region"About"
        private readonly Property<string> _propertyAboutText = new Property<string>();
        public string AboutText
        {
            get => _propertyAboutText.Value;
            set => _propertyAboutText.Value = value;
        }
        #endregion

        #region"Official"
        private readonly Property<string> _propertyOfficialText = new Property<string>();
        public string OfficialText
        {
            get => _propertyOfficialText.Value;
            set => _propertyOfficialText.Value = value;
        }
        #endregion
    }
}
