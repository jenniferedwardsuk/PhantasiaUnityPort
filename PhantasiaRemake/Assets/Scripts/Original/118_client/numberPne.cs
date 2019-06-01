//import java.awt.*;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class numberPne : JavaCanvas
{

    string theValue;
    int canvasWidth = 0;
    int canvasHeight = 0;
    Rectangle theBounds = null;
    JavaFont theFont = new JavaFont("Helvetica", JavaFont.BOLD, 12);
    JavaLabel textJavaLabel;

    public numberPne(string message, string NAME) : base("NumberPne" + NAME, false)
    {
        super();
        theValue = message;
        setBackground(foregroundColor);//highlightColor); //changed for unity
        setSize(100, 18);

        //added for unity
        init(message);
    }

    public void init(string message)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_init, message);
    }
    public void M_init(string message)
    {
        textJavaLabel = new JavaLabel(this, message);
    }

    internal void paint(JavaGraphics g)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_paint, g);
    }
    internal void M_paint(JavaGraphics g)
    {
        theBounds = getBounds();
        canvasWidth = theBounds.width - 1;
        canvasHeight = theBounds.height - 1;

        g.setColor(highlightColor);
        g.clearRect(0, 0, canvasWidth, canvasHeight);

        g.setColor(foregroundColor);
        g.setFont(theFont);
        g.drawString(theValue, 0, canvasHeight - 3, (UnityTextComponents)textJavaLabel.unityComponentGroup);
    }

    internal void changeStats(string message)
    {
        theValue = message;
        paint(getGraphics());
    }
}
