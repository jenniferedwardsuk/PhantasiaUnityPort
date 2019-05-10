//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.awt.event.*;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class stdDlog : Dialog , IJavaActionListener
{

    private pClient parent;
    JavaPanel top_panel;
    JavaPanel bottom_panel;
    JavaLabel textJavaLabel;
    JavaButton theJavaButton;

    public stdDlog(pClient c)
    {
        if (UnityGameController.inSetup)
        {
            init(c);
        }
        else
        {
            init_deferred(c);
        }
    }

    internal void init_deferred(pClient c)
    {
        parent = c;
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(init, c);
    }
    public void init(pClient c)
    {
        top_panel = new JavaPanel("Top", false);
        bottom_panel = new JavaPanel("Bottom", false);
        textJavaLabel = new JavaLabel(null);
        theJavaButton = new JavaButton("OK");

        super(c.f, true);

        parent = c;

        theJavaButton.addActionListener(this);

        textJavaLabel.setText(parent.readString());
        top_panel.add(textJavaLabel);
        bottom_panel.add(theJavaButton);

        //setLayout(new BorderLayout());
        add("North", top_panel);
        add("South", bottom_panel);
        setLayout(new BorderLayout());

        pack();
        toFront();
        setVisible(true);
    }

    public void actionPerformed(ActionEvent evt)
    {
        setVisible(false);
        dispose();
        return;
    }
}
