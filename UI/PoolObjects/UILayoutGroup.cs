using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class UILayoutGroupContainer
{
    public List<UILayoutGroup> groups;

    public UILayoutGroupContainer(List<UILayoutGroup> groups)
    {
        this.groups = groups;
    }

    public bool TryGetUILayourGroup<T>(string ID, out T result) where T : UILayoutGroup
    {
        foreach (var group in groups)
        {
            if(group.ID == ID)
            {
                result = group as T;
                return true;
            }
        }

        result = null;
        return false;
    }
    public void SetActiveGroup()
    {
        foreach (var group in groups)
        {
            if(group.gameObject.activeSelf == group.HasChild())
            {
                continue;
            }
            group.gameObject.SetActive(group.HasChild());
        }
    }
    public void ADD(UILayoutGroup group)
    {
        groups.Add(group);
    }
    public void Remove(UILayoutGroup group)
    {
        groups.Remove(group);
    }
}
public class UILayoutGroup : UIPoolObject
{
    public HorizontalOrVerticalLayoutGroup group;

    public virtual void Set()
    {
    }
    public virtual bool IsADDAvailable()
    {
        return true;
    }
    public virtual bool HasChild()
    {
        return group.transform.childCount > 0;
    }
}
