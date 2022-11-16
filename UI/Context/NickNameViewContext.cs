using Slash.Unity.DataBind.Core.Data;
using System;
using UnityEngine;

namespace MindPlus.Contexts.TitleView
{
    public class NickNameViewContext : Context
    {
        #region"Text"
        public Action<string> nickNameChanged;
        private readonly Property<string> _nickNameProperty = new Property<string>();
        public string NickName
        {
            get => _nickNameProperty.Value;
            set
            {
                _nickNameProperty.Value = value;
                nickNameChanged?.Invoke(NickName);
            }
        }
        #endregion

        #region"Button"
        public Action onClickNext;
        public void OnClickNext()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickNext?.Invoke();
        }
        private readonly Property<bool> _isPlayInteractProperty = new Property<bool>();
        public bool IsPlayInteract
        {
            get => _isPlayInteractProperty.Value;
            set => _isPlayInteractProperty.Value = value;
        }
        #endregion
    }
}