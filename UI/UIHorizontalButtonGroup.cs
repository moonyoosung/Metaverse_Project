using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIHorizontalButtonGroup : UILayoutGroup
{
    public Button more;
    public enum Division
    {
        World,
        People
    }
    public Division division;

    public override void Set()
    {
        if (!more) return;
        more.onClick.RemoveAllListeners();
        more.onClick.AddListener(() => { OnClickMore(); });
        more.gameObject.SetActive(false);
    }

    private void OnClickMore()
    {
        if (division == Division.World)
        {
            SetWorldContent();
        }
        else if(division == Division.People)
        {
            SetPeopleContent();
        }
    }

    private void SetWorldContent()
    {
        WorldView worldView = UIView.Get<WorldView>();
        ContentListView contentListView = UIView.Get<ContentListView>();
        contentListView.Set(ID);
        worldView.Set(true);
        worldView.Push("", false, true, null, contentListView);
    }

    private void SetPeopleContent()
    {
        Debug.Log("ID :: " + ID); 
        Debug.Log("Division :: " + division);
        PeopleView peopleView = UIView.Get<PeopleView>();
        PeopleListView peopleListView = UIView.Get<PeopleListView>();
        peopleListView.Set(ID);
        peopleView.Set(true,false);
        peopleView.Push("",  false, true, null, peopleListView);
    }

    public override bool IsADDAvailable()
    {
        if (group.transform.childCount >= 4)
        {
            if (!more.gameObject.activeSelf) more.gameObject.SetActive(true);
            return false;
        }
        more.gameObject.SetActive(false);
        return true;
    }
}
