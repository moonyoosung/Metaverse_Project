using Slash.Unity.DataBind.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Contexts.Master.Menus.WellnessView
{
    public class AllowHealthAppViewContext : Context
    {
        public Action onClickAllowall;
        public void OnClickAllowall()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickAllowall?.Invoke();
        }
    }
}

