using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SetCanvasCamera : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        if (photonView != null && photonView.IsMine)
        {
            foreach (var item in GameObject.FindObjectsOfType<Canvas>())
            {
                item.worldCamera = GetComponent<Camera>();
            }
        }
    }
}
