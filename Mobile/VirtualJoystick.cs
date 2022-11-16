using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;

public class VirtualJoystick : PlayerTouchDrag
{
    [SerializeField]
    public RectTransform lever;
    public RectTransform rectTransform;
    public RectTransform[] dots;

    [SerializeField, Range(10, 150)]
    private float leverRange;

    public enum JoystickType { ADD_DIRECTION, SET_POSITION }
    public JoystickType limitType = JoystickType.ADD_DIRECTION;

    private void OnEnable()
    {
    }

    public void InitializeJoystick()
    {
        lever.anchoredPosition = Vector2.zero;
        inputDirection = Vector2.zero;
        SetDots(rectTransform.position, rectTransform.position);
    }

    public void ControlJoystickLever(Finger finger)
    {
        Vector2 leverPos = Vector2.zero;
        switch (limitType)
        {
            case JoystickType.ADD_DIRECTION:
                /*
                var inputPos = pos - (Vector2)rectTransform.position;
                leverPos = inputPos.magnitude < leverRange ? inputPos : inputPos.normalized * leverRange;
                lever.position = leverPos + (Vector2)rectTransform.position;
                inputDirection = leverPos / leverRange;
                */
                Vector2 bodyPos = rectTransform.position;
                var inputPos = finger.currentTouch.screenPosition - bodyPos;
                if(inputPos.magnitude <= leverRange)
                {
                    leverPos = inputPos;
                }
                else
                {
                    leverPos = inputPos.normalized * leverRange;

                    bodyPos += inputPos.normalized * 15;

                    Vector2 dir = bodyPos - finger.currentTouch.startScreenPosition;
                    float value = 400;
                    if(dir.magnitude > value)
                    {
                        bodyPos = finger.currentTouch.startScreenPosition + dir.normalized * value;
                    }
                }
                SetDots(finger.currentTouch.startScreenPosition, bodyPos);

                lever.position = (Vector3)leverPos + rectTransform.position;
                inputDirection = leverPos / leverRange;
                rectTransform.position = bodyPos;
                break;
            case JoystickType.SET_POSITION:
                leverPos = finger.currentTouch.screenPosition;
                lever.position = leverPos;
                inputDirection = leverPos / leverRange;
                break;
            default:
                break;
        }
    }

    public void SetDots(Vector3 startPos, Vector3 endPos)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].position = Vector3.Lerp(startPos, endPos, i * 0.2f);
            dots[i].localScale = Vector3.one * Mathf.Lerp(0.3f, 0.6f, i * 0.2f);
        }
    }
}
