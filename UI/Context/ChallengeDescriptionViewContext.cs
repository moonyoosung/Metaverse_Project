using Slash.Unity.DataBind.Core.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Contexts.Master.Menus.ChallengeView
{
    public class ChallengeDescriptionViewContext : Context
    {
        private readonly Property<string> _titleTextProperty = new Property<string>();
        public string Title
        {
            get => _titleTextProperty.Value;
            set => _titleTextProperty.Value = value;
        }
        private readonly Property<string> _descriptionTextProperty = new Property<string>();
        public string Description
        {
            get => _descriptionTextProperty.Value;
            set => _descriptionTextProperty.Value = value;
        }
        private readonly Property<Sprite> _iconProperty = new Property<Sprite>();
        public Sprite Icon
        {
            get => _iconProperty.Value;
            set => _iconProperty.Value = value;
        }
    }
}

