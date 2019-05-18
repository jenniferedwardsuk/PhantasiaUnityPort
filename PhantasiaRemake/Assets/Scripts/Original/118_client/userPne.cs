//import java.awt.JavaPanel;
//import java.awt.List;
//import java.awt.BorderLayout;
//import java.io.DataInputStream;
//import java.util.Vector;
//import java.awt.event.*;
using UnityEngine;
using System.Collections.Generic;
using System;

public class userPne : JavaPanel, IJavaActionListener 
{

    private JavaList theDisplay = new JavaList();
    public JavaVector theList = new JavaVector();

    private pClient parent = null;

    public userPne(pClient c) : base("UserPne", true)
    {
        parent = c;

        theDisplay.setFont(UserFont);
        theDisplay.addActionListener(this);

        //this.setLayout(new BorderLayout());
        this.add("Center", theDisplay);
        this.setLayout(new BorderLayout());
    }


    internal void AddUser()
    {
        String name = parent.readString();
        name = name.Replace('\0', '¬').Replace('$', '¬').Replace("¬", "");

        theDisplay.addItem(parent.readString() + "- " + name);

        theList.addElement(name);
    }

    internal void RemoveUser()
    {

        int index = 0;

        string playerstr = parent.readString();
        index = theList.indexOf(playerstr);
        //Debug.LogError("Player list debug: Java removing player " + playerstr + " at index " + index);

        theDisplay.delItem(index);
        theList.removeElementAt(index);
    }

    public void actionPerformed(ActionEvent evt)
    {
	    parent.sendString(constants.C_EXAMINE_PACKET + theList.elementAt(theDisplay.getSelectedIndex()) + "\0");
	    return;
    }
}
