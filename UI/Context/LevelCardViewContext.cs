using Slash.Unity.DataBind.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Contexts.Master.Menus.ChallengeView
{
    public class LevelCardViewContext : Context
    {
        private readonly Property<float> _levelProgressProperty = new Property<float>();
        public float LevelProgress
        {
            get => _levelProgressProperty.Value;
            set => _levelProgressProperty.Value = value;
        }
        private readonly Property<float> _overallProgressProperty = new Property<float>();
        public float OverallProgress
        {
            get => _overallProgressProperty.Value;
            set
            {
                _overallProgressProperty.Value = value;
                SetValue("ProgressText", string.Format("{0}%", Math.Round(value * 100)));
            }
        }
        private readonly Property<string> _progressTextProperty = new Property<string>();
        public string ProgressText
        {
            get => _progressTextProperty.Value;
            set => _progressTextProperty.Value = value;
        }
        private readonly Property<string> _levelTextProperty = new Property<string>();
        public string LevelText
        {
            get => _levelTextProperty.Value;
            set => _levelTextProperty.Value = value;
        }

        private readonly Property<string> _infoTextProperty = new Property<string>();
        public string InfoText
        {
            get => _infoTextProperty.Value;
            set => _infoTextProperty.Value = value;
        }

        public Action onClickLevelCard;
        public void OnClickLevelCard()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickLevelCard?.Invoke();
        }
        public Action onClickProgress;
        public void OnClickProgress()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickProgress?.Invoke();
        }

        private readonly Property<string> _outdoorLevelProperty = new Property<string>();
        public string OutdoorLevel
        {
            get => _outdoorLevelProperty.Value;
            set => _outdoorLevelProperty.Value = value;
        }
        private readonly Property<string> _inappLevelProperty = new Property<string>();
        public string InappLevel
        {
            get => _inappLevelProperty.Value;
            set => _inappLevelProperty.Value = value;
        }
        private readonly Property<string> _healthLevelProperty = new Property<string>();
        public string HealthLevel
        {
            get => _healthLevelProperty.Value;
            set => _healthLevelProperty.Value = value;
        }
        private readonly Property<string> _totalLevelProperty = new Property<string>();
        public string TotalLevel
        {
            get => _totalLevelProperty.Value;
            set => _totalLevelProperty.Value = value;
        }
        private readonly Property<Sprite> _tierIconProperty = new Property<Sprite>();
        public Sprite TierIcon
        {
            get => _tierIconProperty.Value;
            set => _tierIconProperty.Value = value;
        }
    }
}
