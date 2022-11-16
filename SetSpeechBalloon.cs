using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using MindPlus.Contexts.Player;
using Slash.Unity.DataBind.Core.Presentation;

public class SetSpeechBalloon : MonoBehaviour
{
    public ContextHolder contextHolder;
    public SetSpeechBalloonContext Context { private set; get; }

    public void Initialize(bool isActive = false)
    {
        this.Context = new SetSpeechBalloonContext();
        contextHolder.Context = Context;

        Context.SetValue("IsActiveBalloon", isActive);
    }

    public void SetText(string msg)
    {
        string result = msg.TrimStart();
        Context.SetValue("SpeechBalloonText", result);
    }

    public void SetBalloon(bool isActive = true)
    {
        Context.SetValue("IsActiveBalloon", isActive);
    }
}