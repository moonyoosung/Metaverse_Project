using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINavigation
{
    public string ID => id;
    private string id = "";
    public UIView Current
    {
        private set { current = value; }
        get { return current; }
    }
    private UIView current;
    public Stack<UIView> history = new Stack<UIView>();

    public UINavigation(string ID)
    {
        this.id = ID;
    }
    public UIView Push(UIView view)
    {

        Current = view;
        history.Push(view);
        Debug.Log(ID + " : Push / Current = " + current.name + " / History Count = " + history.Count);
        return Current;
    }
    public UIView Push(string viewName)
    {
        UIView UIView = UIView.Get(viewName);
        return Push(UIView);
    }
    public UIView Pop()
    {
        if (Current != null)
        {
            history.Pop();
        }

        if (history.Count == 0)
        {
            Current = null;
        }
        else
        {
            Current = history.Peek();
        }
        Debug.Log(ID + " : Pop / Current = " + current?.name + " / History Count = " + history.Count);
        return Current;
    }

    public UIView PopTo(string viewName)
    {
        UIView view = Pop();

        if (viewName != view.name)
        {
            return PopTo(viewName);
        }

        return view;
    }
    public UIView[] GetShowing(int stackCnt = 0)
    {
        UIView[] list = history.ToArray();
        List<UIView> result = new List<UIView>();
        for (int i = 0; i < list.Length; i++)
        {
            if (i > list.Length - stackCnt)
            {
                break;
            }

            if (list[i].IsShowing())
            {
                result.Add(list[i]);
            }
        }

        return result.ToArray();
    }
    public UIView PopToRoot(int stackCnt = 0)
    {
        UIView view = Current;

        if (history.Count != stackCnt)
        {
            view = Pop();
        }

        if (history.Count > stackCnt)
        {
            return PopToRoot(stackCnt);
        }

        return view;
    }

}
