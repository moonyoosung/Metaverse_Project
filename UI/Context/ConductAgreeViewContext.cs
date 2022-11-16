namespace MindPlus.Contexts.TitleView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class ConductAgreeViewContext : Context
    {
        private readonly Property<bool> _agreeToggleProperty = new Property<bool>();
        public bool IsAgree
        {
            get => _agreeToggleProperty.Value;
            set => _agreeToggleProperty.Value = value;
        }
        public Action onClickPlay;
        public void OnClickPlay()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickPlay?.Invoke();
        }
    }
}
