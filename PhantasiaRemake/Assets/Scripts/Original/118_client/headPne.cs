//import java.awt.*;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class headPne : JavaCanvas
{

    string title1;
    string title2;
    int cx = 0;
    int canvasWidth = 0;
    int canvasHeight = 0;
    Rectangle theBounds = null;
    FontMetrics fm = null;
    JavaFont theFont = new JavaFont("TimesRoman", JavaFont.BOLD, 14);
    JavaFont theFont2 = new JavaFont("Helvetica", JavaFont.PLAIN, 12);
    JavaLabel textJavaLabel;
    JavaLabel textJavaLabel2; //unity: added

    public headPne(string message) : base("Head", false)
    {
        super();
        title1 = message;
        title2 = "";
        //setBackground(foregroundColor);//highlightColor); //changed for unity
        //setSize(38, 38);

        ////added for unity
        //textJavaLabel = new JavaLabel(this, message);
        init(message);
    }

    //public void init(string message)
    //{
    //    UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_init, message);
    //}
    public void init(string message)
    {
        setBackground(foregroundColor);//highlightColor); //changed for unity
        setSize(38, 38);

        //added for unity
        textJavaLabel = new JavaLabel(this, message);
        textJavaLabel2 = new JavaLabel(this, ""); //unity: added
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

        fm = getFontMetrics(theFont);
        cx = (int)((canvasWidth - fm.stringWidth(title1)) / 2);
        g.setColor(foregroundColor);

        g.setFont(theFont);
        g.drawString(title1, cx, 16, (UnityTextComponents)textJavaLabel.unityComponentGroup);

        fm = getFontMetrics(theFont2);
        cx = (int)((canvasWidth - fm.stringWidth(title2)) / 2);

        g.setFont(theFont2);
        g.drawString(title2, cx, canvasHeight - 6, (UnityTextComponents)textJavaLabel2.unityComponentGroup); //unity: amended

        //unity: added: manual offset for now
        UnityTextComponents label1 = (UnityTextComponents)textJavaLabel.unityComponentGroup;
        UnityTextComponents label2 = (UnityTextComponents)textJavaLabel2.unityComponentGroup;
        label2.textComponent.transform.position = label1.textComponent.transform.position - new Vector3(0, 15, 0);
    }

    internal void changeHead(string message)
    {
	    title1 = message;
	    paint(getGraphics());
    }

    internal void changeTail(string message)
    {
	    title2 = message;
	    paint(getGraphics());
    }
}
