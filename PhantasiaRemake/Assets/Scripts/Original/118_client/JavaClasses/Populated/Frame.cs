//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.applet.*;
//import java.awt.event.*;
//import java.lang.Thread;
//import java.awt.event.KeyEvent;
using System;

public class Frame : JavaComponent
{
    private string windowTitle;

    //GridBagLayout layout;

    public Frame(string windowTitle)
    {
        this.windowTitle = windowTitle;
        UnityJavaInterface.setMainTitle();
        AddPanelToFrame("-Frame", true);
        //unityComponentGroup = UnityJavaInterface.AddPanel("-Frame", true);
    }

    internal void addIJavaKeyListener(pClient pClient)
    {
        //unnecessary for unity
    }

    internal void AddPanelToFrame(string v1, bool v2)
    {
        if (UnityGameController.inSetup) //i.e. this is being run by the main thread
            UnityJavaInterface.AddPanel(v1, v2, this);
        else
            UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_AddPanelToFrame, v1, v2);
    }
    internal void M_AddPanelToFrame(string v1, bool v2)
    {
        UnityJavaInterface.AddPanel(v1, v2, this);
    }

    internal void setSize(int v1, int v2)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_setSize, v1, v2);
    }
    internal void M_setSize(int v1, int v2)
    {
        UnityJavaInterface.setMainCanvasSize(v1,v2);
    }

    internal void addWindowListener(WindowAdapter windowAdapter)
    {
        //unnecessary for unity
    }

    internal void setBackground(JavaColor backgroundColor)
    {
        UnityJavaInterface.setMainCanvasBackground(backgroundColor);
    }

    internal void setVisible(bool v)
    {
        //UnityJavaInterface.HideMainCanvas();
        if (!v)
        {
            if (UnityJavaUIFuncQueue.GetInstance())
                UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(UnityJavaInterface.HideMainCanvas);
        }
        else
        {
            if (UnityJavaUIFuncQueue.GetInstance())
                UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(UnityJavaInterface.ShowMainCanvas);

        }
    }

    internal void add(JavaComponent component)
    {
        childComponents.Add(component);
        component.unityComponentGroup.rectComponent.parent = unityComponentGroup.rectComponent;
    }

    internal void dispose()
    {
        //unnecessary for unity
    }
}