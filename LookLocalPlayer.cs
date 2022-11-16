using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LookLocalPlayer : MonoBehaviourPun
{
    public Transform targetTr;
    private PhotonView pv;

    public void Initialize(Transform localPlayer, PhotonView pv)
    {
        this.pv = pv;
        this.targetTr = localPlayer;
    }
    private void Update()
    {
        if(Camera.main)
            transform.LookAt(2 * transform.position - Camera.main.transform.position);
    }
}
