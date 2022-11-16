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

    //����ڿ��� ���� ��û �˾��� ����.
    //Ios ���� 1ȸ ���� �� ������ �ź�
    //Android 2ȸ ���� �� ������ �ź�
    public void RequestUserPermission(object permission)
    {
#if PLATFORM_ANDROID
        Permission.RequestUserPermission((string)permission);
#elif UNITY_IOS
        Application.RequestUserAuthorization((UserAuthorization)permission);
#endif
    }

    //���� ���θ� Ȯ��
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
    
    //TODO ���� ��
    public void OpenAppSetting()
    {
#if PLATFORM_ANDROID
        //Android 7.0 ���� �̻� �� ���ɼ� ����
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
            //IOS ���� â -> ���� ����
            Application.OpenURL("app-settings:Privacy");
            Debug.Log("PermissionManager::OpenURL(app-settings:Privacy)");
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);

            try
            {
                //IOS ���� â���� �̵�
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
            //������ ������ ���� ��
            yield return new WaitForEndOfFrame();

            //���� Ȯ��
            if (!CheckPermission(permission))
            {
                //RequestUserPermission(permission);
                //��� ���� ��û �˾� ����
#if PLATFORM_ANDROID
                Permission.RequestUserPermission((string)permission);
#elif UNITY_IOS
                yield return Application.RequestUserAuthorization((UserAuthorization)permission);
#endif
                yield return new WaitForSeconds(0.125f);

                //���� ��û �˾�â�� �߸鼭 ��Ŀ���� false�� ��
                //���� ��û �˾�â�� ����Ǹ� ���� �ڵ� ����
                yield return new WaitUntil(() => Application.isFocused == true);
                yield return new WaitForSeconds(0.125f);
                //���� ��Ȯ��
                if (!CheckPermission(permission))
                {
                    //����� ���� ���� ���� -> �׽�Ʈ �˾� Ȱ��ȭ
                    if (appSettingPopup)
                    {
                        appSettingPopup.gameObject.SetActive(true);
                        //�˾� ���� �� �ڷ�ƾ ����
                        yield return new WaitUntil(() => !appSettingPopup.gameObject.activeSelf == true); 
                    }
                    isCoroutineing = false;
                    yield break;
                }
            }
            //����� ����ũ ã�� ����
            if (onAction != null) onAction.Invoke();
            isCoroutineing = false;
            yield break;
        }
    }
}
