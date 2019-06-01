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
        string msgstr = parent.readString();
        //Debug.LogError("Msg debug: " + msgstr.Replace('\0', '¬').Replace('$', '¬').Replace("¬", ""));
        msgstr = msgstr.Replace('\0', '¬').Replace('$', '¬').Replace("¬", "");
        textArea.append(msgstr + "\n");
        
        SetScroll(new Vector2(0, 0));
    }

    internal void SetScroll(Vector2 position) //added for unity
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_SetScroll, position);
    }
    internal void M_SetScroll(Vector2 position) //added for unity
    {
        if (textArea.unityComponentGroup != null)
        {
            UnityScrollComponents scrollcomponents = (UnityScrollComponents)textArea.unityComponentGroup;
            scrollcomponents.scrollComponent.normalizedPosition = position;

            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollcomponents.rectComponent); //refresh layout
        }
        else
        {
            Debug.LogError("Can't find scroll rect to set position");
        }
    }

    internal void ClearScreen()
    {
	    textArea.selectAll();
	    textArea.replaceRange("", textArea.getSelectionStart(), textArea.getSelectionEnd());

        SetScroll(new Vector2(0, 1));
    }
}
