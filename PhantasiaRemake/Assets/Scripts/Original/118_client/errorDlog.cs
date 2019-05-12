//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.awt.event.*;
//import java.awt.*;
using UnityEngine;
using System.Collections.Generic;

public class errorDlog : Dialog , IJavaActionListener
{

    private pClient parent;
    JavaPanel top_panel;
    JavaPanel bottom_panel;
    JavaLabel textJavaLabel1;
    JavaLabel textJavaLabel2;
    JavaLabel textJavaLabel3;
    public JavaButton theJavaButton;

    public errorDlog(pClient c, bool hideByDefault = false)
    {
        if (UnityGameController.inSetup)
        {
            init(c, hideByDefault);
        }
        else
        {
            init_deferred(c, hideByDefault);
        }
    }

    internal void init_deferred(pClient c, bool hideByDefault)
    {
        parent = c;
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(init, c, hideByDefault);
    }
    public void init(pClient c, bool hideByDefault)
    {
        top_panel = new JavaPanel("Top", false);
        bottom_panel = new JavaPanel("Bottom", false);
        textJavaLabel1 = new JavaLabel(null);
        textJavaLabel2 = new JavaLabel(null);
        textJavaLabel3 = new JavaLabel(null);
        theJavaButton = new JavaButton("OK");

        super(c.f, true);

        parent = c;

        theJavaButton.addActionListener(this);

        //top_panel.setLayout(new BorderLayout());
        top_panel.add("North", textJavaLabel1);
        top_panel.add("Center", textJavaLabel2);
        top_panel.add("South", textJavaLabel3);
        top_panel.setLayout(new BorderLayout());

        bottom_panel.add(theJavaButton);

        //setLayout(new BorderLayout());
        add("North", top_panel);
        add("South", bottom_panel);
        setLayout(new BorderLayout());

        pack();

        if (hideByDefault)
            setVisible(false);
    }

    internal void bringUp(string string1, string string2, string string3)
    {
        Debug.LogError("Java client error: " + string1 + ", " + string2 + ", " + string3);

        if (UnityJavaUIFuncQueue.GetInstance())
        {
            textJavaLabel1.setAlignment(JavaLabel.CENTER);
            textJavaLabel1.setText(string1);
            //UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(textJavaLabel1.setAlignment, JavaLabel.CENTER);
            //UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(textJavaLabel1.setText, string1);

            textJavaLabel2.setAlignment(JavaLabel.CENTER);
            textJavaLabel2.setText(string2);
            //UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(textJavaLabel2.setAlignment, JavaLabel.CENTER);
            //UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(textJavaLabel2.setText, string2);

            textJavaLabel3.setAlignment(JavaLabel.CENTER);
            textJavaLabel3.setText(string3);
            //UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(textJavaLabel3.setAlignment, JavaLabel.CENTER);
            //UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(textJavaLabel3.setText, string3);
        }

        pack();
        toFront();
        setVisible(true);
    }

    public void actionPerformed(ActionEvent evt) 
    {
        setVisible(false);
        return;
    }
}
