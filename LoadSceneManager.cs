using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Loading 관련 씬 핸들링하기 
/// </summary>
public class LoadSceneManager : MonoBehaviour
{
    public string beforeScene;

    public interface IEventHandler
    {
        void OnStartSceneChanged(Scene prev);
        IEnumerator OnEndScenechanged(Scene next);
    }

    private List<IEventHandler> eventHandlers = new List<IEventHandler>();

    public void Resister(IEventHandler eventHandler)
    {
        eventHandlers.Add(eventHandler);
    }
    public void UnResister(IEventHandler eventHandler)
    {
        eventHandlers.Remove(eventHandler);
    }

    public IEnumerator AdditiveSceneAsync(string sceneName, bool allowSceneActive = true, Action<Scene> action = null)
    {
        yield return new WaitForSeconds(1f);
        //Debug.Log("sceneName1 :: " + sceneName);
        //var temp = SceneManager.GetAllScenes();
        //for (int i = 0; i < temp.Length; i++)
        //{
        //    Debug.Log("sceneName2 :: " + temp[i].name);
        //}

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        if (asyncOperation == null) yield break;

        asyncOperation.allowSceneActivation = allowSceneActive;

        while (!asyncOperation.isDone)
        {
            yield return null;
        }


        Scene scene = SceneManager.GetSceneByName(sceneName);

        action?.Invoke(scene);
    }

    public void Change(string sceneName, Action complete = null, bool isAutoLoading = true)
    {
        StartCoroutine(ChangeScene(sceneName, complete, isAutoLoading));
    }
    public IEnumerator ChangeScene(string sceneName, Action complete = null, bool isAutoLoading = false)
    {
        Resources.UnloadUnusedAssets();
        yield return null;
        Scene current = SceneManager.GetActiveScene();
        beforeScene = current.name;
        AsyncOperation loadingAsyncOperation = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        loadingAsyncOperation.allowSceneActivation = true;

        while (!loadingAsyncOperation.isDone)
        {
            yield return null;
        }

        Scene loading = SceneManager.GetSceneByName("Loading");

        SceneManager.SetActiveScene(loading);

        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.OnStartSceneChanged(current);
        }

        AsyncOperation currentOP = SceneManager.UnloadSceneAsync(current);

        while (!currentOP.isDone)
        {
            yield return null;
        }

        Resources.UnloadUnusedAssets();

        AsyncOperation nextAsyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        nextAsyncOperation.allowSceneActivation = true;

        while (!nextAsyncOperation.isDone)
        {
            yield return null;
        }

        Scene next = SceneManager.GetSceneByName(sceneName);
        //if (VoiceManager.Instance.recorder)
        //{
        //    SceneManager.MoveGameObjectToScene(VoiceManager.Instance.ppvs.recorder.gameObject, next);
        //}
        SceneManager.SetActiveScene(next);

        foreach (var eventHandler in eventHandlers)
        {
            yield return eventHandler.OnEndScenechanged(next);
        }

        complete?.Invoke();
        if (isAutoLoading)
        {
            SceneManager.UnloadSceneAsync("Loading");
        }

        Resources.UnloadUnusedAssets();
        Debug.Log("LoadSceneComplete : " + next.name);
    }

    public void UnloadLoading()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == "Loading")
            {
                SceneManager.UnloadSceneAsync("Loading");
                break;
            }
        }
        Resources.UnloadUnusedAssets();
    }
}
