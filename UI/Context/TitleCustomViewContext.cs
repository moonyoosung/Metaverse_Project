using Slash.Unity.DataBind.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MindPlus.Contexts.TitleView
{
    public class TitleCustomViewContext : Context
    {
        private readonly Property<string> _backButtonTextProperty = new Property<string>();
        public string backButtonText
        {
            get => _backButtonTextProperty.Value;
            set => _backButtonTextProperty.Value = value;
        }
        private readonly Property<bool> _isActiveAnchorHeadProperty = new Property<bool>();
        public bool IsActiveAnchorHead
        {
            get => _isActiveAnchorHeadProperty.Value;
            set
            {
                _isActiveAnchorHeadProperty.Value = value;
                SetValue("IsActiveAnchorHeadSub", value);
                SetValue("IsHairPart", true);
            }
        }
        private readonly Property<bool> _isActiveAnchorTopBottomProperty = new Property<bool>();
        public bool IsActiveAnchorTopBottom
        {
            get => _isActiveAnchorTopBottomProperty.Value;
            set
            {
                _isActiveAnchorTopBottomProperty.Value = value;
                SetValue("IsTopPart", true);
            }
        }
        private readonly Property<bool> _isActiveAnchorHandProperty = new Property<bool>();
        public bool IsActiveAnchorHand
        {
            get => _isActiveAnchorHandProperty.Value;
            set
            {
                _isActiveAnchorHandProperty.Value = value;
                SetValue("IsGlovePart", true);
            }
        }
        private readonly Property<bool> _isActiveAnchorFavoriteProperty = new Property<bool>();
        public bool IsActiveAnchorFavorite
        {
            get => _isActiveAnchorFavoriteProperty.Value;
            set
            {
                _isActiveAnchorFavoriteProperty.Value = value;
            }
        }
        private readonly Property<bool> _isActiveAnchorHeadSubProperty = new Property<bool>();
        public bool IsActiveAnchorHeadSub
        {
            get => _isActiveAnchorHeadSubProperty.Value;
            set
            {
                _isActiveAnchorHeadSubProperty.Value = (IsActiveAnchorHead && !IsActiveColorPallete);
            }
        }
        private readonly Property<bool> _isActiveColorPalleteProperty = new Property<bool>();
        public bool IsActiveColorPallete
        {
            get => _isActiveColorPalleteProperty.Value;
            set
            {
                _isActiveColorPalleteProperty.Value = value;
                SetValue("IsActiveAnchorHeadSub", !value);
            }
        }
        private readonly Property<bool> _isActiveCaptureProperty = new Property<bool>();
        public bool IsActiveCapture
        {
            get => _isActiveCaptureProperty.Value;
            set => _isActiveCaptureProperty.Value = value;
        }
        private readonly Property<bool> _isActiveColorButtonProperty = new Property<bool>();
        public bool IsActiveColorButton
        {
            get => _isActiveColorButtonProperty.Value;
            set => _isActiveColorButtonProperty.Value = value;
        }

        private readonly Property<bool> _isHairPartProperty = new Property<bool>();
        public bool IsHairPart
        {
            get => _isHairPartProperty.Value;
            set
            {
                _isHairPartProperty.Value = value;
                if(IsActiveAnchorHead)
                    OnChangeAnchor?.Invoke(AvatarPartsType.Hair, _isHairPartProperty.Value);
            }
        }
        private readonly Property<bool> _isBeardPartProperty = new Property<bool>();
        public bool IsBeardPart
        {
            get => _isBeardPartProperty.Value;
            set
            {
                _isBeardPartProperty.Value = value;
                if (IsActiveAnchorHead)
                    OnChangeAnchor?.Invoke(AvatarPartsType.Beard, _isBeardPartProperty.Value);
            }
        }
        private readonly Property<bool> _isSkinPartProperty = new Property<bool>();
        public bool IsSkinPart
        {
            get => _isSkinPartProperty.Value;
            set
            {
                _isSkinPartProperty.Value = value;
                if (IsActiveAnchorHead)
                    OnChangeAnchor?.Invoke(AvatarPartsType.Head, _isSkinPartProperty.Value);
            }
        }
        private readonly Property<bool> _isHatPartProperty = new Property<bool>();
        public bool IsHatPart
        {
            get => _isHatPartProperty.Value;
            set
            {
                _isHatPartProperty.Value = value;
                if (IsActiveAnchorHead)
                    OnChangeAnchor?.Invoke(AvatarPartsType.Hats, _isHatPartProperty.Value);
            }
        }
        private readonly Property<bool> _isGlassesPartProperty = new Property<bool>();
        public bool IsGlassesPart
        {
            get => _isGlassesPartProperty.Value;
            set
            {
                _isGlassesPartProperty.Value = value;
                if (IsActiveAnchorHead)
                    OnChangeAnchor?.Invoke(AvatarPartsType.Glasses, _isGlassesPartProperty.Value);
            }
        }
        private readonly Property<bool> _isEarringsPartProperty = new Property<bool>();
        public bool IsEarringsPart
        {
            get => _isEarringsPartProperty.Value;
            set
            {
                _isEarringsPartProperty.Value = value;
                if (IsActiveAnchorHead)
                    OnChangeAnchor?.Invoke(AvatarPartsType.Earrings, _isEarringsPartProperty.Value);
            }
        }
        private readonly Property<bool> _isTopPartProperty = new Property<bool>();
        public bool IsTopPart
        {
            get => _isTopPartProperty.Value;
            set
            {
                _isTopPartProperty.Value = value;
                if(IsActiveAnchorTopBottom)
                    OnChangeAnchor?.Invoke(AvatarPartsType.Top, _isTopPartProperty.Value);
            }
        }
        private readonly Property<bool> _isBottomPartProperty = new Property<bool>();
        public bool IsBottomPart
        {
            get => _isBottomPartProperty.Value;
            set
            {
                _isBottomPartProperty.Value = value;
                if(IsActiveAnchorTopBottom)
                    OnChangeAnchor?.Invoke(AvatarPartsType.Bottom, _isBottomPartProperty.Value);
            }
        }
        private readonly Property<bool> _isBagPartProperty = new Property<bool>();
        public bool IsBagPart
        {
            get => _isBagPartProperty.Value;
            set
            {
                _isBagPartProperty.Value = value;
                if (IsActiveAnchorTopBottom)
                    OnChangeAnchor?.Invoke(AvatarPartsType.Bags, _isBagPartProperty.Value);
            }
        }

        private readonly Property<bool> _isGlovePartProperty = new Property<bool>();
        public bool IsGlovePart
        {
            get => _isGlovePartProperty.Value;
            set
            {
                _isGlovePartProperty.Value = value;
                if(IsActiveAnchorHand)
                    OnChangeAnchor?.Invoke(AvatarPartsType.Glove, _isGlovePartProperty.Value);
            }
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
        public Action onClickNext;
        public void OnClickNext()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickNext?.Invoke();
        }
        public Action onClickColor;
        public void OnClickColor()
        {
            if(Input.touchCount >= 2)
            {
                return;
            }
            onClickColor?.Invoke();
        }
        public Action<AvatarPartsType, bool> OnChangeAnchor;

    }
}
