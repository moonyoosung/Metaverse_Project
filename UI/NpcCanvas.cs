using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class NpcCanvas : MonoBehaviour
{
    public SetSpeechBalloon setSpeechBalloon;
    public BillBoard billBoard;
    private float seconds;
    private float maxSeconds;
    private bool isToggle;
    private int ranIndex = 0;
    public string[] texts;
    public Text npcName;   //임시로 추가.. 필요한지 ?

    public void Initialize()
    {
        setSpeechBalloon.Initialize(false);

        ranIndex = UnityEngine.Random.Range(0, texts.Length);
        setSpeechBalloon.SetText(texts[ranIndex]);

        billBoard.enabled = true;
        seconds = 0;
        maxSeconds = 7f;
        isToggle = false;
        npcName.text = "Caleb";
    }

    public void SetBalloon(bool isActive = true)
    {
        setSpeechBalloon.SetBalloon(isActive);
    }

    private void ChangeText()
    {
        isToggle = !isToggle;

        if (++ranIndex >= texts.Length)
        {
            ranIndex = 0;
        }
        setSpeechBalloon.SetText(texts[ranIndex]);
    }

    void Update()
    {
        if (setSpeechBalloon.Context == null || !setSpeechBalloon.Context.IsActiveBalloon)
            return;

        if (seconds >= maxSeconds)
        {
            seconds = 0;
            ChangeText();
        }

        seconds += Time.deltaTime;
    }
}

