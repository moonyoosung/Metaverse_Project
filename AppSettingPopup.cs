using MindPlus.Contexts.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AppSettingPopup : MonoBehaviour
{
    private MainViewContext context;

    public void Initialize(Persistent persistent, MainViewContext context)
    {
        this.context = context;
        this.context.onClickReject += OnReject;
        this.context.onClickAreement += OnAreement;
        persistent.VoiceManager.appSettingPopup = this;
        gameObject.SetActive(false);
    }

    void OnAreement()
    {
        gameObject.SetActive(false);
        OpenAppSetting();
    }

    void OnReject()
    {
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        this.context.onClickReject -= OnReject;
        this.context.onClickReject -= OnAreement;
    }

    public void OpenAppSetting()
    {
#if PLATFORM_ANDROID
        //Android 7.0 버전 이상만 될 가능성 있음
        try
        {
            //Java 클래스 생성
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity")) //Java 오브젝트 가져오기
            {
                string packageName = currentActivityObject.Call<string>("getPackageName");

                using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                {
                    intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                    intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                    currentActivityObject.Call("startActivity", intentObject);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

#elif UNITY_IOS
        try
        {
            //IOS 설정 창 -> 개인 정보
            Application.OpenURL("app-settings:Privacy");
            Debug.Log("PermissionManager::OpenURL(app-settings:Privacy)");
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);

            try
            {
                //IOS 설정 창으로 이동
                Application.OpenURL("app-settings:");
                Debug.Log("PermissionManager::OpenURL(app-settings)");
            }
            catch (System.Exception)
            {
                Debug.LogException(ex);
                throw;
            }
            throw;
        }
#endif
    }
}

