//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.applet.*;
//import java.awt.event.*;
//import java.lang.Thread;
//import java.awt.event.KeyEvent;
using System;
using UnityEngine;
using UnityEngine.UI;

public class JavaApplet
{
    public JavaColor backgroundColor = new JavaColor(new Color(1, 1, 0.8f));

    internal JavaURL getCodeBase() //Gets the base URL.
    {
        return new JavaURL();
    }

    internal void stop() //Called by the browser or applet viewer to inform this applet that it should stop its execution.
    {
        //unnecessary in unity
    }

    internal string getParameter(string v)
    {
        string str = "";
        switch (v)
        {
            case "num": //username?
                str = UnityJavaInterface.GetNumParam(); 
                break;
            case "hash": //password?
                str = UnityJavaInterface.GetHashParam();
                break;
            case "time": // ???
                str = UnityJavaInterface.GetTimeParam();
                break;
        }
        return str;

        /*print "<APPLET CODE=\"pClient.class\" CODEBASE=\"/118_client\" ARCHIVE=\"118_client.jar\" HEIGHT=0 WIDTH=0>
         * <PARAM name=\"num\" value=\"$cookie{'num'}\">
         * <PARAM name=\"hash\" value=\"$cookie{'hash'}\">
         * <PARAM name=\"time\" value=\"$ENV{'QUERY_STRING'}\">";    //QUERY_STRING is set by the web server it is simply the query string from the uri
         */
    }

    internal Sprite getImage(JavaURL URL, string filename)
    {
        //e.g.  parent.theImages[0] = parent.getImage(parent.getCodeBase(), "shield.gif");
        return UnityJavaInterface.GetImage(URL, filename);
    }
}