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
            List<string> readstrings = new List<string> { "", "", "", "", "", "", "", "", "", "", "", "", "" };
            init(c, readstrings);
        }
        else
        {
            init_deferred(c);
        }
    }

    internal void init_deferred(pClient c)
    {
        parent = c;
        List<string> readstrings = new List<string> { "", "", "", "", "", "", "", "", "", "", "", "", "" };
        readstrings[0] = parent.readString();
        readstrings[1] = parent.readString();
        readstrings[2] = parent.readString();
        readstrings[3] = parent.readString();
        readstrings[4] = parent.readString();
        readstrings[5] = parent.readString();
        readstrings[6] = parent.readString();
        readstrings[7] = parent.readString();
        readstrings[8] = parent.readString();
        readstrings[9] = parent.readString();
        readstrings[10] = parent.readString();
        readstrings[11] = parent.readString();
        readstrings[12] = parent.readString();
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(init, c, readstrings);
    }
    public void init(pClient c, List<string> readstrings)
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

        title = readstrings[0];          /* player title */
        setTitle(title);
        textArea.setText("Character Information -\n");

        textArea.append("Modified Name: " + title + "\n");
        textArea.append("Name: " + readstrings[1] + "\n");
        textArea.append("Faithful: " + readstrings[2] + "\n");
        textArea.append("Parent Account: " + readstrings[3] + "\n");
        textArea.append("Parent Network: " + readstrings[4] + "\n\n");
        textArea.append("Mute Count: " + readstrings[5] + "\n\n");

        textArea.append("Account Information -\n");
        textArea.append("Account: " + readstrings[6] + "\n");
        textArea.append("E-mail: " + readstrings[7] + "\n");
        textArea.append("Parent Network: " + readstrings[8] + "\n\n");

        textArea.append("Connection Information -\n");
        textArea.append("IP Address: " + readstrings[9] + "\n");
        textArea.append("Network: " + readstrings[10] + "\n");
        textArea.append("Machine ID: " + readstrings[11] + "\n");
        textArea.append("Connected On: " + readstrings[12] + "\n");

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
