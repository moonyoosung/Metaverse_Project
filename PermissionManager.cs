using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#elif UNITY_IOS
using UnityEngine.iOS;
#endif

public class PermissionManager
{
    private bool isCoroutineing = false;

    //사용자에게 권한 요청 팝업을 띄운다.
    //Ios 최초 1회 거절 시 무조건 거부
    //Android 2회 거절 시 무조건 거부
    public void RequestUserPermission(object permission)
    {
#if PLATFORM_ANDROID
        Permission.RequestUserPermission((string)permission);
#elif UNITY_IOS
        Application.RequestUserAuthorization((UserAuthorization)permission);
#endif
    }

    //권한 여부를 확인
    public bool CheckPermission(object permission)
    {
        bool isOK = false;
#if PLATFORM_ANDROID
        isOK = Permission.HasUserAuthorizedPermission((string)permission);
#elif UNITY_IOS
        isOK  = Application.HasUserAuthorization((UserAuthorization)permission); 
#endif
        return isOK;
    }
    
    //TODO 지울 것
    public void OpenAppSetting()
    {
#if PLATFORM_ANDROID
        //Android 7.0 버전 이상만 될 가능성 있음
        try
        {
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity")) 
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

    public IEnumerator DoCheck(object permission, AppSettingPopup appSettingPopup = null, Action onAction = null)
    {
        if (!isCoroutineing)
        {
            Debug.Log("Voice DoCheck isCoroutineing");
            isCoroutineing = true;
            //프레임 완전히 종료 후
            yield return new WaitForEndOfFrame();

            //권한 확인
            if (!CheckPermission(permission))
            {
                //RequestUserPermission(permission);
                //기기 권한 요청 팝업 띄우기
#if PLATFORM_ANDROID
                Permission.RequestUserPermission((string)permission);
#elif UNITY_IOS
                yield return Application.RequestUserAuthorization((UserAuthorization)permission);
#endif
                yield return new WaitForSeconds(0.125f);

                //권한 요청 팝업창이 뜨면서 포커스가 false가 됨
                //권한 요청 팝업창이 종료되면 다음 코드 실행
                yield return new WaitUntil(() => Application.isFocused == true);
                yield return new WaitForSeconds(0.125f);
                //권한 재확인
                if (!CheckPermission(permission))
                {
                    //사용자 수동 권한 조작 -> 테스트 팝업 활성화
                    if (appSettingPopup)
                    {
                        appSettingPopup.gameObject.SetActive(true);
                        //팝업 종료 시 코루틴 종료
                        yield return new WaitUntil(() => !appSettingPopup.gameObject.activeSelf == true); 
                    }
                    isCoroutineing = false;
                    yield break;
                }
            }
            //사용자 마이크 찾고 설정
            if (onAction != null) onAction.Invoke();
            isCoroutineing = false;
            yield break;
        }
    }
}
