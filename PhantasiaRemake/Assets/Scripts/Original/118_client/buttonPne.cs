//import java.awt.JavaGridLayout;
//import java.awt.BorderLayout;
//import java.io.*;
//import java.awt.JavaPanel;
//import java.awt.JavaButton;
//import java.awt.Font;
//import java.awt.event.*;
//import java.lang.Integer;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class buttonPne : JavaPanel, IJavaActionListener
    {

    private pClient parent = null;
    private labelPne[] buttonJavaLabel = new labelPne[8];
    private JavaPanel[] buttonJavaPanel = new JavaPanel[8];
    public JavaButton[] theJavaButtons = new JavaButton[8];
    private bool[] buttonStatus = new bool[8];

    public buttonPne(pClient c) : base("ButtonPne", true)
    {
        parent = c;

        init();
        //base.setLayout(new JavaGridLayout(1, 8, 1, 0));
    }
    //public void init()
    //{
    //    UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_init);
    //}
    public void init()
    {
        for (int i = 0; i < 8; i++)
        {

            buttonJavaLabel[i] = new labelPne(parent, (i+1).ToString());
            buttonJavaLabel[i].setSize(9, 9);

            theJavaButtons[i] = new JavaButton();
            theJavaButtons[i].setFont(JavaButtonFont);
            theJavaButtons[i].setActionCommand(i.ToString());
            theJavaButtons[i].addActionListener(this);
            theJavaButtons[i].addIJavaKeyListener(parent);

            buttonJavaPanel[i] = new JavaPanel("Button", false);
            buttonJavaPanel[i].setBackground(backgroundColor);
            //buttonJavaPanel[i].setLayout(new BorderLayout());
            buttonJavaPanel[i].add("North", theJavaButtons[i]);
            buttonJavaPanel[i].add("South", buttonJavaLabel[i]);
            buttonJavaPanel[i].setLayout(new BorderLayout());

            this.add(buttonJavaPanel[i]);
        }
        base.setLayout(new JavaGridLayout(1, 8, 1, 0));

        Deactivate();
    }

    internal void Deactivate()
    {
        for (int i = 0; i < 8; i++)
        {
            theJavaButtons[i].setEnabled(false);
            buttonStatus[i] = false;
        }
    }

    internal void Question()
    {
	    string buttonJavaLabel = null;

        for (int i = 0; i < 8; i++)
        {

            buttonJavaLabel = parent.readString();

            theJavaButtons[i].setJavaLabel(buttonJavaLabel);

            if (buttonJavaLabel.Length > 0)
            {
                theJavaButtons[i].setEnabled(true);
                buttonStatus[i] = true;
            }
        }
        parent.raiseSendFlag(BUTTONS);
    }

    internal void turn()
    {
        parent.compass.Activate();
        Question();
    }

    internal void DoJavaButton(int theNumber)
    {
        if (buttonStatus[theNumber])
        {

            if (parent.pollSendFlag(BUTTONS))
            {

                parent.chat.takeFocus();
                parent.compass.Deactivate();
                Deactivate();
                parent.sendString(constants.C_RESPONSE_PACKET + theNumber.ToString() + "\0");
            }
        }
        return;
    }

    public void actionPerformed(ActionEvent evt)
    {
        DoJavaButton(int.Parse(evt.getActionCommand()));
        return;
    }

    internal void timeout()
    {
        parent.compass.Deactivate();
        Deactivate();
    }

    internal void setImages()
    {

        for (int i = 0; i < 8; i++)
        {

            buttonJavaLabel[i].setImage(19 + i);
        }

    }

    internal void spacebar()
    {

        for (int i = 0; i < 8; i++)
        {

            if (theJavaButtons[i].isEnabled())
            {
                DoJavaButton(i);
                return;
            }
        }

        return;
    }

}
