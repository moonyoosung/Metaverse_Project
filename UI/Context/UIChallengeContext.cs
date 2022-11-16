using Slash.Unity.DataBind.Core.Data;
using System;
using UnityEngine;

namespace MindPlus.Contexts.Pool
{
    public class UIChallengeContext : Context
    {
        private readonly Property<bool> _activeLevelProperty = new Property<bool>();
        public bool IsActiveLevel
        {
            get => _activeLevelProperty.Value;
            set => _activeLevelProperty.Value = value;
        }
        private readonly Property<bool> _activeProgressProperty = new Property<bool>();
        public bool IsActiveProgress
        {
            get => _activeProgressProperty.Value;
            set => _activeProgressProperty.Value = value;
        }
        private readonly Property<string> _nameProperty = new Property<string>();
        public string Name
        {
            get => _nameProperty.Value;
            set => _nameProperty.Value = value;
        }
        private readonly Property<string> _levelProperty = new Property<string>();
        public string Level
        {
            get => _levelProperty.Value;
            set => _levelProperty.Value = value;
        }
        private readonly Property<string> _progressProperty = new Property<string>();
        public string Progress
        {
            get => _progressProperty.Value;
            set => _progressProperty.Value = value;
        }
        private readonly Property<Sprite> _iconProperty = new Property<Sprite>();
        public Sprite Icon
        {
            get => _iconProperty.Value;
            set => _iconProperty.Value = value;
        }

        public Action onClickAssignment;
        public void OnClickAssignment()
        {
            onClickAssignment?.Invoke();
        }
    }
}
