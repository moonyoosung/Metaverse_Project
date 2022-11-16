using System.Collections.Generic;
using UnityEngine;

public class StreamPool
{
    private List<GameObject> pool = new List<GameObject>();
    private GameObject instanceObject;
    private GameObject parent;
    public StreamPool(GameObject instanceObject, int cnt = 10)
    {
        parent = new GameObject("[POOL]" + instanceObject.name);
        this.instanceObject = instanceObject;
        for (int i = 0; i < cnt; i++)
        {
            GameObject instance = GameObject.Instantiate(this.instanceObject, parent.transform);
            instance.SetActive(false);
            pool.Add(instance);
        }
    }

    public GameObject Get(Transform parent = null)
    {
        foreach (var obj in pool)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        Transform anchor = parent == null ? this.parent.transform : parent;
        GameObject newObj = GameObject.Instantiate(this.instanceObject, anchor);
        newObj.SetActive(true);
        pool.Add(newObj);

        return newObj;
    }
}
