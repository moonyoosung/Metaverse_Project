using Slash.Unity.DataBind.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Contexts.Master.Menus.ChallengeView
{
    public class CallendarViewContext : Context
    {
        public Action onClickToday;
        public void OnClickToday()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }
            onClickToday?.Invoke();
        }
    }
}
