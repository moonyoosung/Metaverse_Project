using System;
using UnityEngine;
public abstract class PluginHandler : MonoBehaviour
{
    //공통의 기능 정리
    public abstract void Initialize();
    public abstract bool CheckPermission();
    public abstract void Authorize(Action<bool> result = null);
    public abstract bool IsAuthorized();
    public abstract void SetAuthorized(bool isOn);
}
