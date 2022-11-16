using Slash.Unity.DataBind.Core.Data;
using System;
using TMPro;
using UnityEngine;

namespace MindPlus.Contexts.Master
{
    public class MainViewContext : Context
    {
        #region "Button"
        public Action onClickPersonChange;
        public void OnClickPersonChange()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickPersonChange?.Invoke();
        }
        public Action onClickEmotion;
        public void OnClickEmotion()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickEmotion?.Invoke();
        }
        public Action<float> onClickEmotionIcon;
        public void OnClickEmotionIcon(float index)
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickEmotionIcon?.Invoke(index);
        }
        public Action onClickGesture;
        public void OnClickGesture()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickGesture?.Invoke();
        }
      
        public Action onClickSpeaker;
        public void OnClickSpeaker()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickSpeaker?.Invoke();
        }
        public Action onClickReject;
        public void OnClickReject()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickReject?.Invoke();
        }
        public Action onClickAreement;
        public void OnClickAreement()
        {
            onClickAreement?.Invoke();
        }
        public Action onClickMenu;
        public void OnClickMenu()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickMenu?.Invoke();
        }
        #endregion

        #region "Text"
        private readonly Property<string> _chatListTextProperty = new Property<string>();
        public string ChatListText
        {
            get => _chatListTextProperty.Value;
            set => _chatListTextProperty.Value = value;
        }
        private readonly Property<int> _chatMaxLineProperty = new Property<int>();
        public int MaxLine
        {
            get => _chatMaxLineProperty.Value;
            set => _chatMaxLineProperty.Value = value;
        }
        #endregion

        #region"ScrollBar"
        private readonly Property<float> _chatScrollValueProperty = new Property<float>();
        public float ScrollValue
        {
            get => _chatScrollValueProperty.Value;
            set
            {
                _chatScrollValueProperty.Value = value;
            }
        }
        #endregion

        #region"ImageFilled"
        private readonly Property<float> _micSensitiviteProperty = new Property<float>();
        public float MicSensitivite
        {
            get => _micSensitiviteProperty.Value;
            set => _micSensitiviteProperty.Value = value;
        }
        #endregion

        #region "Bool"
        private readonly Property<bool> _activeMuteProperty = new Property<bool>();
        public bool IsActiveMute
        {
            get => _activeMuteProperty.Value;
            set => _activeMuteProperty.Value = value;
        }
        private readonly Property<bool> _activeMicIndicatorProperty = new Property<bool>();
        public bool IsActiveMic
        {
            get => _activeMicIndicatorProperty.Value;
            set => _activeMicIndicatorProperty.Value = value;
        }

        #endregion
    }
}
