//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.applet.*;
//import java.awt.event.*;
//import java.lang.Thread;
//import java.awt.event.KeyEvent;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;

public class pClient : JavaApplet, IJavaKeyListener
{

    /* Main Thread */
    JavaThread mainThread = null;

    /* Window components */
    public Frame f = new Frame("Phantasia v4");
    public statusPne status;
    public buttonPne buttons;
    public msgPne messages;
    public userPne users;
    public chatPne chat;
    public compassPne compass;
    public errorDlog errorDialog;

    public JavaPanel rightPane = new JavaPanel("Right", false);
    public Sprite[] theImages = new Sprite[27];


    public Socket socket = null;
    lThread listen = null;
    DataOutputStream output = null;
    BufferedReader input = null;

    int ioStatus = constants.NO_REQUEST;

    public pClient()
    {
        status = new statusPne(this);
        buttons = new buttonPne(this);
        messages = new msgPne(this);
        users = new userPne(this);
        chat = new chatPne(this);
        compass = new compassPne(this);
        errorDialog = new errorDlog(this, true);
        
        init();
    }


    internal void windowClosing(WindowEvent evt = null)
    {
        pClientQuit();
    }

    internal void init()
    {
        Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": PCLIENT INIT");

        f.addIJavaKeyListener(this);

        f.setSize(600, 400);

        GridBagConstraints constraints = new GridBagConstraints();
        //added for unity
        GridBagLayout bagLayout = new GridBagLayout();
        GridBagLayout.setConstraints(f, constraints);


        /* handle the closing of the window */
        f.addWindowListener(new WindowAdapter(windowClosing)); //unnecessary for unity

        //addComponent include constraint setting
        addComponent(status, 0, 0, 2, 1, 0, 0, "status");
        addComponent(messages, 0, 1, 1, 1, 1, 1, "messages");
        addComponent(buttons, 0, 2, 1, 1, 0, 0, "buttons");
        addComponent(chat, 0, 3, 1, 1, 1, 2, "chat");
        addComponent(rightPane, 1, 1, 1, 3, 0, 0, "right");

        f.setLayout(bagLayout);

        //rightPane.setLayout(new BorderLayout(0, 1));
        rightPane.add("South", compass);
        rightPane.add("Center", users);
        rightPane.setLayout(new BorderLayout(0, 1));

        f.setBackground(backgroundColor);

        /*
            f.pack();
        */

        /* show the frame */
        f.setVisible(true);

        status.loadImages();
        buttons.setImages();

        /* set up the socket */
        try
        {
            socket = new Socket(this.getCodeBase().getHost(), 43302);
            output = new DataOutputStream(socket.getOutputStream());
            input = new BufferedReader(new InputStreamReader(socket.getInputStream()));
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e);
            errorDialog.bringUp("The system can not connect to the server.",
                "The server could be down or a firewall could exist between you and it.",
                "Please try again later.");
            stop();
        }

        UnityGameController.inSetup = false;

        /* start the listen thread */
        listen = new lThread(this);
        listen.start();
    }

    private void addComponent(JavaPanel item, int gridx, int gridy, int gridwidth, int gridheight, int weightx, int weighty, string panelType) //unity: added panelType
    {
        GridBagConstraints constraints = new GridBagConstraints();

        /* add the status area to the frame */
        constraints.gridx = gridx;
        constraints.gridy = gridy;
        constraints.gridwidth = gridwidth;
        constraints.gridheight = gridheight;
        constraints.weightx = weightx;
        constraints.weighty = weighty;
        constraints.fill = GridBagConstraints.BOTH;
        constraints.anchor = GridBagConstraints.CENTER;
        constraints.insets.top = 2;
        constraints.insets.bottom = 2;
        constraints.insets.left = 2;
        constraints.insets.right = 2;

        GridBagLayout.setConstraints(item, constraints, panelType);
        f.add(item);
    }

    internal void pClientQuit()
    {
       Debug.Log("pClientQuit called.");

        /* destroy the listen thread */
        if (listen != null)
        {
            listen.stop();
            listen = null;
        }

       Debug.Log("Listen Thread Stopped.");
        /* close the socket */
        if (socket != null)
        {
            try
            {
                socket.close();
            }
            catch (IOException e)
            {
               Debug.Log("Error: " + e);
            }
            socket = null;
        }
       Debug.Log("Socket Closed.");

        /* remove the main windw */
        f.setVisible(false);
       Debug.Log("Main window hidden.");
        f.dispose();
       Debug.Log("Main window disposed.");
    }

    internal string readString()
    {
	    string message = "";

        try
        {
            message = input.readLine();
            message = message.Replace('\0', '¬').Replace('$', '¬').Replace("¬", "");
        }
        catch (Exception e)
        {
            //Debug.Log("Error: " + e);
            errorDialog.bringUp("There was an error reading from the socket.",
                    "readString error: " + e, "The game will now terminate.");
        }
        /*
       Debug.Log("Returning from socket: " + message);
        */
        return (message);
    }

    internal long readLong()
    {
        long result = 0;
        string str = readString();
        try
        {
            result = long.Parse(str);
        }
        catch (Exception e)
        {
            if (str != "")
            {
                string someStringFiltered = str.Replace('\0', '$');
                Debug.LogError("Java client: Readlong exception: " + e);
                Debug.LogError("Java client continued: input string: || " + someStringFiltered + " ||");
            }
        }
        return result;
    }


    internal int readBool()
    {
	    string message = "";

        try
        {
            message = input.readLine();
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e);
            errorDialog.bringUp("There was an error reading from the socket.",
                    "readString error: " + e, "The game will now terminate.");
        }

        if (message == "No")
        {
            return 0;
        }
        else if (message == "Yes")
        {
            return 1;
        }
        else
        {
            Debug.Log("Error: readBool read the string " + message);
            errorDialog.bringUp("There was an error reading from the socket.",
                    "readBool read the string " + message,
                "The game will now terminate.");
        }
        return -1; //added
    }

    //synchronized 
    internal void sendString(string theString)
    {
        try
        {
            //Debug.Log("Java thread writing data: " + theString);
            output.writeBytes(theString);
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e);
            errorDialog.bringUp("There was an error writing to the socket.",
                "sendString error: " + e, "The game will now terminate.");
        }
    }

    //synchronized 
    internal bool pollSendFlag(int ioArea)
    {
        /*
       Debug.Log("Request Send flag - Poll=" + Integer.toString(ioArea) + "  Status=" + Integer.toString(ioStatus));
        */
        if (ioStatus == ioArea)
        {
            ioStatus = constants.NO_REQUEST;
            return true;
        }

        return false;
    }

    internal void raiseSendFlag(int ioArea)
    {
        if (ioStatus != constants.NO_REQUEST)
        {
            errorDialog.bringUp("Client attempted to have two i/o sources.",
                "The game will now terminate.", "");
        }

        ioStatus = ioArea;
    }

    internal void handlePing()
    {
        int ioArea = ioStatus;

        if (ioStatus != constants.NO_REQUEST && pollSendFlag(ioArea))
        {

            sendString(constants.C_PING_PACKET);

            switch (ioArea)
            {

                case constants.BUTTONS:
                    buttons.timeout();
                    break;

                case constants.STRING_DLOG:
                    listen.stringDialog.timeout();
                    break;

                case constants.COORD_DLOG:
                    listen.coordinateDialog.timeout();
                    break;

                case constants.PLAYER_DLOG:
                    listen.playerDialog.timeout();
                    break;
            }
        }
    }

    public void keyPressed(KeyEvent evt)
    {
        int theKey;

        theKey = evt.getKeyCode();

        switch (theKey)
        {

            case KeyEvent.VK_1:
            case KeyEvent.VK_F1:
                buttons.DoJavaButton(0);
                break;
            case KeyEvent.VK_2:
            case KeyEvent.VK_F2:
                buttons.DoJavaButton(1);
                break;
            case KeyEvent.VK_3:
            case KeyEvent.VK_F3:
                buttons.DoJavaButton(2);
                break;
            case KeyEvent.VK_4:
            case KeyEvent.VK_F4:
                buttons.DoJavaButton(3);
                break;
            case KeyEvent.VK_5:
            case KeyEvent.VK_F5:
                buttons.DoJavaButton(4);
                break;
            case KeyEvent.VK_6:
            case KeyEvent.VK_F6:
                buttons.DoJavaButton(5);
                break;
            case KeyEvent.VK_7:
            case KeyEvent.VK_F7:
                buttons.DoJavaButton(6);
                break;
            case KeyEvent.VK_8:
            case KeyEvent.VK_F8:
                buttons.DoJavaButton(7);
                break;
            case KeyEvent.VK_NUMPAD1:
                compass.DoJavaButton(6);
                break;
            case KeyEvent.VK_NUMPAD2:
                compass.DoJavaButton(7);
                break;
            case KeyEvent.VK_NUMPAD3:
                compass.DoJavaButton(8);
                break;
            case KeyEvent.VK_NUMPAD4:
                compass.DoJavaButton(3);
                break;
            case KeyEvent.VK_NUMPAD5:
                compass.DoJavaButton(4);
                break;
            case KeyEvent.VK_NUMPAD6:
                compass.DoJavaButton(5);
                break;
            case KeyEvent.VK_NUMPAD7:
                compass.DoJavaButton(0);
                break;
            case KeyEvent.VK_NUMPAD8:
                compass.DoJavaButton(1);
                break;
            case KeyEvent.VK_NUMPAD9:
                compass.DoJavaButton(2);
                break;
            case KeyEvent.VK_SPACE:
                buttons.spacebar();
                break;
            case KeyEvent.VK_PU:
                //popup
                break;
            case KeyEvent.VK_PD:

                break;
        }

        return;
    }

    public void keyReleased(KeyEvent evt)
    {;}
    public void keyTyped(KeyEvent evt)
    {;}

}
