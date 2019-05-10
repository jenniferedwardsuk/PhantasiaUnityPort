//import java.awt.JavaGridLayout;
//import java.awt.BorderLayout;
//import java.io.*;
//import java.awt.JavaPanel;
//import java.awt.JavaButton;
//import java.awt.Font;
//import java.awt.event.*;
//import java.lang.Integer;
using System;
using UnityEngine;
using UnityEngine.UI;

public class JavaButton : JavaComponent
{
    private string buttonLabel;
    public ActionEvent evt;
    bool enabled;

    UnityKeyListener UKeyListener;

    public JavaButton(string buttonLabel)
    {
        INIT(buttonLabel);
    }

    public JavaButton()
    {
        INIT("");
    }

    void INIT(string buttonLabel)
    {
        this.buttonLabel = buttonLabel;
        evt = new ActionEvent();

        unityComponentGroup = UnityJavaInterface.AddButton();
        if (((UnityButtonComponents)unityComponentGroup).textComponent)
        {
            ((UnityButtonComponents)unityComponentGroup).textComponent.text = buttonLabel;
        }
        else
        {
            Debug.LogError("Unity Component not created");
        }
    }

    internal void setFont(JavaFont buttonFont)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_setFont, buttonFont);
    }
    internal void M_setFont(JavaFont buttonFont)
    {
        if (((UnityButtonComponents)unityComponentGroup).textComponent && buttonFont != null)
        {
            ((UnityButtonComponents)unityComponentGroup).textComponent.font = buttonFont.UnityFont;
            ((UnityButtonComponents)unityComponentGroup).textComponent.fontSize = buttonFont.fontSize;
            ((UnityButtonComponents)unityComponentGroup).textComponent.fontStyle = buttonFont.fontStyle;
        }
        else
        {
            Debug.LogError("Unity Component or buttonFont doesn't exist");
        }
    }

    internal void setActionCommand(string p)
    {
        evt.setActionCommand(p);
    }
    //internal string getActionCommand()
    //{
    //    return evt.getActionCommand();
    //}

    internal ActionEvent getActionEvent()
    {
        Debug.Log("fetching button event: " + evt.getActionCommand());
        return evt;
    }

    internal void addIJavaKeyListener(pClient parent)
    {
        //currently unnecessary, unityjavainterface listens for everything
        //UKeyListener = UnityJavaInterface.AddKeyListener();
        //UKeyListener.SetClient(parent);
    }

    internal void setEnabled(bool v)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_setEnabled, v);
    }
    internal void M_setEnabled(bool v)
    {
        enabled = v;
        if (((UnityButtonComponents)unityComponentGroup).buttonComponent)
        {
            ((UnityButtonComponents)unityComponentGroup).buttonComponent.enabled = v;
        }
        else
        {
            Debug.LogError("Unity Component doesn't exist");
        }
    }
 
    internal void setSize(int v1, int v2)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_setSize, v1, v2);
    }
    internal void M_setSize(int v1, int v2)
    {
        if (((UnityButtonComponents)unityComponentGroup).rectComponent)
        {
            ((UnityButtonComponents)unityComponentGroup).rectComponent.sizeDelta = new Vector2(v1,v2);
        }
        else
        {
            Debug.LogError("Unity Component doesn't exist");
        }
    }

    internal void addActionListener(IJavaActionListener listener)
    {
        if (((UnityButtonComponents)unityComponentGroup).buttonComponent)
        {
            ((UnityButtonComponents)unityComponentGroup).buttonComponent.onClick.AddListener(delegate { listener.actionPerformed(evt); });
        }
        else
        {
            Debug.LogError("Unity Component doesn't exist");
        }
    }

    internal void setJavaLabel(string label)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_setJavaLabel, label);
    }
    internal void M_setJavaLabel(string label)
    {
        buttonLabel = label;
        if (((UnityButtonComponents)unityComponentGroup).textComponent)
        {
            ((UnityButtonComponents)unityComponentGroup).textComponent.text = label;
        }
        else
        {
            Debug.LogError("Unity Component doesn't exist");
        }
    }

    internal void requestFocus()
    {
        //unnecessary?
    }

    internal bool isEnabled()
    {
        return enabled;
    }
}