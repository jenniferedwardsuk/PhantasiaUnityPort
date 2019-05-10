//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.applet.*;
//import java.awt.event.*;
//import java.lang.Thread;
//import java.awt.event.KeyEvent;
using System;

internal class BufferedReader
{
    private InputStreamReader inputStreamReader;

    public BufferedReader(InputStreamReader inputStreamReader) //only used in pclient: input = new BufferedReader(new InputStreamReader(socket.getInputStream()));
    {
        this.inputStreamReader = inputStreamReader;
    }

    internal string readLine()
    {
        //intended to read from socket
        return UnityJavaInterface.ReadFromSocket();
    }
}