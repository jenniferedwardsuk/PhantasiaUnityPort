//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.applet.*;
//import java.awt.event.*;
//import java.lang.Thread;
//import java.awt.event.KeyEvent;
using System;

internal class JavaThread
{
    //used by lThread.cs

    public string clientVersion { get; internal set; }

    public JavaThread()
    {
        clientVersion = UnityJavaInterface.GetClientVersion();
    }

    internal void start()
    {
        //intended to trigger the run method of lThread
        UnityJavaInterface.StartThread(this);
    }

    internal void stop()
    {
        //intended to halt the run method of lThread
        UnityJavaInterface.StopThread(this);
    }
}