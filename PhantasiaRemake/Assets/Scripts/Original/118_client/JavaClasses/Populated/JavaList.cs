//import java.awt.JavaPanel;
//import java.awt.List;
//import java.awt.BorderLayout;
//import java.io.DataInputStream;
//import java.util.Vector;
//import java.awt.event.*;
using System;
using UnityEngine;
using UnityEngine.UI;

internal class JavaList : JavaComponent //The List component presents the user with a scrolling list of text items.
{
    UnityListComponents unityComponents;

    internal JavaList()
    {
        unityComponentGroup = UnityJavaInterface.AddListScroll();
        unityComponents = (UnityListComponents)unityComponentGroup;
    }

    internal void setFont(JavaFont userFont)
    {
        if (userFont != null)
        {
            foreach (GameObject item in unityComponents.contentListItems)
            {
                Text itemText = item.GetComponent<Text>();
                itemText.font = userFont.UnityFont;
                itemText.fontSize = userFont.fontSize;
                itemText.fontStyle = userFont.fontStyle;
            }
        }
        else
        {
            Debug.LogError("userFont doesn't exist");
        }
    }

    internal void addActionListener(userPne userPne)
    {
        //unnecessary for unity
    }

    internal void addItem(string v)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_addItem, v);
    }
    internal void M_addItem(string v)
    {
        GameObject itemTemplate = unityComponents.contentListItems[0];
        if (itemTemplate)
        {
            GameObject newItem = GameObject.Instantiate(itemTemplate);
            newItem.transform.parent = itemTemplate.transform.parent;
            newItem.GetComponent<Text>().text = v;
            newItem.GetComponent<Button>().interactable = true;
            int buttonNum = unityComponents.contentListItems.Count;
            newItem.GetComponent<Button>().onClick.AddListener(delegate { ItemClicked(buttonNum); });
            unityComponents.contentListItems.Add(newItem);
        }
        else
        {
            Debug.LogError("Item template not found for list scroll");
        }
    }

    public void ItemClicked(int buttonNum)
    {
        selectedIndex = buttonNum;
        selectedItem = unityComponents.contentListItems[buttonNum].GetComponent<Text>().text;
    }

    internal void delItem(int index)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_delItem, index);
    }
    internal void M_delItem(int index)
    {
        if (index == 0 && unityComponents.contentListItems.Count == 1) //don't destroy first item if it's the last item - it's used as a template for later items
        {
            unityComponents.contentListItems[index].GetComponent<Text>().text = "";
            unityComponents.contentListItems[index].GetComponent<Button>().interactable = false;
        }
        else
        {
            GameObject.Destroy(unityComponents.contentListItems[index]);
            unityComponents.contentListItems.RemoveAt(index);
        }
    }

    int selectedIndex = -1;
    internal int getSelectedIndex()
    {
        return selectedIndex;
    }

    //added to supercede javachoice
    string selectedItem;
    internal string getSelectedItem()
    {
        //selectedItem = unityComponents.contentListItems[selectedIndex].GetComponent<Text>().text; //set on click instead, to avoid deferring to main thread
        return selectedItem;
    }
}