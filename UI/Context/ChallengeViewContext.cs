using Slash.Unity.DataBind.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MindPlus.Contexts.Master.Menus
{
    public class ChallengeViewContext : Context
    {
        private Sprite titleIcon;
        private Sprite backIcon;
        public ChallengeViewContext(Sprite titleIcon, Sprite backIcon)
        {
            this.titleIcon = titleIcon;
            this.backIcon = backIcon;
        }
        private readonly Property<string> _coinTextProperty = new Property<string>();
        public string CoinText
        {
            get => _coinTextProperty.Value;
            set => _coinTextProperty.Value = value;
        }
        private readonly Property<string> _heartTextProperty = new Property<string>();
        public string HeartText
        {
            get => _heartTextProperty.Value;
            set => _heartTextProperty.Value = value;
        }
        private readonly Property<Vector2> _titleTransformProperty = new Property<Vector2>();
        public Vector2 TitleTransform
        {
            get => _titleTransformProperty.Value;
            set => _titleTransformProperty.Value = value;
        }
        private readonly Property<Sprite> _TitleSpriteProperty = new Property<Sprite>();
        public Sprite TitleIcon
        {
            get => _TitleSpriteProperty.Value;
            set => _TitleSpriteProperty.Value = value;
        }
        private readonly Property<bool> _IsActiveDividerProperty = new Property<bool>();
        public bool IsActiveDivider
        {
            get => _IsActiveDividerProperty.Value;
            set => _IsActiveDividerProperty.Value = value;
        }
        private readonly Property<bool> _IsTitleInteractProperty = new Property<bool>();
        public bool IsTitleInteract
        {
            get => _IsTitleInteractProperty.Value;
            set
            {
                _IsTitleInteractProperty.Value = value;
                SetValue("IsActiveDivider", value);
                SetValue("TitleIcon", value ? backIcon : titleIcon);
                SetValue("TitleTransform", value ? new Vector2(52f, 0f) : new Vector2(24f, 0f));
            }
        }
        private readonly Property<string> _titleTextProperty = new Property<string>();
        public string TitleText
        {
            get => _titleTextProperty.Value;
            set => _titleTextProperty.Value = value;
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
        public Action onClickBack;
        public void OnClickBack()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickBack?.Invoke();
        }
    }
}
