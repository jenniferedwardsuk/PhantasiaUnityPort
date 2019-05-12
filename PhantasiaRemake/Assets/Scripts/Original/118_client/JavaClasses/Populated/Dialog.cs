//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.awt.event.*;
//import java.lang.Long;
using System;
using UnityEngine;

public class Dialog : JavaComponent
{
    UnityPopupComponents unityComponents;

    public Dialog()
    {
        UnityJavaInterface.AddPopup(this);
        //if (!unityComponentGroup.rectComponent) //deferred to main thread
        //{
        //    Debug.LogError("Unity object not created");
        //}
        //else
        //{
        //    unityComponents = (UnityPopupComponents)unityComponentGroup;
        //}
    }

    new public void SetComponentGroup(UnityComponentGroup componentGroup)
    {
        unityComponentGroup = componentGroup;
        if (!componentGroup.rectComponent)
        {
            Debug.LogError("Unity object not created");
        }
        else
        {
            unityComponents = (UnityPopupComponents)componentGroup;
        }
    }

    internal void super(Frame f, bool v)
    {
        //unnecessary for c#
    }

    //Causes this Window to be sized to fit the preferred size and layouts of its subcomponents. The resulting width and height 
    //of the window are automatically enlarged if either of dimensions is less than the minimum size as specified by the previous 
    //call to the setMinimumSize method.
    internal void pack()
    {
        if (UnityJavaUIFuncQueue.GetInstance())
            UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_pack);
        else
            Debug.Log("Warning: UnityJavaUIFuncQueue not initialised for UI updates");
    }
    internal void M_pack()
    {
        UnityJavaInterface.PackPopupLayout(this);
    }

    internal void setVisible(bool v)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_setVisible, v);
    }
    internal void M_setVisible(bool v)
    {
        unityComponentGroup.rectComponent.gameObject.GetComponent<Canvas>().enabled = v;
    }

    internal void add(string location, JavaComponent component)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_add, location, component);
    }
    internal void M_add(string location, JavaComponent component)
    {
        component.layoutLocation = location;
        childComponents.Add(component);

        if (component != null)
        {
            //Debug.LogError("scoreboard debug: component is not null");
            if (component.unityComponentGroup != null) //todo: problem is here
            {
                //Debug.LogError("scoreboard debug: unityComponentGroup is not null");
                //if (component.unityComponentGroup.rectComponent != null)
                //{
                //    Debug.LogError("scoreboard debug: rectComponent is not null");
                //    if (unityComponents != null)
                //    {
                //        Debug.LogError("scoreboard debug: unityComponents is not null");
                //        if (unityComponents.panelComponent != null)
                //        {
                //            Debug.LogError("scoreboard debug: panelComponent is not null");
                //        }
                //    }
                //}
            }
        }
        else
        {
            Debug.LogError("scoreboard debug: component IS NULL");
        }
        component.unityComponentGroup.rectComponent.parent = unityComponents.panelComponent.transform;
        //pack(); //todo: causes issue with scoreboard 
    }

    internal void dispose()
    {
        GameObject.Destroy(unityComponents.rectComponent.gameObject);
    }

    internal void setTitle(string title)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_setTitle, title);
    }
    internal void M_setTitle(string title)
    {
        unityComponents.titleTextComponent.text = title;
    }

    internal void toFront()
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_toFront);
    }
    internal void M_toFront()
    {
        unityComponents.canvasComponent.sortingOrder = UnityJavaInterface.maxSortOrder + 1;
        UnityJavaInterface.updateMaxSortOrder(unityComponents.canvasComponent.sortingOrder);
    }
}