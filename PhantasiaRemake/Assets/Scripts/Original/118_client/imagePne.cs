//import java.awt.*;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class imagePne : JavaCanvas
{

    pClient parent = null;
    int imageNumber = -1;

    public imagePne(pClient c, string NAME) : base("ImagePne" + NAME, false)
    {
        super();
        parent = c;
        setBackground(highlightColor);
    }

    internal void repaint()
    {
        if (componentGraphics == null)
            componentGraphics = new JavaGraphics(this);
        paint(componentGraphics); //todo?
    }

    internal void paint(JavaGraphics g)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_paint, g);
    }
    internal void M_paint(JavaGraphics g)
    {
        if (imageNumber != -1 && parent.theImages[imageNumber] != null)
        {
            g.drawImage(parent.theImages[imageNumber], 0, 0, this);
        }
    }

    internal void setImage(int newImage)
    {
        imageNumber = newImage;
        repaint();
    }
}

