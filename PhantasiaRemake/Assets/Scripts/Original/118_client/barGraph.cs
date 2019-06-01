//import java.awt.*;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class barGraph : JavaCanvas
{

    float percent = 0;
    float otherPercent = 0;
    long otherMax = 0;
    string theValue;
    int cx = 0;


    int canvasWidth = 0;
    int canvasHeight = 0;
    int theFill = 0;
    Rectangle theBounds = null;
    Color fillColor = Color.black;//null;
    FontMetrics fm = null; // unnecessary
    JavaFont theFont = new JavaFont("Helvetica", JavaFont.BOLD, 12);
    JavaLabel textJavaLabel;

    public barGraph() : base("Bargraph", false)
    {
        super();
        setBackground(highlightColor);
        percent = 0;
        theValue = "0";

        //added for unity
        init(theValue);
    }

    public void init(string message)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_init, message);
    }
    public void M_init(string message)
    {
        textJavaLabel = new JavaLabel(this, theValue);
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

        if (percent == 1)
        {
            fillColor = Color.green;
        }
        else if (percent < .2)
        {
            fillColor = Color.red;
        }
        else if (percent > .7)
        {
            fillColor = new Color((float)Math.Cos(3.14159 * (percent - .7)), (float)1.0, (float)0);
        }
        else
        {
            fillColor = new Color((float)1.0, (float)Math.Sin(3.14159 * (percent - .2)), (float)0);
        }

        fm = getFontMetrics(theFont);
        cx = (int)((canvasWidth - fm.stringWidth(theValue)) / 2);

        g.setColor(highlightColor);
        g.clearRect(0, 0, canvasWidth, canvasHeight);

        if (otherPercent > 0)
        {
            g.setColor(new JavaColor(Color.blue));
            theFill = (int)(otherPercent * canvasWidth);
            g.fillRect(0, 0, theFill, canvasHeight, otherPercent);
        }

        g.setColor(foregroundColor);
        g.drawRect(0, 0, canvasWidth, canvasHeight);
        g.drawRect(1, 1, canvasWidth - 2, canvasHeight - 2);

        g.setColor(new JavaColor(fillColor));
        theFill = (int)(percent * (canvasWidth - 7));
        g.fillRect(4, 4, theFill, canvasHeight - 7, percent);

        g.setColor(foregroundColor);
        g.setFont(theFont);
        g.drawString(theValue, cx, 14, (UnityTextComponents)textJavaLabel.unityComponentGroup);
    }


    internal void changeStats(string value, long maxValue, long secondValue)
    {
        percent = (float)long.Parse(value) / maxValue;
        theValue = value;

        if (secondValue == 0)
        {
            otherPercent = 0;
            otherMax = 0;
        }
        else if (secondValue > otherMax)
        {
            otherMax = secondValue;
            otherPercent = 1;
        }
        else
        {
            otherPercent = (float)secondValue / otherMax;
        }

        paint(getGraphics());
    }
}
