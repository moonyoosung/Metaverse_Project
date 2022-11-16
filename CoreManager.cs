using Looxid.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Looxid.Link;

public class CoreManager : MonoBehaviour
{
    public LooxidLinkMessage looxidLinkMessage;

    public IEnumerator Start()
    {
        looxidLinkMessage = GameObject.FindObjectOfType<LooxidLinkMessage>();

        Initialize();
        yield return new WaitForSeconds(1);
        StartDevice();

        StartCoroutine(CheckLeadOffState());
    }

    public void Initialize()
    {
        LooxidCoreManager.Instance.Initialize();
    }

    public void StartDevice()
    {
        LooxidCoreManager.Instance.StartDevice();
        LooxidCoreManager.Instance.StartStreaming();
    }

    IEnumerator CheckLeadOffState()
    {
        while (true)
        {
            yield return null;

            if (LooxidCoreManager.Instance.isDeviceOpen)
            {
                looxidLinkMessage.HideMessage(LooxidLinkMessageType.HubDisconnected);


            }
            else
            {
                looxidLinkMessage.ShowMessage(LooxidLinkMessageType.HubDisconnected);
            }
        }
    }

    public void StopDevice()
    {
        //LooxidCoreManager.Instance.StopStreaming();
        LooxidCoreManager.Instance.StopDevice();
    }

    public void StartCalibration()
    {
        LooxidCoreManager.Instance.StartCalibration();
    }

    public void StopCalibration()
    {
        /*
        return;

        if (isCalibration)
        {
            isCalibration = false;
            LooxidCoreManager.Instance.StopCalibration();
        }
        */
    }


    public float GetAttention()
    {
        return (float)LooxidCoreManager.Instance.attention;
    }
}
