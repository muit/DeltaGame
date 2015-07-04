using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ScrollItem
{
    public System.Guid id;
    public string name;
    public int points;
    public Button.ButtonClickedEvent onClick;

    public ScrollItem(string name, int points) {
        this.name = name;
        this.points = points;
        id = new System.Guid();
    }
}

public class ScrollList : MonoBehaviour
{
    public GameObject scrollButton;
    public List<ScrollItem> itemList;

    public Transform contentPanel;

    void Start()
    {
    }

    public void PopulateList(List<ScrollItem> items = null)
    {
        if (items != null) {
            itemList = items;
        }

        //Reset List
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);

        foreach (ScrollItem item in itemList)
        {
            CreateItem(item);
        }
    }

    public System.Guid AddItem(string name, int points) {
        ScrollItem item = new ScrollItem(name, points);
        itemList.Add(item);
        CreateItem(item);
        return item.id;
    }

    void CreateItem(ScrollItem item)
    {
        GameObject newButton = Instantiate(scrollButton) as GameObject;
        ScrollButton button = newButton.GetComponent<ScrollButton>();
        button.id = item.id;
        button.gameObject.name = item.name;
        button.name.text = item.name;
        button.amount.text = "" + item.points;
        newButton.transform.SetParent(contentPanel);
    }

    public void OnClick()
    {
        Debug.Log("I done did something!");
    }

    public void SomethingElseToDo(GameObject item)
    {
        Debug.Log(item.name);
    }
}