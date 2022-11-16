using System;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using MindPlus;

public class SplashView : MonoBehaviour
{
    public Image title;
    public Image backgroundColor;

    [HideInInspector]
    public bool isMinRun = false;
    public bool isSplashEnd = false;

    private void Awake()
    {
        isMinRun = false;
        isSplashEnd = false;
        Color color = title.color;
        color.a = 0.05f;
        title.color = color;

        StartSplashView();
    }

    public void StartSplashView()
    {
        StartCoroutine(RunTimer(2.5f));
        FadeIn();
    }

    private void FadeIn()
    {
        title.DOFade(1, 1.25f);
    }

    public void FadeOut()
    {
        title.DOFade(0, 1.25f).OnComplete(() =>
        {
            isSplashEnd = true;
        });
    }

    private IEnumerator RunTimer(float maxTime)
    {
        if (isMinRun)
            yield break;

        float time = 0;

        while (time < maxTime)
        {
            yield return new WaitForSeconds(0.5f);
            time += 0.5f;
        }
        isMinRun = true;
    }

    //private IEnumerator Start()
    //{
    //    yield return null;
    //    AsyncOperation async = Application.LoadLevelAsync("Title");
    //    async.allowSceneActivation = false;
    //    FadeIn();



    //    //타이틀 이미지를 보여주는 최소 시간이 끝나면
    //    yield return new WaitUntil(() => isTimerEnd);

    //    async.allowSceneActivation = true;

    //    //게임매니저 초기화가 어느정도 진행 되었을 시
    //    yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.isInitalized);

    //    while (!async.isDone)
    //    {
    //        yield return null;
    //    }

    //    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Splash");
    //}
}
