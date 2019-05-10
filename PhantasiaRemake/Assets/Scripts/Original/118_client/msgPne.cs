//import java.awt.JavaPanel;
//import java.awt.TextArea;
//import java.awt.List;
//import java.io.DataInputStream;
//import java.awt.BorderLayout;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class msgPne : JavaPanel
{

    pClient parent = null;
    public TextArea textArea = null;

    public msgPne(pClient c) : base("MsgPne", true)
    {
        parent = c;

        textArea = new TextArea("", 5, 60, TextArea.SCROLLBARS_VERTICAL_ONLY);
        textArea.setFont(MsgFont);
        textArea.setEditable(false);
        textArea.addIJavaKeyListener(parent);

        add("Center", textArea);
        //setLayout(new BorderLayout());
    }

    internal void PrintLine()
    {
	    textArea.append(parent.readString() + "\n");
    }

    internal void ClearScreen()
    {
	    textArea.selectAll();
	    textArea.replaceRange("", textArea.getSelectionStart(), textArea.getSelectionEnd());
    }
}
