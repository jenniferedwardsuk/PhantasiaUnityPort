//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.awt.event.*;
//import java.lang.Long;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class coordDlog : Dialog, IJavaActionListener 
{

    private pClient parent;
    JavaPanel top_panel;
    JavaPanel middle_panel;
    JavaPanel bottom_panel;
    JavaPanel x_panel;
    JavaPanel y_panel;
    JavaLabel topJavaLabel;
    JavaLabel xJavaLabel;
    JavaLabel yJavaLabel;
    TextField xField;
    TextField yField;
    JavaButton okJavaButton;
    JavaButton cancelJavaButton;


    public coordDlog(pClient c)
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
        x_panel = new JavaPanel("X", false);
        y_panel = new JavaPanel("Y", false);
        topJavaLabel = new JavaLabel(null, parent.readString(), JavaLabel.CENTER);
        xJavaLabel = new JavaLabel(null, "X Coordinate:");
        yJavaLabel = new JavaLabel(null, "Y Coordinate:");
        xField = new TextField(15);
        yField = new TextField(15);
        okJavaButton = new JavaButton(constants.OK_LABEL);
        cancelJavaButton = new JavaButton(constants.CANCEL_LABEL);

        super(c.f, false);

        parent = c;

        okJavaButton.addActionListener(this);
        cancelJavaButton.addActionListener(this);
        xField.addActionListener(this);
        yField.addActionListener(this);

        cancelJavaButton.setActionCommand("Cancel");

        top_panel.add(topJavaLabel);

        x_panel.add(xJavaLabel);
        x_panel.add(xField);

        y_panel.add(yJavaLabel);
        y_panel.add(yField);

        bottom_panel.add(okJavaButton);
        bottom_panel.add(cancelJavaButton);

        //middle_panel.setLayout(new BorderLayout());
        middle_panel.add("North", x_panel);
        middle_panel.add("South", y_panel);
        middle_panel.setLayout(new BorderLayout());

        //setLayout(new BorderLayout());
        add("North", top_panel);
        add("South", bottom_panel);
        add("Center", middle_panel);
        setLayout(new BorderLayout());

        pack();
        setVisible(true);
        xField.requestFocus();
        parent.raiseSendFlag(constants.COORD_DLOG);
    }

    internal void timeout()
    {
	    setVisible(false);
    }

    public void actionPerformed(ActionEvent evt)
    {
        if (parent.pollSendFlag(constants.COORD_DLOG))
        {
            setVisible(false);
            if (evt.getActionCommand() =="Cancel")
            {
                parent.sendString(constants.C_CANCEL_PACKET);
            }
            else
            {
                parent.sendString(constants.C_RESPONSE_PACKET + xField.getText() + "\0" +
                constants.C_RESPONSE_PACKET + yField.getText() + "\0");
            }
            dispose();
        }
        return;
    }
}