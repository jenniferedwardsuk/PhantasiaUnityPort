//import java.awt.*;
//import java.net.*;  <-- sockets
//import java.io.*;
//import java.applet.*;
//import java.awt.event.*;
//import java.lang.Thread;
//import java.awt.event.KeyEvent;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class Socket
{
    OutputStream outStream;
    InputStream inStream;

    //	Socket(String host, int port)        //Creates a stream socket and connects it to the specified port number on the named host.
    public Socket(string host, int port) //port is unused
    {
        outStream = new OutputStream();
        inStream = new InputStream();

        UnityJavaInterface.OpenSocket();
    }

    internal OutputStream getOutputStream() //used to set up dataoutputstream which passes socket data to/from unity interface
    {
        return outStream;
    }

    internal InputStream getInputStream() //used to set up bufferedreader which passes socket data to/from unity interface
    {
        return inStream;
    }

    internal void close()
    {
        UnityJavaInterface.CloseSocket();
    }
}