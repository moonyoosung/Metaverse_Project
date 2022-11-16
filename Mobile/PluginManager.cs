using System;
using UnityEngine;

public class PluginManager : MonoBehaviour
{
    //IOS OR Android 
    public PluginHandler Handler { private set; get; }
    public void Initialize()
    {
#if UNITY_IOS || UNITY_EDITOR_OSX
            this.Handler = new GameObject("PluginHandler").AddComponent<IOSPluginHandler>();
#elif UNITY_ANDROID || UNITY_EDITOR_WIN

#endif
        if(Handler == null)
        {
            //Debug.Log("Not Load PluginHandler");
            return;
        }

        Handler.transform.SetParent(this.transform);
        this.Handler.Initialize();
    }

}
