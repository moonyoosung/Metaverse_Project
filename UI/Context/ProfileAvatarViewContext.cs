namespace MindPlus.Contexts.Master.ProfileView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ProfileAvatarViewContext : Context
    {
        #region "Button"
        public Action onClickEdit;
        public void OnClickEdit()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickEdit?.Invoke();
        }
        #endregion

        #region "Bool"
        private readonly Property<bool> _isActiveEditproperty = new Property<bool>();
        public bool IsActiveEdit
        {
            get => _isActiveEditproperty.Value;
            set => _isActiveEditproperty.Value = value;
        }
        private readonly Property<bool> _isActiveProfileproperty = new Property<bool>();
        public bool IsActiveProfile
        {
            get => _isActiveProfileproperty.Value;
            set => _isActiveProfileproperty.Value = value;
        }
        private readonly Property<bool> _isActiveCaptureproperty = new Property<bool>();
        public bool IsActiveCapture 
        { 
            get => _isActiveCaptureproperty.Value;
            set => _isActiveCaptureproperty.Value = value;
        }   
        #endregion
    }
}