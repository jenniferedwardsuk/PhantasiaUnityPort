//import java.awt.JavaPanel;
//import java.awt.JavaButton;
//import java.awt.TextField;
//import java.awt.TextArea;
//import java.awt.List;
//import java.awt.BorderLayout;
//import java.io.DataInputStream;
//import java.awt.Color;
//import java.awt.event.*;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TextArea : JavaComponent
{
    private string text;
    private int rows;
    private int columns;
    private int scrollbars;
    public static int SCROLLBARS_VERTICAL_ONLY = 0;
    UnityScrollComponents scrollcomponents;

    //TextArea(String text, int rows, int columns, int scrollbars)    //Constructs a new text area with the specified text, and with the rows, columns, and scroll bar visibility as specified.
    public TextArea(string text, int rows, int columns, int scrollbars)
    {
        this.text = text;
        this.rows = rows;
        this.columns = columns;
        this.scrollbars = scrollbars;

        unityComponentGroup = UnityJavaInterface.AddScrollArea();
        scrollcomponents = (UnityScrollComponents)unityComponentGroup;
    }

    //TextArea(int rows, int columns)
    public TextArea(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;
    }


    internal void setFont(JavaFont chatFont)
    {
        if (chatFont != null)
        { 
            scrollcomponents.contentTextComponent.font = chatFont.UnityFont;
            scrollcomponents.contentTextComponent.fontSize = chatFont.fontSize;
            scrollcomponents.contentTextComponent.fontStyle = chatFont.fontStyle;
        }
        else
        {
            Debug.LogError("chatFont doesn't exist");
        }
    }

    internal void setText(string v)
    {
        text = v;
        scrollcomponents.contentTextComponent.text = v;
    }

    internal void setEditable(bool v)
    {
        //always set to false in phantasia. false by definition in unity.
    }

    internal void addIJavaKeyListener(pClient parent)
    {
        //unnecessary in unity
    }

    internal void append(string v)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_append, v);
    }
    internal void M_append(string v)
    {
        text += v;
        scrollcomponents.contentTextComponent.text += v;
    }

    int selectStart = -1;
    int selectEnd = -1;
    internal void selectAll() //Selects all the text in this text component.
    {
        selectStart = 0;
        selectEnd = text.Length - 1;
    }

    internal void replaceRange(string str, int start, int end) //replaceRange(String str, int start, int end)   Replaces text between the indicated start and end positions with the specified replacement text.
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_replaceRange, str, start, end);
    }
    internal void M_replaceRange(string str, int start, int end) //replaceRange(String str, int start, int end)   Replaces text between the indicated start and end positions with the specified replacement text.
    {
        if (start >= 0 && end >= 0)
        {
            text = text.Substring(0, Math.Max(0, start - 1)) + str + text.Substring(end + 1, text.Length - (end + 1));
            scrollcomponents.contentTextComponent.text = text;
        }
        else
        {
            Debug.Log("TextArea replaceRange called without selection");   //this happens intentionally?
        }
    }

    internal int getSelectionStart()
    {
        return selectStart;
    }

    internal int getSelectionEnd()
    {
        return selectEnd;
    }

    internal void requestFocus() //Requests that this Component get the input focus, and that this Component's top-level ancestor become the focused Window. 
    {
        //unnecessary in unity
    }
}