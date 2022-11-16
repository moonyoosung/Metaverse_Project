using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingManager : MonoBehaviour
{
    public Sprite[] sprites;
    public string[] tips;
    public Image background;
    public Text tipText;

    void Start()
    {
        //씬에 해당되는 게임매니저 구독시켜야함

        if (sprites.Length <= 0)
        {
            //로딩 이미지가 없을 시 디폴트 이미지로?
            return;
        }

        LoadingView();
    }

    private int GetRandomIndex(int min, int max)
    {
        return Random.Range(min, max);
    }

    private void LoadingView()
    {
        background.sprite = sprites[GetRandomIndex(0, sprites.Length)];
        tipText.text = tips[GetRandomIndex(0, tips.Length)];
    }
}
