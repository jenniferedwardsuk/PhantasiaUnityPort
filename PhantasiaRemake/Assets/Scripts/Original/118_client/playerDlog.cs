//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.awt.event.*;
//import java.awt.*;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class playerDlog : Dialog , IJavaActionListener 
{

    private pClient parent;
    JavaPanel top_panel;
    JavaPanel middle_panel;
    JavaPanel bottom_panel;
    JavaLabel textJavaLabel;
    //JavaChoice popUpMenu;
    JavaList popUpMenu;
    JavaButton okJavaButton;
    JavaButton cancelJavaButton;

    public playerDlog(pClient c)
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
        middle_panel = new JavaPanel("Middle", false);
        bottom_panel = new JavaPanel("Bottom", false);
        textJavaLabel = new JavaLabel(null);
        //popUpMenu = new JavaChoice();
        popUpMenu = new JavaList();
        okJavaButton = new JavaButton(constants.OK_LABEL);
        cancelJavaButton = new JavaButton(constants.CANCEL_LABEL);

        super(c.f, true);

        parent = c;

        okJavaButton.addActionListener(this);
        cancelJavaButton.addActionListener(this);
        cancelJavaButton.setActionCommand("Cancel");

        textJavaLabel = new JavaLabel(null, parent.readString());

        top_panel.add(textJavaLabel);
        middle_panel.add(popUpMenu);
        bottom_panel.add(okJavaButton);
        bottom_panel.add(cancelJavaButton);

        //setLayout(new BorderLayout());
        add("North", top_panel);
        add("Center", middle_panel);
        add("South", bottom_panel);
        setLayout(new BorderLayout());

        middle_panel.remove(popUpMenu);
        //popUpMenu = new JavaChoice();
        popUpMenu = new JavaList();
        middle_panel.add(popUpMenu);

        for (int i = 0; i < parent.users.theList.Count; i++)
            popUpMenu.addItem((string)parent.users.theList.elementAt(i));

        /* this must go before setVisible, because it doesn't return */
        parent.raiseSendFlag(constants.PLAYER_DLOG);

        pack();
        setVisible(true);
        okJavaButton.requestFocus();
    }

    internal void timeout()
    {
	    setVisible(false);
    }

    public void actionPerformed(ActionEvent evt)
    {
        if (parent.pollSendFlag(constants.PLAYER_DLOG))
        {
            setVisible(false);
            if (evt.getActionCommand() == "Cancel")
            {
                parent.sendString(constants.C_CANCEL_PACKET);
            }
            else
            {
                parent.sendString(constants.C_RESPONSE_PACKET + popUpMenu.getSelectedItem() + "\0");
            }
            dispose();
        }
        return;
    }
}