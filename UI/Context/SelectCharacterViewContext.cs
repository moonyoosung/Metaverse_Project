using Slash.Unity.DataBind.Core.Data;
using System;
using UnityEngine;

namespace MindPlus.Contexts.TitleView
{
    public class SelectCharacterViewContext : Context
    {
        #region "Button"
        public Action<bool> onClickHeadChange;
        public void OnClickHeadChange(bool isLeft)
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickHeadChange?.Invoke(isLeft);
        }
        public Action<bool> onClickHairChange;
        public void OnClickHairChange(bool isLeft)
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickHairChange?.Invoke(isLeft);
        }
        public Action<bool> onClickBottomChange;
        public void OnClickBottomChange(bool isLeft)
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickBottomChange?.Invoke(isLeft);
        }
        public Action<bool> onClickTopChange;
        public void OnClickTopChange(bool isLeft)
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickTopChange?.Invoke(isLeft);
        }
        public Action<bool> onClickItemChange;
        public void OnClickItemChange(bool isLeft)
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickItemChange?.Invoke(isLeft);
        }
        public Action onClickNext;
        public void OnClicknext()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickNext?.Invoke();
        }
        #endregion
    }
}