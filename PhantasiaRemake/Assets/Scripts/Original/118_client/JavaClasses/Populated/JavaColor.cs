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
using UnityEngine;
using UnityEngine.UI;

public class JavaColor
{
    Color color;

    public JavaColor(Color setColor)
    {
        color = setColor;
    }

    internal Color GetUnityColor()
    {
        return color;
    }
}