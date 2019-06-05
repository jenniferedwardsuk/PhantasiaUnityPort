//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.awt.event.*;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class scoreDlog : Dialog , IJavaActionListener 
{

    private pClient parent;
    JavaPanel top_panel;
    JavaPanel bottom_panel;
    TextArea textArea;
    JavaButton nextJavaButton;
    JavaButton prevJavaButton;
    JavaButton theJavaButton;

    private long start, records;

    public scoreDlog(pClient c)
    {
        if (UnityGameController.inSetup)
        {
            List<string> textstrings = new List<string>();
            for (int i = 0; i < 50; i++)
            {
                textstrings.Add("");
                textstrings.Add("");
            }
            init(c, 0, 50, textstrings);
        }
        else
        {
            init_deferred(c);
        }
    }

    internal void init_deferred(pClient c)
    {
        parent = c;
        long long1 = parent.readLong();
        long long2 = parent.readLong();
        List<string> textstrings = new List<string>();
        for (int i = 0; i < long2 * 2; i++)
        {
            textstrings.Add(parent.readString());
        }
        //string textstring = parent.readString();
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(init, c, long1, long2, textstrings);
    }
    public void init(pClient c, long long1, long long2, List<string> textstring)
    {
        top_panel = new JavaPanel("Top", false);
        bottom_panel = new JavaPanel("Bottom", false);
        textArea = new TextArea(20, 80);
        nextJavaButton = new JavaButton("Next");
        prevJavaButton = new JavaButton("Prev");
        theJavaButton = new JavaButton(constants.OK_LABEL);

        super(c.f, false);

        parent = c;

        nextJavaButton.addActionListener(this);
        nextJavaButton.setActionCommand("next");
        bottom_panel.add(nextJavaButton);

        prevJavaButton.addActionListener(this);
        prevJavaButton.setActionCommand("prev");
        bottom_panel.add(prevJavaButton);

        theJavaButton.addActionListener(this);
        theJavaButton.setActionCommand("ok");
        bottom_panel.add(theJavaButton);

        //setLayout(new BorderLayout());
        add("North", top_panel); //added for unity
        add("South", bottom_panel);
        add("Center", textArea);
        setLayout(new BorderLayout());

        start = long1;  /* Starting record */
        records = long2;          /* Records to print */
        setTitle("Scoreboard");

        if (start == 0)
        {
            prevJavaButton.setEnabled(false);
        }

        if (records == 1)
        {
            nextJavaButton.setEnabled(false);
        }

        /* print out all the high scores */
        for (int i = 0; i < records * 2; i++)
        {
            textArea.append(textstring[i] + "\n"); 
        }

        pack();
        setVisible(true);
    }

    public void actionPerformed(ActionEvent evt)
    {
        if ((evt.getActionCommand()) == "next")
        {
            parent.sendString(constants.C_SCOREBOARD_PACKET);
            parent.sendString((start + 50) + "\0");
        }
        else if ((evt.getActionCommand()) == "prev")
        {
            parent.sendString(constants.C_SCOREBOARD_PACKET);
            parent.sendString((start - 50) + "\0");
        }

        setVisible(false);
        dispose();
        return;
    }
}
