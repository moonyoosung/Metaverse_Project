using Slash.Unity.DataBind.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Contexts.Pool
{
    public class UIArticleContext : Context
    {
        public Action onClickArticle;
        public void OnClickArticle()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickArticle?.Invoke();
        }
        private readonly Property<string> _titleProperty = new Property<string>();
        public string Title
        {
            get => _titleProperty.Value;
            set => _titleProperty.Value = value;
        }
        private readonly Property<Sprite> _imageProperty = new Property<Sprite>();
        public Sprite Image
        {
            get => _imageProperty.Value;
            set => _imageProperty.Value = value;
        }
    }
}
