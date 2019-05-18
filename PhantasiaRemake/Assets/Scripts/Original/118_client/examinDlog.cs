//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.awt.event.*;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class examinDlog : Dialog , IJavaActionListener 
    {

    private pClient parent;
    JavaPanel bottom_panel;
    TextArea textArea;
    JavaButton theJavaButton;

    private string title = null;          /* player title */

    public examinDlog(pClient c)
    {
        if (UnityGameController.inSetup)
        {
            List<string> readstrings = new List<string>() { };
            for (int i = 0; i < 41; i++)
            {
                readstrings.Add("");
            }
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
        List<string> readstrings = new List<string>() { };
        for (int i = 0; i < 41; i++)
        {
            readstrings.Add(parent.readString());
        }
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(init, c, readstrings);
    }
    public void init(pClient c, List<string> readstrings)
    {
        int i = 0;

        bottom_panel = new JavaPanel("Bottom", false);
        textArea = new TextArea(20, 40);
        theJavaButton = new JavaButton(constants.OK_LABEL);

        super(c.f, false);

        parent = c;

        theJavaButton.addActionListener(this);
        bottom_panel.add(theJavaButton);

        //setLayout(new BorderLayout());
        add("South", bottom_panel);
        add("Center", textArea);
        setLayout(new BorderLayout());

        title = readstrings[i++];          /* player title */
        setTitle(title);
        textArea.setText(title + "\n");

        textArea.append("Location: " + readstrings[i++] + "\n\n");

        textArea.append("Account: " + readstrings[i++] + "\n");
        textArea.append("Network: " + readstrings[i++] + "\n");
        textArea.append("Channel: " + readstrings[i++] + "\n\n");

        textArea.append("Level: " + readstrings[i++] + "\n");
        string exp = readstrings[i++]; //added for unity: constrain to 2 d.p.
        if (exp.IndexOf(".") > 0)
        {
            exp = exp.Substring(0, exp.IndexOf(".") + 2);
        }
        textArea.append("Experience: " + exp + "\n");
        textArea.append("Next Level At: " + readstrings[i++] + "\n\n");

        textArea.append("Energy: " + readstrings[i++] + " / (" + readstrings[i++] + " + " + readstrings[i++] + " shield)\n");
        textArea.append("Strength: " + readstrings[i++] + " / (" + readstrings[i++] + " + " + readstrings[i++] + " sword)\n");
        textArea.append("Quickness: " + readstrings[i++] + " / (" + readstrings[i++] + " + " + readstrings[i++] + " quicksilver)\n");
        textArea.append("Brains: " + readstrings[i++] + "\n");
        textArea.append("Magic Level: " + readstrings[i++] + "\n");
        textArea.append("Mana: " + readstrings[i++] + "\n");
        textArea.append("Gender: " + readstrings[i++] + "\n");
        textArea.append("Poison: " + readstrings[i++] + "\n");
        string Sin = readstrings[i++]; //added for unity: constrain to 2 d.p.
        if (Sin.IndexOf(".") > 0)
        {
            Sin = Sin.Substring(0, Sin.IndexOf(".") + 2);
        }
        textArea.append("Sin: " + Sin + "\n");
        textArea.append("Lives: " + readstrings[i++] + "\n\n");

        textArea.append("Gold: " + readstrings[i++] + "\n");
        textArea.append("Gems: " + readstrings[i++] + "\n");
        textArea.append("Holy Water: " + readstrings[i++] + "\n");
        textArea.append("Amulets: " + readstrings[i++] + "\n");
        textArea.append("Charms: " + readstrings[i++] + "\n");
        textArea.append("Staves/Crowns: " + readstrings[i++] + "\n");
        textArea.append("Virgin: " + readstrings[i++] + "\n");
        textArea.append("Blessing: " + readstrings[i++] + "\n");
        textArea.append("Palantir: " + readstrings[i++] + "\n");
        textArea.append("Ring: " + readstrings[i++] + "\n\n");

        textArea.append("Cloaked: " + readstrings[i++] + "\n");
        textArea.append("Blind: " + readstrings[i++] + "\n");
        textArea.append("Age: " + readstrings[i++] + "\n");
        textArea.append("Degenerated: " + readstrings[i++] + "\n");
        textArea.append("Played For: " + readstrings[i++] + "\n");
        textArea.append("Loaded On: " + readstrings[i++] + "\n");
        textArea.append("Created On: " + readstrings[i++] + "\n");

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
