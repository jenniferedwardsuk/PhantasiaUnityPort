//import java.awt.JavaPanel;
//import java.awt.JavaButton;
//import java.awt.TextField;
//import java.awt.TextArea;
//import java.awt.List;
//import java.awt.BorderLayout;
//import java.io.DataInputStream;
//import java.awt.Color;
//import java.awt.event.*;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class chatPne : JavaPanel , IJavaActionListener, IJavaKeyListener 
{

    pClient parent = null;
    TextArea textArea = null;
    public JavaButton sendJavaButton = null;
    public JavaButton clearJavaButton = null;
    TextField theTextField = null;
    bool active = false;
    JavaPanel controlPane;
    JavaPanel buttonPane;

    public chatPne(pClient c) :base("ChatPne", true)
    {
        controlPane = new JavaPanel("Control", false);
        buttonPane = new JavaPanel("Button", false);

        parent = c;

        setBackground(backgroundColor);

        textArea = new TextArea("", 3, 60, TextArea.SCROLLBARS_VERTICAL_ONLY);
        textArea.setFont(ChatFont);
        textArea.setEditable(false);
        textArea.addIJavaKeyListener(parent);

        sendJavaButton = new JavaButton("Send");
        sendJavaButton.setFont(JavaButtonFont);
        sendJavaButton.addActionListener(this);
        sendJavaButton.setEnabled(false);

        clearJavaButton = new JavaButton("Clear");
        clearJavaButton.setFont(JavaButtonFont);
        clearJavaButton.setActionCommand("Clear");
        clearJavaButton.addActionListener(this);

        theTextField = new TextField();
        theTextField.setFont(ChatFont);
        theTextField.addActionListener(this);
        theTextField.addIJavaKeyListener(this);

        //buttonPane.setLayout(new BorderLayout());
        buttonPane.add("West", clearJavaButton);
        buttonPane.add("East", sendJavaButton);
        buttonPane.setLayout(new BorderLayout());

        //controlPane.setLayout(new BorderLayout());
        controlPane.add("West", buttonPane);
        controlPane.add("Center", theTextField);
        controlPane.setLayout(new BorderLayout());

        //this.setLayout(new BorderLayout());
        this.add("South", controlPane);
        this.add("Center", textArea);
        this.setLayout(new BorderLayout());
    }

    internal void PrintLine()
    {
        textArea.append(parent.readString() + "\n");

        /*
            textArea.addItem(parent.readString());
            if (textArea.countItems() > 50)
                textArea.delItem(1);
        */
    }

    public void actionPerformed(ActionEvent evt) 
    {
        if (active)
        {
            if (evt.getActionCommand() == "Clear")
            {

                textArea.selectAll();
                textArea.replaceRange("", textArea.getSelectionStart(), textArea.getSelectionEnd());
            }
            else
            {
                String theString = theTextField.getText();
                parent.sendString(constants.C_CHAT_PACKET + theString + "\0");
                theTextField.setText("");
            }
        }

        return;
    }

    internal void activateChat()
    {
        active = true;
        sendJavaButton.setEnabled(true);
    }

    internal void deactivateChat()
    {
        active = false;
        sendJavaButton.setEnabled(false);
    }

    internal void takeFocus()
    {
        textArea.requestFocus();
    }

	/* handle function keys pressed in the chat window */
    public void keyPressed(KeyEvent evt)
    {
        int theKey;

        theKey = evt.getKeyCode();

        if (theKey == KeyEvent.VK_F1 || theKey == KeyEvent.VK_F2 ||
            theKey == KeyEvent.VK_F3 || theKey == KeyEvent.VK_F4 ||
            theKey == KeyEvent.VK_F5 || theKey == KeyEvent.VK_F6 ||
            theKey == KeyEvent.VK_F7 || theKey == KeyEvent.VK_F8)
        {

            parent.keyPressed(evt);
        }

        return;
    }

    public void keyReleased(KeyEvent evt) 
    {;}
    public void keyTyped(KeyEvent evt) 
    {;}
}
