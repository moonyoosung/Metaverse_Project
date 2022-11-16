using Slash.Unity.DataBind.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Contexts.Pool
{
    public class UIBioDataContext : Context
    {
        
        private readonly Property<Color> _dotIconColorProperty = new Property<Color>();
        public Color DotIconColor
        {
            get => _dotIconColorProperty.Value;
            set => _dotIconColorProperty.Value = value;
        }
        private readonly Property<Color> _dataTextColorProperty = new Property<Color>();
        public Color DataTextColor
        {
            get => _dataTextColorProperty.Value;
            set => _dataTextColorProperty.Value = value;
        }
        private readonly Property<string> _titleTextProperty = new Property<string>();
        public string TitleText
        {
            get => _titleTextProperty.Value;set => _titleTextProperty.Value = value;
        }
        private readonly Property<string> _dataTextProperty = new Property<string>();
        public string DateText
        {
            get => _dataTextProperty.Value; set => _dataTextProperty.Value = value;
        }
        private readonly Property<string> _unitTextProperty = new Property<string>();
        public string UnitText
        {
            get => _unitTextProperty.Value; set => _unitTextProperty.Value = value;
        }
        public Action onClickDetail;



        public void OnClickDetail()
        {
            if(Input.touchCount >= 2)
            {
                return;
            }
   
            onClickDetail?.Invoke();
        }
    }
}

