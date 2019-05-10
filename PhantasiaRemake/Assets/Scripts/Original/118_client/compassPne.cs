//import java.awt.JavaPanel;
//import java.awt.JavaButton;
//import java.awt.JavaGridLayout;
//import java.io.DataInputStream;
//import java.awt.Font;
//import java.awt.event.*;
//import java.lang.Integer;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class compassPne : JavaPanel , IJavaActionListener 
{
    private pClient parent = null;
    public JavaButton[] theJavaButtons = new JavaButton[9];
    private bool compassStatus;

    public compassPne(pClient c) : base("Compass", true)
    {
        parent = c;

        string[] buttonTitle = { "NW", "N", "NE", "W", "Rest", "E", "SW", "S", "SE" };

        //this.setLayout(new JavaGridLayout(3, 3));

        for (int i = 0; i < 9; i++)
        {
            theJavaButtons[i] = new JavaButton(buttonTitle[i]);
            theJavaButtons[i].setFont(CompassFont);
            theJavaButtons[i].setSize(18, 18);
            theJavaButtons[i].setActionCommand(i.ToString());
            theJavaButtons[i].addActionListener(this);
            theJavaButtons[i].addIJavaKeyListener(parent);
            this.add(theJavaButtons[i]);
        }
        this.setLayout(new JavaGridLayout(3, 3));

        Deactivate();
    }

    internal void Activate()
    {
        for (int i = 0; i < 9; i++)
        {
            theJavaButtons[i].setEnabled(true);
        }
        compassStatus = true;
    }

    internal void Deactivate()
    {
        for (int i = 0; i < 9; i++)
        {
            theJavaButtons[i].setEnabled(false);
        }
        compassStatus = false;
    }

    internal void DoJavaButton(int theNumber)
    {
        if (compassStatus)
        {

            if (parent.pollSendFlag(BUTTONS))
            {

                parent.chat.takeFocus();
                parent.buttons.Deactivate();
                Deactivate();
                parent.sendString(constants.C_RESPONSE_PACKET + (theNumber + 8).ToString() + "\0");
            }
        }
        return;
    }

    public void actionPerformed(ActionEvent evt) 
    {
        DoJavaButton(int.Parse(evt.getActionCommand()));
        return;
    }

}
