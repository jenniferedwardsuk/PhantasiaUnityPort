//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.awt.event.*;
//import java.lang.Long;
using System;
using UnityEngine;

internal class JavaLabel : JavaComponent
{
    
    private string v;
    private int alignment;
    public static int CENTER = 0;
    public static int LEFT = 1;
    public static int RIGHT = 2;
    /*
static int	CENTER
Indicates that the label should be centered.
static int	LEFT
Indicates that the label should be left justified.
static int	RIGHT
Indicates that the label should be right justified.
     */

    public JavaLabel(JavaCanvas specificParent)
    {
        INIT(specificParent, "", 0);
    }

    public JavaLabel(JavaCanvas specificParent, string v)
    {
        INIT(specificParent, v, 0);
    }

    public JavaLabel(JavaCanvas specificParent, string v, int alignment)
    {
        INIT(specificParent, v, alignment);
    }

    public void INIT(JavaCanvas specificParent, string v, int alignment)
    {
        this.v = v;
        this.alignment = CENTER;
        unityComponentGroup = UnityJavaInterface.AddLabel(specificParent);
        if (((UnityTextComponents)unityComponentGroup).textComponent)
        {
            ((UnityTextComponents)unityComponentGroup).textComponent.text = v;
            ((UnityTextComponents)unityComponentGroup).textComponent.alignment = UnityEngine.TextAnchor.MiddleCenter;
            if (alignment == LEFT)
                ((UnityTextComponents)unityComponentGroup).textComponent.alignment = UnityEngine.TextAnchor.MiddleLeft;
            else if (alignment == RIGHT)
                ((UnityTextComponents)unityComponentGroup).textComponent.alignment = UnityEngine.TextAnchor.MiddleRight;
        }
        else
        {
            Debug.LogError("Unity component not created");
        }
    }

    internal void setAlignment(int alignment)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_setAlignment, alignment);
    }
    internal void M_setAlignment(int alignment)
    {
        this.alignment = alignment;
        if (((UnityTextComponents)unityComponentGroup).textComponent)
        {
            ((UnityTextComponents)unityComponentGroup).textComponent.alignment = UnityEngine.TextAnchor.MiddleCenter;
            if (alignment == LEFT)
                ((UnityTextComponents)unityComponentGroup).textComponent.alignment = UnityEngine.TextAnchor.MiddleLeft;
            else if (alignment == RIGHT)
                ((UnityTextComponents)unityComponentGroup).textComponent.alignment = UnityEngine.TextAnchor.MiddleRight;
        }
        else
        {
            Debug.LogError("Unity component doesn't exist");
        }
    }

    internal void setText(string string1)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_setText, string1);
    }
    internal void M_setText(string string1)
    {
        v = string1;
        if (((UnityTextComponents)unityComponentGroup).textComponent)
        {
            ((UnityTextComponents)unityComponentGroup).textComponent.text = string1;
        }
        else
        {
            Debug.LogError("Unity component doesn't exist");
        }
    }
}