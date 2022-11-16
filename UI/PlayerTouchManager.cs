using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

[System.Serializable]
public class PlayerTouch
{
    public Finger finger;
    public bool zoomBegin = false;
    public GameObject firstObject;
    public PlayerTouchDrag playerTouchDrag;

    public PlayerTouch() { }
    public PlayerTouch(Finger newFinger)
    {
        this.finger = newFinger;
    }
    public PlayerTouch(PlayerTouch clone)
    {
        SetFinger(clone.finger);
        this.zoomBegin = clone.zoomBegin;
        this.firstObject = clone.firstObject;
        this.playerTouchDrag = clone.playerTouchDrag;
    }

    public void Init()
    {
        zoomBegin = false;
        firstObject = null;
        if (playerTouchDrag != null)
        {
            if (playerTouchDrag.onEndDrag != null)
                playerTouchDrag.onEndDrag.Invoke();
            playerTouchDrag = null;
        }
    }

    public void SetFinger(Finger newFinger)
    {
        finger = newFinger;
    }

    public bool CheckEnd()
    {
        if (!finger.currentTouch.valid)
            return false;

        switch (finger.currentTouch.phase)
        {
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                Init();
                return true;
                /*
            default:
                float halfPosX = Screen.width * 0.5f;
                if (finger.currentTouch.startScreenPosition.x < halfPosX)
                {
                    if (finger.currentTouch.screenPosition.x >= halfPosX)
                    {
                        Init();
                        return true;
                    }
                }
                else
                {
                    if (finger.currentTouch.screenPosition.x <= halfPosX)
                    {
                        Init();
                        return true;
                    }
                }
                break;
                */
        }
        return false;
    }

    public void CheckTouch()
    {
        switch (finger.currentTouch.phase)
        {
            case TouchPhase.Began:
                CheckBeginZoom(finger.currentTouch.screenPosition);
                break;
            case TouchPhase.Moved:
                CheckBeginZoom(finger.currentTouch.screenPosition);
                if (playerTouchDrag != null && playerTouchDrag.onDrag != null)
                    playerTouchDrag.onDrag.Invoke(finger);
                break;
            case TouchPhase.Stationary:
                break;
            default:
                break;
        }
    }

    public void Update()
    {
        if (playerTouchDrag != null && playerTouchDrag.InputAction != null)
        {
            playerTouchDrag.InputAction.Invoke(playerTouchDrag.inputDirection);
        }
    }

    public void CheckBeginZoom(Vector2 pos)
    {
        if (firstObject)
            return;
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = finger.currentTouch.startScreenPosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            firstObject = raycastResults[0].gameObject;
            zoomBegin = string.Compare(firstObject.gameObject.name, "Rotate") == 0;

            if (zoomBegin)
                PlayerCameraSetting.fingerIndex = finger.index;

            playerTouchDrag = firstObject.GetComponent<PlayerTouchDrag>();
            if (playerTouchDrag == null)
                playerTouchDrag = firstObject.GetComponentInParent<PlayerTouchDrag>();

            if (playerTouchDrag != null)
                playerTouchDrag.SetFinger(finger);
        }

        if (playerTouchDrag != null && playerTouchDrag.onBeginDrag != null)
            playerTouchDrag.onBeginDrag.Invoke(pos);
    }
}

public class PlayerTouchManager : MonoBehaviour
{
    [SerializeField]
    public List<PlayerTouch> touchList = new List<PlayerTouch>();

    public PlayerCameraSetting playerCameraSetting;

    private void Start()
    {
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
#if UNITY_EDITOR
        //TouchSimulation.Enable();
#endif
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
#if UNITY_EDITOR
        //TouchSimulation.Disable();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCameraSetting == null)
        {
            foreach (var item in GameObject.FindObjectsOfType<PlayerCameraSetting>())
            {
                if (item.photonView && item.photonView.IsMine)
                {
                    playerCameraSetting = item;
                    break;
                }
            }
            return;
        }
        foreach (var finger in Touch.activeFingers)
        {
            if (finger.currentTouch.isTap)
                continue;

            if (finger.currentTouch.phase == TouchPhase.Began)
                continue;

            PlayerTouch pt = touchList.Find((x) => x.finger.index == finger.index);

            if (pt == null)
            {
                pt = new PlayerTouch(finger);
                touchList.Add(pt);
            }
            pt.SetFinger(finger);
        }


        for (int i = touchList.Count - 1; i >= 0; i--)
        {
            if (touchList[i].CheckEnd())
            {
                touchList.RemoveAt(i);
            }
            else
            {
                touchList[i].CheckTouch();
                touchList[i].Update();
            }
        }
        if (Input.touchCount > 1 && touchList.Count > 1)
        {
            int indexA = touchList.Count - 2;
            int indexB = touchList.Count - 1;
            if (touchList[indexA].zoomBegin && touchList[indexB].zoomBegin)
            {
                //playerCameraSetting.Zoom(touchList[indexA].finger.currentTouch, touchList[indexB].finger.currentTouch);
                playerCameraSetting.Zoom(Input.GetTouch(0), Input.GetTouch(1));
            }
        }
    }

}
