namespace MindPlus.Contexts.Master
{
    using Slash.Unity.DataBind.Core.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ColorPalleteViewContext : Context
    {
        public Action onClickDone;
        public void OnClickDone()
        {
            if (Input.touchCount >= 2)
            {
                return;
            }

            onClickDone?.Invoke();
        }
    }
}
