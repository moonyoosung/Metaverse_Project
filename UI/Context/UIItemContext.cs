namespace MindPlus.Contexts.Pool
{
    using Slash.Unity.DataBind.Core.Data;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class UIItemContext : Context
    {
        private readonly Property<Sprite> _itemIconProperty = new Property<Sprite>();
        public Sprite ItemIcon
        {
            get => _itemIconProperty.Value;
            set => _itemIconProperty.Value = value;
        }
        private readonly Property<bool> _ownedProperty = new Property<bool>();
        public bool IsOwned
        {
            get => _ownedProperty.Value;
            set => _ownedProperty.Value = value;
        }
        private readonly Property<bool> _favoriteProperty = new Property<bool>();
        public bool IsFavorite
        {
            get => _favoriteProperty.Value;
            set => _favoriteProperty.Value = value;
        }
        private readonly Property<bool> _activeValueProperty = new Property<bool>();
        public bool IsActiveValue
        {
            get => _activeValueProperty.Value;
            set => _activeValueProperty.Value = value;
        }
        private readonly Property<bool> _activeFavoriteProperty = new Property<bool>();
        public bool IsActiveFavorite
        {
            get => _activeFavoriteProperty.Value;
            set => _activeFavoriteProperty.Value = value;
        }
        private readonly Property<Sprite> _valueIconProperty = new Property<Sprite>();
        public Sprite ValueIcon
        {
            get => _valueIconProperty.Value;
            set => _valueIconProperty.Value = value;
        }
        private readonly Property<string> _valueTextProperty = new Property<string>();
        public string ValueText
        {
            get => _valueTextProperty.Value;
            set => _valueTextProperty.Value = value;
        }
    }
}

