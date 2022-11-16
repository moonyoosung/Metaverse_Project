using DG.Tweening;
using UnityEngine;

public abstract class UIAnimation : MonoBehaviour
{
    public abstract Sequence ShowSequences(float insertTime, float duration);
    public abstract Sequence HideSequences(float insertTime, float duration);
    public abstract Sequence ShowSequences();
    public abstract Sequence HideSequences();
}
