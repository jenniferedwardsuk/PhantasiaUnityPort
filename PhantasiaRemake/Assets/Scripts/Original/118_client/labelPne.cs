//import java.awt.*;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class labelPne : JavaCanvas
{

    pClient parent = null;
    int imageNumber = -1;
    Rectangle theBounds = null;
    int canvasWidth = 0;

    public labelPne(pClient c, string NAME) : base("LabelPne" + NAME, false)
    {
        super();
        parent = c;
        setBackground(backgroundColor);
    }
    
    internal void repaint()
    {
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

        g.setColor(highlightColor);
        g.drawLine(0, 4, canvasWidth, 4);
        g.drawLine(0, 2, 0, 6);
        g.drawLine(canvasWidth, 2, canvasWidth, 6);
        
        if (imageNumber != -1 && parent.theImages[imageNumber] != null)
        {
            g.drawImage(parent.theImages[imageNumber], (canvasWidth - 9) / 2, 0, this);
        }
    }

    internal void setImage(int newImage)
    {
        imageNumber = newImage;
        repaint();
    }
}

