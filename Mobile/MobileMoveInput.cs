using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class MobileMoveInput : PlayerTouchDrag
{
    public VirtualJoystick virtualJoystick;
    public bool isBegin = false;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3);
        virtualJoystick.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        onEndDrag += EndDrag;
        onBeginDrag += BeginDrag;
        onDrag += OnDrag;
    }

    private void OnDisable()
    {
        onEndDrag -= EndDrag;
        onBeginDrag -= BeginDrag;
        onDrag -= OnDrag;
    }

    private void BeginDrag(Vector2 pos)
    {
        StopAllCoroutines();
        isBegin = true;
        virtualJoystick.gameObject.SetActive(true);
        virtualJoystick.rectTransform.position = pos;
        virtualJoystick.InitializeJoystick();
        this.inputDirection = Vector2.zero;
    }

    private void OnDrag(Finger finger)
    {
        if (isBegin)
        {
            isBegin = false;
        }
        virtualJoystick.ControlJoystickLever(finger);
        this.inputDirection = virtualJoystick.inputDirection;
    }

    private void EndDrag()
    {
        virtualJoystick.gameObject.SetActive(false);
    }
}
