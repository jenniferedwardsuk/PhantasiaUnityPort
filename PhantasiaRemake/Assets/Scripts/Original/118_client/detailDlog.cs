//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.awt.event.*;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class detailDlog : Dialog , IJavaActionListener
{

    private pClient parent;
    JavaPanel bottom_panel;
    TextArea textArea;
    JavaButton theJavaButton;
    private string title;          /* player title */

    public detailDlog(pClient c)
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
        bottom_panel = new JavaPanel("Bottom", false);
        theJavaButton = new JavaButton(constants.OK_LABEL);
        textArea = new TextArea(20, 40);

        super(c.f, false);

        parent = c;

        theJavaButton.addActionListener(this);
        bottom_panel.add(theJavaButton);

        //setLayout(new BorderLayout());
        add("South", bottom_panel);
        add("Center", textArea);
        setLayout(new BorderLayout());

        title = parent.readString();          /* player title */
        setTitle(title);
        textArea.setText("Character Information -\n");

        textArea.append("Modified Name: " + title + "\n");
        textArea.append("Name: " + parent.readString() + "\n");
        textArea.append("Faithful: " + parent.readString() + "\n");
        textArea.append("Parent Account: " + parent.readString() + "\n");
        textArea.append("Parent Network: " + parent.readString() + "\n\n");
        textArea.append("Mute Count: " + parent.readString() + "\n\n");

        textArea.append("Account Information -\n");
        textArea.append("Account: " + parent.readString() + "\n");
        textArea.append("E-mail: " + parent.readString() + "\n");
        textArea.append("Parent Network: " + parent.readString() + "\n\n");

        textArea.append("Connection Information -\n");
        textArea.append("IP Address: " + parent.readString() + "\n");
        textArea.append("Network: " + parent.readString() + "\n");
        textArea.append("Machine ID: " + parent.readString() + "\n");
        textArea.append("Connected On: " + parent.readString() + "\n");

        pack();
        setVisible(true);
    }


    public void actionPerformed(ActionEvent evt) 
    {
        setVisible(false);
        dispose();
        return;
    }
}
