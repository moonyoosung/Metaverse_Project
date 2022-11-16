using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using Pixelplacement.TweenSystem;

[System.Serializable]
public class AnimationBlend
{
    [Range(0f, 100f)]
    public float value;
}

public class SkinnedMeshAnimation : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public List<AnimationBlend> blendList = new List<AnimationBlend>();

    public float speed = 0.3f;

    [Range(-1, 6)]
    public int currentIndex;

    private void Start()
    {
        //Tween.StopAll();
    }

    private void Update()
    {
        for (int i = 0; i < blendList.Count; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, blendList[i].value);
        }
    }


    void Idle()
    {
        for (int i = 0; i < blendList.Count; i++)
        {
            TweenBlend(i, blendList[i].value, 0, speed, 0);
        }
    }

    void SayAH(float endValue, float duration, float delay = 0, System.Action callback = null)
    {
        TweenBlend(2, blendList[2].value, endValue, duration, delay, callback);
    }

    void SayE(float endValue, float duration, float delay = 0, System.Action callback = null)
    {
        TweenBlend(3, blendList[3].value, endValue, duration, delay, callback);
    }

    void SayI(float endValue, float duration, float delay = 0, System.Action callback = null)
    {
        TweenBlend(4, blendList[4].value, endValue, duration, delay, callback);
    }

    void SayOH(float endValue, float duration, float delay = 0, System.Action callback = null)
    {
        TweenBlend(5, blendList[5].value, endValue, duration, delay, callback);
    }

    void SayWoo(float endValue, float duration, float delay = 0, System.Action callback = null)
    {
        TweenBlend(6, blendList[6].value, endValue, duration, delay, callback);
    }

    void TweenBlend(int index, float startValue, float endValue, float duration, float delay = 0, System.Action callback = null)
    {
        Tween.Value(startValue, endValue, (value) => blendList[index].value = value, duration, delay, null, Tween.LoopType.None, null, callback);
    }
}
