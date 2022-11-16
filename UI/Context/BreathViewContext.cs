namespace MindPlus.Contexts.MeditationView
{
    using Slash.Unity.DataBind.Core.Data;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BreathViewContext : Context
    {
        #region "Button"
        public System.Action onClickClose;
        public void OnClickClose()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickClose?.Invoke();
        }
        #endregion

        #region String
        private readonly Property<string> _stateTextProperty = new Property<string>();
        public string StateInfoText
        {
            get => _stateTextProperty.Value;
            set => _stateTextProperty.Value = value;
        }

        private readonly Property<string> _timeTextProperty = new Property<string>();
        public string TimeText
        {
            get => _timeTextProperty.Value;
            set => _timeTextProperty.Value = value;
        }
        #endregion
    }
}
