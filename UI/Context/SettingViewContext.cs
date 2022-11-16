using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindPlus.Contexts.Master.Menus
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class SettingViewContext : Context
    {
        public Action onClickEmail;
        public void OnClickEmail()
        {
            onClickEmail?.Invoke();
        }
        private readonly Property<bool> _isActiveMoreProperty = new Property<bool>();
        public bool IsActiveMore
        {
            get => _isActiveMoreProperty.Value;
            set => _isActiveMoreProperty.Value = value;
        }
        private readonly Property<bool> _isAlbeProperty = new Property<bool>();
        public bool IsAlbe
        {
            get => _isAlbeProperty.Value;
            set => _isAlbeProperty.Value = value;
        }

        public Action onClickClose;
        public void OnClickClose()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickClose?.Invoke();
        }

        public Action onClickLogout;
        public void OnClickLogout()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickLogout?.Invoke();
        }

        public Action<float> onMusicSliderValueChanged;
        private readonly Property<float> _musicSliderValueProperty = new Property<float>();
        public float MusicSliderValue
        {
            get => _musicSliderValueProperty.Value;
            set
            {
                if (_musicSliderValueProperty.Value != value)
                {
                    _musicSliderValueProperty.Value = value;
                    onMusicSliderValueChanged?.Invoke(value);
                }
            }
        }

        public Action<bool> onMusicToggleChanged;

        private readonly Property<bool> _musicToggleProperty = new Property<bool>();
        public bool MusicToggle
        {
            get => _musicToggleProperty.Value;
            set
            {
                _musicToggleProperty.Value = value;
                onMusicToggleChanged?.Invoke(value);
            }
        }

        public Action<float> onSoundSliderValueChanged;
        private readonly Property<float> _soundSliderValueProperty = new Property<float>();
        public float SoundSliderValue
        {
            get => _soundSliderValueProperty.Value;
            set
            {
                if (_soundSliderValueProperty.Value != value)
                {
                    _soundSliderValueProperty.Value = value;
                    onSoundSliderValueChanged?.Invoke(value);
                }
            }
        }

        public Action<bool> onSoundToggleChanged;

        private readonly Property<bool> _soundToggleProperty = new Property<bool>();
        public bool SoundToggle
        {
            get => _soundToggleProperty.Value;
            set
            {
                _soundToggleProperty.Value = value;
                onSoundToggleChanged?.Invoke(value);
            }
        }

        public Action<float> onAmbienceSliderValueChanged;
        private readonly Property<float> _ambienceSliderValueProperty = new Property<float>();
        public float AmbienceSliderValue
        {
            get => _ambienceSliderValueProperty.Value;
            set
            {
                if (_ambienceSliderValueProperty.Value != value)
                {
                    _ambienceSliderValueProperty.Value = value;
                    onAmbienceSliderValueChanged?.Invoke(value);
                }
            }
        }

        public Action<bool> onAmbienceToggleChanged;

        private readonly Property<bool> _ambienceToggleProperty = new Property<bool>();
        public bool AmbienceToggle
        {
            get => _ambienceToggleProperty.Value;
            set
            {
                _ambienceToggleProperty.Value = value;
                onAmbienceToggleChanged?.Invoke(value);
            }
        }

        public Action<float> onVoiceSliderValueChanged;
        private readonly Property<float> _voiceSliderValueProperty = new Property<float>();
        public float VoiceSliderValue
        {
            get => _voiceSliderValueProperty.Value;
            set
            {
                if (_voiceSliderValueProperty.Value != value)
                {
                    _voiceSliderValueProperty.Value = value;
                    onVoiceSliderValueChanged?.Invoke(value);
                }
            }
        }

        public Action<bool> onVoiceToggleChanged;

        private readonly Property<bool> _voiceToggleProperty = new Property<bool>();
        public bool VoiceToggle
        {
            get => _voiceToggleProperty.Value;
            set
            {
                _voiceToggleProperty.Value = value;
                onVoiceToggleChanged?.Invoke(value);
            }
        }

        private readonly Property<string> _propertyCoinText = new Property<string>();
        public string CoinText
        {
            get => _propertyCoinText.Value;
            set => _propertyCoinText.Value = value;
        }
        private readonly Property<string> _propertyHeartText = new Property<string>();
        public string HeartText
        {
            get => _propertyHeartText.Value;
            set => _propertyHeartText.Value = value;
        }
    }
}
