

namespace MindPlus.Contexts.Master.Menus.WorldView
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using UnityEngine;
    public class EventViewContext : Context
    {
        public Action onClickFloating;
        public void OnClickFloating()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickFloating?.Invoke();
        }
    }
}
