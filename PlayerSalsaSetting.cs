using CrazyMinnow.SALSA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSalsaSetting : MonoBehaviour
{
    // Start is called before the first frame update
    public IEnumerator Start()
    {
        Salsa salsa = null;
        while (salsa == null)
        {
            salsa = GetComponentInChildren<Salsa>();
            yield return null;
        }

        if (salsa != null)
        {
            salsa.audioSrc = GetComponentInChildren<AudioSource>(); 
        }
    }
}
