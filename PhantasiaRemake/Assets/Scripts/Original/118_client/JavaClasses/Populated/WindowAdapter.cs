//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.applet.*;
//import java.awt.event.*;
//import java.lang.Thread;
//import java.awt.event.KeyEvent;
using System;

internal class WindowAdapter
{
    private Action<WindowEvent> windowClosing;

    public WindowAdapter(Action<WindowEvent> windowClosing)
    {
        this.windowClosing = windowClosing;
        // unnecessary for unity
    }
}