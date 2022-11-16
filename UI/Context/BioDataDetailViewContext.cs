using Slash.Unity.DataBind.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Contexts.Master.Menus.WellnessView
{
    public class BioDataDetailViewContext : Context
    {
        public Action<int> onClickSetData;
        public void OnClickSetData(float direction)
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickSetData?.Invoke((int)direction);
        }
        private readonly Property<string> _averageTextProperty = new Property<string>();
        public string AverageText
        {
            get => _averageTextProperty.Value; set => _averageTextProperty.Value = value;
        }
        private readonly Property<string> _thisWeekTextProperty = new Property<string>();
        public string ThisWeekText
        {
            get => _thisWeekTextProperty.Value; set => _thisWeekTextProperty.Value = value;
        }
        private readonly Property<string> _dateTextProperty = new Property<string>();
        public string Date
        {
            get => _dateTextProperty.Value; set => _dateTextProperty.Value = value;
        }
        private readonly Property<string> _averageValueProperty = new Property<string>();
        public string AverageValue
        {
            get => _averageValueProperty.Value; set => _averageValueProperty.Value = value;
        }
        private readonly Property<string> _dataUnitProperty = new Property<string>();
        public string DataUnit
        {
            get => _dataUnitProperty.Value; set => _dataUnitProperty.Value = value;
        }
        private readonly Property<string> _graphUnitProperty = new Property<string>();
        public string GraphUnit
        {
            get => _graphUnitProperty.Value;set => _graphUnitProperty.Value = value;
        }
        private readonly Property<string> _lastTimeTextProperty = new Property<string>();
        public string LastTime
        {
            get => _lastTimeTextProperty.Value; set => _lastTimeTextProperty.Value = value;
        }
        private readonly Property<string> _lastValueProperty = new Property<string>();
        public string LastValue
        {
            get => _lastValueProperty.Value; set => _lastValueProperty.Value = value;
        }
        private readonly Property<string> _rangeValueProperty = new Property<string>();
        public string RangeValue
        {
            get => _rangeValueProperty.Value; set => _rangeValueProperty.Value = value;
        }
        private readonly Property<string> _descriptionTitleProperty = new Property<string>();
        public string DescriptionTitle
        {
            get => _descriptionTitleProperty.Value; set => _descriptionTitleProperty.Value = value; 
        }
        private readonly Property<string> _descriptionProperty = new Property<string>();
        public string Description
        {
            get => _descriptionProperty.Value; set => _descriptionProperty.Value = value;
        }
        private readonly Property<bool> _isActiveDescription = new Property<bool>();
        public bool IsActiveDescription
        {
            get => _isActiveDescription.Value; set => _isActiveDescription.Value = value;
        }
    }
}

