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
        //���� �ش�Ǵ� ���ӸŴ��� �������Ѿ���

        if (sprites.Length <= 0)
        {
            //�ε� �̹����� ���� �� ����Ʈ �̹�����?
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
