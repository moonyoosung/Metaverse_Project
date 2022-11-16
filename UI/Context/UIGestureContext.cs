namespace MindPlus.Contexts.Pool
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UIGestureContext : Context
    {
        private readonly Property<Sprite> _gestureIconProperty = new Property<Sprite>();
        public Sprite GestureIcon
        {
            get => _gestureIconProperty.Value;
            set => _gestureIconProperty.Value = value;
        }
        private readonly Property<string> _gestureNameTextProperty = new Property<string>();
        public string GestureNameText
        {
            get => _gestureNameTextProperty.Value;
            set => _gestureNameTextProperty.Value = value;
        }
        private readonly Property<bool> _activeBackLightProperty = new Property<bool>();
        public bool IsActiveBackLight
        {
            get => _activeBackLightProperty.Value;
            set => _activeBackLightProperty.Value = value;
        }
        private readonly Property<bool> _activeFavoriteProperty = new Property<bool>();
        public bool IsActiveFavorite
        {
            get => _activeFavoriteProperty.Value;
            set => _activeFavoriteProperty.Value = value;
        }
        private readonly Property<bool> _favoriteProperty = new Property<bool>();
        public bool IsFavorite
        {
            get => _favoriteProperty.Value;
            set => _favoriteProperty.Value = value;
        }
        public Action onClickGesture;
        public void OnClickGesture()
        {
            onClickGesture?.Invoke();
        }
    }
}