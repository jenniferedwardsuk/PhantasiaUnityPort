//import java.awt.JavaPanel;
//import java.awt.JavaLabel;
//import java.awt.Canvas;
//import java.awt.Color;
//import java.awt.BorderLayout;
//import java.awt.FlowLayout;
//import java.awt.JavaGridLayout;
//import java.awt.GridBagLayout;
//import java.awt.GridBagConstraints;
//import java.awt.MediaTracker;
//import java.io.DataInputStream;
//import java.awt.Font;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JavaPanel : JavaComponent
{
    public JavaColor backgroundColor { get; internal set; }
    public JavaFont JavaButtonFont { get; internal set; }
    public int BUTTONS { get; internal set; }
    public JavaFont ChatFont { get; internal set; }
    public JavaFont CompassFont { get; internal set; }
    public JavaFont MsgFont { get; internal set; }
    public JavaColor highlightColor { get; internal set; }
    public JavaFont UserFont { get; internal set; }

    //UnityPanelComponents UnityPanel;

    public JavaPanel(string name, bool isParent)
    {
        INIT(name, isParent, false, false);
    }

    public JavaPanel(string name, bool isParent, bool isMiddleParent)
    {
        INIT(name, isParent, isMiddleParent, false);
    }

    public JavaPanel(string name, bool isParent, bool isMiddleParent, bool parentToPreviousLevel)
    {
        INIT(name, isParent, isMiddleParent, parentToPreviousLevel);
    }


    void INIT(string name, bool isParent, bool isMiddleParent, bool parentToPreviousLevel)
    {
        UnityJavaInterface.AddPanel("-Panel" + name, isParent, isMiddleParent, parentToPreviousLevel, this);
        //if (!unityComponentGroup.rectComponent) //deferred to main thread
        //{
        //    Debug.LogError("Unity object not created");
        //}

        //Debug.Log("defaulting fonts");
        JavaButtonFont = new JavaFont();
        ChatFont = new JavaFont();
        CompassFont = new JavaFont();
        MsgFont = new JavaFont();
        UserFont = new JavaFont();
}


    internal void setSize(int v1, int v2)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_setSize, v1, v2);
    }
    internal void M_setSize(int v1, int v2)
    {
        if (unityComponentGroup.rectComponent)
        {
            ((UnityPanelComponents)unityComponentGroup).rectComponent.sizeDelta = new Vector2(v1, v2);
        }
        else
        {
            Debug.LogError("Unity Component doesn't exist");
        }
    }

    internal void setBackground(JavaColor color)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_setBackground, color);
    }
    internal void M_setBackground(JavaColor color)
    {
        if (((UnityPanelComponents)unityComponentGroup).imageComponent)
        {
            if (color == null)
                color = JavaCanvas.backgroundColor;
            ((UnityPanelComponents)unityComponentGroup).imageComponent.color = color.GetUnityColor();
        }
        else
        {
            Debug.LogError("Unity Component doesn't exist");
        }
    }

    internal void repaint() //Repaints this component. If this component is a lightweight component, this method causes a call to this component's paint method as soon as possible. Otherwise, this method causes a call to this component's update method as soon as possible.
    {
        paint(componentGraphics);
        //This method is called when the contents of the component should be painted; such as when the component is first being shown or is damaged and in need of repair. 
        //The clip rectangle in the Graphics parameter is set to the area which needs to be painted. Subclasses of Component that override this method need not call super.paint(g).
    }
    internal void paint(JavaGraphics g)
    {
        //unnecessary in unity?
    }

    internal void remove(JavaComponent component) //Removes the specified component from this container. This method also notifies the layout manager to remove the component from this container's layout via the removeLayoutComponent method.
    {
        childComponents.Remove(component);
        GameObject.Destroy(component.unityComponentGroup.rectComponent.gameObject, 0);
        UnityJavaInterface.RefreshAllLayouts(this);
    }

    internal void add(JavaComponent component) //Appends the specified component to the end of this container.
    {
        childComponents.Add(component);
        component.unityComponentGroup.rectComponent.parent = unityComponentGroup.rectComponent;
    }
    internal void add(JavaComponent component, int index) //Adds the specified component to this container at the given position. 
    {
        childComponents.Add(component);
        component.unityComponentGroup.rectComponent.parent = unityComponentGroup.rectComponent;
        component.unityComponentGroup.rectComponent.SetSiblingIndex(index);
    }
    internal void add(string location, JavaComponent component) //Adds the specified component to this container.
    {
        childComponents.Add(component);
        component.layoutLocation = location;
        if (component.unityComponentGroup != null)
        {
            if (component.unityComponentGroup.rectComponent && unityComponentGroup.rectComponent)
                component.unityComponentGroup.rectComponent.parent = unityComponentGroup.rectComponent;
            else
                Debug.LogError("Error: rectcomponent doesn't exist for adding " + component);
        }
        else
            Debug.LogError("Error: unityComponentGroup doesn't exist for adding " + component);
    }
    
}