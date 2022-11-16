using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public abstract class PlayerTouchDrag : MonoBehaviour
{
    public Vector2 inputDirection;
    public System.Action<Vector2> InputAction;
    public System.Action<Vector2> onBeginDrag;
    public System.Action<Finger> onDrag;
    public System.Action onEndDrag;

    public Finger finger;

    public virtual void SetFinger(Finger finger)
    {
    }
}
