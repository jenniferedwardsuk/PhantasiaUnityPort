//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.awt.event.*;
//import java.awt.*;
using System;
using UnityEngine;
using UnityEngine.UI;

internal class JavaChoice : JavaComponent //The Choice class presents a pop-up menu of choices.
{
    //rerouted to javalist, but coded here for fallback if needed

    UnityListComponents unityComponents;
    
    public JavaChoice()
    {
        unityComponentGroup = UnityJavaInterface.AddListScroll();
        unityComponents = (UnityListComponents)unityComponentGroup;
    }

    internal void addItem(string s)
    {
        GameObject itemTemplate = unityComponents.contentListItems[0];
        if (itemTemplate)
        {
            GameObject newItem = GameObject.Instantiate(itemTemplate);
            newItem.transform.parent = itemTemplate.transform.parent;
            newItem.GetComponent<Text>().text = s;
            newItem.GetComponent<Button>().interactable = true;
            unityComponents.contentListItems.Add(newItem);
        }
        else
        {
            Debug.LogError("choice: Item template not found for list scroll");
        }
    }

    public string selectedItem; //todo - update from unityjavainterface
    internal string getSelectedItem()
    {
        return selectedItem;
    }
}