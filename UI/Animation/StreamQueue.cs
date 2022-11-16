
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StreamQueue : MonoBehaviour
{
    public string ID;
    private Queue<UIAnimManager.Stream> streams = new Queue<UIAnimManager.Stream>();
    public void Initialize(UIAnimManager manager, string ID)
    {
        this.ID = ID;
        StartCoroutine(HandleEvent(manager));
    }
    public bool IsBusy()
    {
        return streams.Count > 0;
    }
    public void Push(UIAnimManager.Stream stream)
    {
        if (streams.Contains(stream))
        {
            //Debug.Log(ID + "\t Push Fail Stream : " + stream.ID + " / Count :" + streams.Count);
            return;
        }

        //Debug.Log(ID + " \t Push Stream / " + stream.ID + " / Count :" + streams.Count);
        streams.Enqueue(stream);
    }
    public void Interrupt(UIAnimManager manager, UIAnimManager.Stream stream)
    {
        Debug.Log(ID + "\t Interrupt Event : " + stream.ID + " / Count :" + streams.Count);
        if (streams.Count <= 0)
        {
            Push(stream);
            return;
        }

        StopAllCoroutines();
        streams.Peek().Kill();
        streams.Clear();
        streams.Enqueue(stream);
        Debug.Log(ID + "Kill");
        StartCoroutine(HandleEvent(manager));
    }
    private IEnumerator HandleEvent(UIAnimManager manager)
    {
        while (true)
        {
            if (streams.Count > 0)
            {
                yield return StartCoroutine(streams.Peek().Handle(manager, this));
                streams.Dequeue();
            }
            yield return null;
        }
    }
}
