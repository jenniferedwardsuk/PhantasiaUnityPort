//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.awt.event.*;
//import java.awt.*;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class stringDlog : Dialog , IJavaActionListener 
{

    private pClient parent;
    JavaPanel top_panel;
    JavaPanel middle_panel;
    JavaPanel bottom_panel;
    JavaLabel textJavaLabel;
    TextField textField;
    JavaButton okJavaButton;
    JavaButton cancelJavaButton;

    internal stringDlog(pClient c, bool maskTextChars)
    {
        if (UnityGameController.inSetup)
        {
            init(c, maskTextChars, "DUMMY STRING");
        }
        else
        {
            init_deferred(c, maskTextChars);
        }
    }

    internal void init_deferred(pClient c, bool maskTextChars)
    {
        parent = c;
        string labelString = parent.readString();
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(init, c, maskTextChars, labelString);
    }
    internal void init(pClient c, bool maskTextChars, string labelString)
    { 
        top_panel = new JavaPanel("Top", false);
        middle_panel = new JavaPanel("Middle", false);
        bottom_panel = new JavaPanel("Bottom", false);
        textJavaLabel = new JavaLabel(null);
        textField = new TextField(12);
        okJavaButton = new JavaButton(constants.OK_LABEL);
        cancelJavaButton = new JavaButton(constants.CANCEL_LABEL);

        super(c.f, false);

        parent = c;

        okJavaButton.addActionListener(this);
        cancelJavaButton.addActionListener(this);
        textField.addActionListener(this);

        cancelJavaButton.setActionCommand("Cancel");

        textJavaLabel = new JavaLabel(null, labelString, JavaLabel.CENTER);

        top_panel.add(textJavaLabel);
        middle_panel.add(textField);
        bottom_panel.add(okJavaButton);
        bottom_panel.add(cancelJavaButton);

        //setLayout(new BorderLayout());
        add("North", top_panel);
        add("Center", middle_panel);
        add("South", bottom_panel);
        setLayout(new BorderLayout());

        if (maskTextChars)
        {
            textField.setEchoChar('X');
        }

        pack();
        setVisible(true);
        textField.requestFocus();
        parent.raiseSendFlag(constants.STRING_DLOG);
    }

    internal void timeout()
    {
	    setVisible(false);
    }

    public void actionPerformed(ActionEvent evt)
    {
        if (parent.pollSendFlag(constants.STRING_DLOG))
        {
            setVisible(false);
            if (evt.getActionCommand() == "Cancel")
            {
                parent.sendString(constants.C_CANCEL_PACKET);
            }
            else
            {
                parent.sendString(constants.C_RESPONSE_PACKET + textField.getText() + "\0");
            }
            dispose();
        }
        return;
    }
}