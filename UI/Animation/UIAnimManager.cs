using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIAnimManager : MonoBehaviour
{
    public abstract class Stream : IEquatable<Stream>
    {
        public abstract string ID { get; }
        protected Sequence totalSequence;
        public bool Equals(Stream other)
        {
            return other.Equals(ID);
        }
        //Assets/A_MindPlus/0_Common/Prefabs/EventView.prefab
        public abstract IEnumerator Handle(UIAnimManager manager, MonoBehaviour caller);
        public void Kill(bool isCallBack = false)
        {
            if(totalSequence != null && totalSequence.IsActive())
            {
                totalSequence.Kill(isCallBack);
                totalSequence = null;
            }
        }
    }
    private const int POOLCOUNT = 5;
    private StreamPool pool;
    private List<StreamQueue> queues = new List<StreamQueue>();
    public bool IsBusy()
    {
        foreach (var queue in queues)
        {
            if (queue.IsBusy())
            {
                return true;
            }
        }

        return false;
    }
    public void Push(Stream stream, string queueID = "")
    {
        foreach (var queue in queues)
        {
            if (queue.ID == queueID)
            {
                queue.Push(stream);
                return;
            }
        }
        AddEventQueue(queueID);
        Push(stream, queueID);
    }
    public void Interrupt(Stream stream, string queueID = "")
    {
        foreach (var queue in queues)
        {
            if (queue.ID == queueID)
            {
                queue.Interrupt(this, stream);
                return;
            }
        }
        AddEventQueue(queueID);
        Interrupt(stream, queueID);
    }
    public bool IsBusy(string queueID)
    {
        foreach (var queue in queues)
        {
            if (queue.ID == queueID)
            {
                return queue.IsBusy();
            }
        }

        return false;
    }
    private void AddEventQueue(string ID)
    {
        StreamQueue queue = pool.Get().GetComponent<StreamQueue>();
        queue.Initialize(this, ID);
        queues.Add(queue);
    }
    public IEnumerator Initialize(GameObject streamQueue)
    {
        pool = new StreamPool(streamQueue, POOLCOUNT);
        yield return null;
        StartCoroutine(Handle());
    }
   
    private IEnumerator Handle()
    {
        while (true)
        {
            for (int i = 0; i < queues.Count; i++)
            {
                if (!queues[i].IsBusy())
                {
                    queues[i].gameObject.SetActive(false);
                    queues.RemoveAt(i);
                }
            }
            yield return null;
        }
    }
}
