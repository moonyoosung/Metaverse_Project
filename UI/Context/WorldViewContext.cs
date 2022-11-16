namespace MindPlus.Contexts.Master.Menus
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class WorldViewContext : Context
    {
        #region "Object"
        private readonly Property<bool> _propertySlider = new Property<bool>();
        public bool SliderActive
        {
            get => _propertySlider.Value;
            set => _propertySlider.Value = value;
        }
        private readonly Property<bool> _propertyIsActiveDivider = new Property<bool>();
        public bool IsActiveDivider
        {
            get => _propertyIsActiveDivider.Value;
            set => _propertyIsActiveDivider.Value = value;
        }
        private readonly Property<bool> _propertyIsActiveBalance = new Property<bool>();
        public bool IsActiveBalance
        {
            get => _propertyIsActiveBalance.Value;
            set => _propertyIsActiveBalance.Value = value;
        }
        private readonly Property<bool> _propertyIsActiveCreate = new Property<bool>();
        public bool IsActiveCreate
        {
            get => _propertyIsActiveCreate.Value;
            set => _propertyIsActiveCreate.Value = value;
        }
        private readonly Property<bool> _propertyIsActiveJoin = new Property<bool>();
        public bool IsActiveJoin
        {
            get => _propertyIsActiveJoin.Value;
            set => _propertyIsActiveJoin.Value = value;
        }
        private readonly Property<bool> _propertyIsActiveStarIcon = new Property<bool>();
        public bool IsActiveStarIcon
        {
            get => _propertyIsActiveStarIcon.Value;
            set => _propertyIsActiveStarIcon.Value = value;
        }
        private readonly Property<bool> _propertyIsAlbe = new Property<bool>();
        public bool IsAlbe
        {
            get => _propertyIsAlbe.Value;
            set => _propertyIsAlbe.Value = value;
        }
        #endregion

        public Action<int> onClickSlider;
        public void OnClickSlider(float idx)
        {
            onClickSlider?.Invoke((int)idx);
        }
        public Action onClickBack;
        public void OnClickBack()
        {
            onClickBack?.Invoke();
        }
        public Action onClickClose;
        public void OnClickClose()
        {
            onClickClose?.Invoke(); 
        }
        public Action onClickCreate;
        public void OnClickCreate()
        {
            onClickCreate?.Invoke();
        }
        public Action onClickJoin;
        public void OnClickJoin()
        {
            onClickJoin?.Invoke();
        }
        public Action onClickPlayWithFriends;
        public void OnClickPlayWithFriends()
        {
            onClickPlayWithFriends?.Invoke();
        }

        private readonly Property<bool> _propertyTitleInteract = new Property<bool>();
        public bool IsTitleInteract
        {
            get => _propertyTitleInteract.Value;
            set => _propertyTitleInteract.Value = value;
        }
        private readonly Property<Sprite> _propertyTitleIcon = new Property<Sprite>();
        public Sprite TitleIcon
        {
            get => _propertyTitleIcon.Value;
            set => _propertyTitleIcon.Value = value;
        }
        private readonly Property<string> _propertyTitleText = new Property<string>();
        public string TitleText
        {
            get => _propertyTitleText.Value;
            set => _propertyTitleText.Value = value;
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
