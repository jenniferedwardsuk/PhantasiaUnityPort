//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.applet.*;
//import java.awt.event.*;
//import java.lang.Thread;
//import java.awt.event.KeyEvent;
using System;
using System.Text;
using UnityEngine;

internal class DataOutputStream //DataOutputStream(OutputStream out) //Creates a new data output stream to write data to the specified underlying output stream.
{
    public DataOutputStream(OutputStream p) //An output stream accepts output bytes and sends them to some sink.
    {
        // only called in pClient during socket setup:  output = new DataOutputStream(socket.getOutputStream());
    }

    internal void writeBytes(string theString) //writeBytes(String s) //Writes out the string to the underlying output stream as a sequence of bytes.
    {
        //Debug.Log("Java thread sending data: " + theString);
        //only called from pClient.sendString using DataOutputStream output 
        byte[] bytes = Encoding.ASCII.GetBytes(theString);
        UnityJavaInterface.GetInstance().SendToSocket(bytes);
    }
}