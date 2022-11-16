using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMainPanelView : UIFlatView
{
    private Persistent persistent;
    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        //PlayerPrefs.DeleteKey("userId");    TODO  
        this.persistent = persistent;
    }

    public override void OnStartShow()
    {
        base.OnStartShow();

        if(navigation.history.Count == 0)
        {
            string userid = string.Empty;//PlayerPrefs.GetString("userId");
            string pasword = string.Empty;//PlayerPrefs.GetString("pw");

            if (!string.IsNullOrEmpty(userid) && !string.IsNullOrEmpty(pasword))
            {
            }
            else
            {
                Push(this.gameObject.name, false, false, null, Get<LoginView>());
            }
        }
    }
    public override void OnStartHide()
    {
        base.OnStartHide();
    }
}
