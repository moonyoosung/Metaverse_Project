using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPool : MonoBehaviour
{
    [Header("Setting")]
    public string ID = "";
    public UIPoolObject prefab;
    public int count = 5;
    private List<UIPoolObject> pools = new List<UIPoolObject>();

    public void Initialize()
    {
        if(ID == string.Empty)
        {
            ID = gameObject.name;
        }

        gameObject.name = "[POOL]" + prefab.gameObject.name;
        for (int i = 0; i < count; i++)
        {
            CreateInstatnce();
        }
    }
    public bool TryGetActivePool<T>(string ID, out T result) where T : UIPoolObject
    {
        foreach (var pool in pools)
        {
            if (!pool.gameObject.activeSelf)
            {
                continue;
            }

            if(pool.ID.Equals(ID))
            {
                result = pool as T;
                return true;
            }
        }

        result = null;
        return false;
    }
    public T Get<T>(Transform parent = null) where T : UIPoolObject
    {
        foreach (var poolObj in pools)
        {
            if (!poolObj.gameObject.activeSelf)
            {
                poolObj.gameObject.SetActive(true);
                poolObj.transform.SetParent(parent == null ? transform : parent);

                return poolObj as T;
            }
        }

        UIPoolObject obj = CreateInstatnce();

        obj.gameObject.SetActive(true);
        obj.transform.SetParent(parent == null ? transform : parent);

        return obj as T;
    }
    private UIPoolObject CreateInstatnce()
    {
        UIPoolObject instance = Instantiate(prefab, transform, false);
        instance.Initialize(transform);
        instance.gameObject.SetActive(false);
        pools.Add(instance);
        return instance;
    }
}
