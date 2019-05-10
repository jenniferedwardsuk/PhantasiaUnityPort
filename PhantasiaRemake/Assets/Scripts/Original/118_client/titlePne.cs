//import java.awt.*;
using UnityEngine;
using System.Collections.Generic;

public class titlePne : JavaCanvas
{

    string theValue;
    int cx = 0;
    int canvasWidth = 0;
    int canvasHeight = 0;
    Rectangle theBounds = null;
    FontMetrics fm = null;
    JavaFont theFont = new JavaFont("Helvetica", JavaFont.BOLD, 12);
    JavaLabel textJavaLabel;

    public titlePne(string message, bool parent) : base("TitlePne", parent)
    {
        super();

        init(message);

        ////added for unity
        //textJavaLabel = new JavaLabel(this, message);

        //theValue = message;
        //setBackground(highlightColor);

        ////added for unity
        //if (componentGraphics == null)
        //    componentGraphics = new JavaGraphics(this);
        //paint(componentGraphics); 
    }

    //public void init(string message)
    //{
    //    UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_init, message);
    //}
    public void init(string message)
    {
        //added for unity
        textJavaLabel = new JavaLabel(this, message);

        theValue = message;
        setBackground(highlightColor);

        //added for unity
        if (componentGraphics == null)
            componentGraphics = new JavaGraphics(this);
        paint(componentGraphics);
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

        fm = getFontMetrics(theFont);
        cx = (int)((canvasWidth - fm.stringWidth(theValue)) / 2);

        g.setColor(highlightColor);
        g.clearRect(0, 0, canvasWidth, canvasHeight);

        g.setColor(foregroundColor);
        g.drawLine(0, canvasHeight - 3, canvasWidth, canvasHeight - 3);
        g.setFont(theFont);
        g.drawString(theValue, cx, canvasHeight - 4, (UnityTextComponents)textJavaLabel.unityComponentGroup);
    }
}
