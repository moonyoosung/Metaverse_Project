using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportAnchorRotate : MonoBehaviour
{
    public static Quaternion quaternion = Quaternion.Euler(0, 0, 0);

    public bool isProvider = false;
    public Transform childTr;

    private void Awake()
    {
        Rotate();
    }

    void LateUpdate()
    {
        Rotate();
    }

    void Rotate()
    {
        if (isProvider)
        {
            quaternion = transform.rotation;
        }
        else
        {
            transform.rotation = quaternion;
            childTr.localRotation = quaternion;
        }
    }
}
