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

public class JavaComponent
{
    public enum LayoutType { Grid, GridBag, Flow, Border };

    public UnityComponentGroup unityComponentGroup;
    internal JavaLayout layout;
    public LayoutType layoutType;
    public string layoutLocation;
    internal JavaGraphics componentGraphics;

    public List<JavaComponent> childComponents;

    internal GridBagConstraints gridBagConstraints;
    internal string layoutName;

    //used for JavaPanel.add

    public JavaComponent()
    {
        childComponents = new List<JavaComponent>();
    }

    public void SetComponentGroup(UnityComponentGroup componentGroup)
    {
        unityComponentGroup = componentGroup;
        if (!componentGroup.rectComponent)
        {
            Debug.LogError("Unity object not created");
        }
    }

    internal void setLayout(JavaLayout gridLayout)
    {
        layout = gridLayout;
        UnityJavaInterface.SetLayout(this, unityComponentGroup, gridLayout);
        
        //added for unity: refresh whole layout after adding and loading everything
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(UnityJavaInterface.RefreshAllLayouts, this);
    }

    internal JavaLayout getLayout()
    {
        return layout;
    }
}