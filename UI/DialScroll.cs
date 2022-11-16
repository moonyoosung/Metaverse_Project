using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class DialScroll : MonoBehaviour
{
    public string ID;
    public VerticalScrollSnap scrollSnap;
    private List<string> dials;
    public void Initialize(Dial dial, DateTime Now)
    {
        scrollSnap.Initialize();
        SetDial(dial, Now);
    }
    private void SetDial(Dial prefab, DateTime Now)
    {
        dials = CreateDialList(Now);

        foreach (var item in dials)
        {
            Dial dial = Instantiate<Dial>(prefab, scrollSnap._scroll_rect.content.transform, false);
            dial.text.text = item;
            scrollSnap.AddChild(dial.gameObject);
            dial.transform.SetAsFirstSibling();
        }
    }
    private List<string> CreateDialList(DateTime now)
    {
        List<string> result = new List<string>();
        switch (ID)
        {
            case "year":
                for (int i = now.Year; i < now.Year + 10; i++)
                {
                    result.Add(i.ToString());
                }
                break;
            case "month":
                for (int i = 0; i < 13; i++)
                {
                    result.Add(i.ToString());
                }
                break;
            case "day":
                for (int i = 0; i < 32; i++)
                {
                    result.Add(i.ToString());
                }
                break;
            case "hour":
                for (int i = 0; i < 24; i++)
                {
                    result.Add(i.ToString());
                }
                break;
            case "minute":
                for (int i = 0; i < 60; i++)
                {
                    result.Add(i.ToString());
                }
                break;

            default:
                Debug.LogError(this.gameObject.name + "Not Set ID");
                break;
        }
        return result;
    }

    public void SetPage(DateTime now)
    {
        switch (ID)
        {
            case "year":
                scrollSnap.GoToScreen(0);
                break;
            case "month":
                scrollSnap.GoToScreen(now.Month);
                break;
            case "day":
                scrollSnap.GoToScreen(now.Day);
                break;
            case "hour":
                scrollSnap.GoToScreen(now.Hour);
                break;
            case "minute":
                scrollSnap.GoToScreen(now.Minute);
                break;

            default:
                Debug.LogError("Not Found ID : " + ID);
                break;
        }
    }
    public string GetSelect()
    {
        return dials[scrollSnap.CurrentPage];
    }
}
