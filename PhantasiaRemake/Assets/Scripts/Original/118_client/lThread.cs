//import java.awt.*;
//import java.net.*;
//import java.io.*;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;

internal class lThread : JavaThread
{

    pClient parent = null;

    internal examinDlog examinePlayer = null;
    internal stringDlog stringDialog = null;
    internal coordDlog coordinateDialog = null;
    internal playerDlog playerDialog = null;
    internal scoreDlog scoreDialog = null;
    internal stdDlog stdDialog = null;
    internal detailDlog detailDialog = null;    

    public lThread(pClient c)
    {
	    parent = c;
    }

    internal void run()
    {
        try
        {
            int thePacket = 0;
            bool threadquittime = false; //added

            for (; ; ) //coroutine not usable as alternative, must wait for socket updates
            {
                if (UnityGameController.StopApplication || threadquittime) //time to quit
                {
                    Debug.Log("Java client: " + System.Threading.Thread.CurrentThread.Name + " stopping in lthread.run");
                    lThreadQuit();
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                thePacket = (int)parent.readLong();
                string msg = "Java client: Received packet ";
                /*
                System.out.println("Switching on packet " + thePacket);
                */
                switch (thePacket)
                {
                    case 0: //Unity: catch 0 packet (likely to be a loop interrupt / game shutting down, which is too late to display error to player (in LAN editor))
                        msg += "0 packet";
                        Debug.Log(msg);
                        break;

                    case constants.HANDSHAKE_PACKET:
                        msg += "HANDSHAKE_PACKET";
                        Debug.Log(msg);

                        //Debug.Log("Java thread sending version: " + clientVersion);
                        parent.sendString(constants.C_RESPONSE_PACKET + clientVersion + "\0");

                        //Debug.Log("Java thread sending num: " + parent.getParameter("num"));
                        parent.sendString(constants.C_RESPONSE_PACKET +
                        parent.getParameter("num") + "\0");

                        //Debug.Log("Java thread sending hash: " + parent.getParameter("hash"));
                        parent.sendString(constants.C_RESPONSE_PACKET +
                        parent.getParameter("hash") + "\0");

                        //Debug.Log("Java thread sending time: " + parent.getParameter("time"));
                        parent.sendString(constants.C_RESPONSE_PACKET +
                        parent.getParameter("time") + "\0");

                        break;

                    case constants.CLOSE_CONNECTION_PACKET:
                        msg += "CLOSE_CONNECTION_PACKET";
                        Debug.Log(msg);
                        lThreadQuit();
                        break;

                    case constants.PING_PACKET:
                        msg += "PING_PACKET";
                        Debug.Log(msg);
                        parent.handlePing();
                        break;

                    case constants.ADD_PLAYER_PACKET:
                        msg += "ADD_PLAYER_PACKET";
                        Debug.Log(msg);
                        parent.users.AddUser();
                        break;

                    case constants.REMOVE_PLAYER_PACKET:
                        msg += "REMOVE_PLAYER_PACKET";
                        Debug.Log(msg);
                        parent.users.RemoveUser();
                        break;

                    case constants.SHUTDOWN_PACKET:
                        msg += "SHUTDOWN_PACKET";
                        Debug.Log(msg);
                        parent.errorDialog.bringUp("The server is being brought down for maintenance.",
                            "Now saving your game and logging you off.",
                            "Please try back again in a few minutes.");
                        break;

                    case constants.CLEAR_PACKET:
                        msg += "CLEAR_PACKET";
                        Debug.Log(msg);
                        parent.messages.ClearScreen();
                        break;

                    case constants.WRITE_LINE_PACKET:
                        msg += "WRITE_LINE_PACKET";
                        Debug.Log(msg);
                        parent.messages.PrintLine();
                        break;

                    case constants.BUTTONS_PACKET:
                        msg += "BUTTONS_PACKET";
                        Debug.Log(msg);
                        parent.buttons.Question();
                        break;

                    case constants.FULL_BUTTONS_PACKET:
                        msg += "FULL_BUTTONS_PACKET";
                        Debug.Log(msg);
                        parent.buttons.turn();
                        break;

                    case constants.STRING_DIALOG_PACKET:
                        msg += "STRING_DIALOG_PACKET";
                        Debug.Log(msg);
                        stringDialog = new stringDlog(parent, false);
                        break;

                    case constants.COORDINATES_DIALOG_PACKET:
                        msg += "COORDINATES_DIALOG_PACKET";
                        Debug.Log(msg);
                        coordinateDialog = new coordDlog(parent);
                        break;

                    case constants.PLAYER_DIALOG_PACKET:
                        msg += "PLAYER_DIALOG_PACKET";
                        Debug.Log(msg);
                        playerDialog = new playerDlog(parent);
                        break;

                    case constants.PASSWORD_DIALOG_PACKET:
                        msg += "PASSWORD_DIALOG_PACKET";
                        Debug.Log(msg);
                        stringDialog = new stringDlog(parent, true);
                        break;

                    case constants.SCOREBOARD_DIALOG_PACKET:
                        msg += "SCOREBOARD_DIALOG_PACKET";
                        Debug.Log(msg);
                        scoreDialog = new scoreDlog(parent);
                        break;

                    case constants.DIALOG_PACKET:
                        msg += "DIALOG_PACKET";
                        Debug.Log(msg);
                        stdDialog = new stdDlog(parent);
                        break;

                    case constants.CHAT_PACKET:
                        msg += "CHAT_PACKET";
                        Debug.Log(msg);
                        parent.chat.PrintLine();
                        break;

                    case constants.ACTIVATE_CHAT_PACKET:
                        msg += "ACTIVATE_CHAT_PACKET";
                        Debug.Log(msg);
                        parent.chat.activateChat();
                        break;

                    case constants.DEACTIVATE_CHAT_PACKET:
                        msg += "DEACTIVATE_CHAT_PACKET";
                        Debug.Log(msg);
                        parent.chat.deactivateChat();
                        break;

                    case constants.PLAYER_INFO_PACKET:
                        msg += "PLAYER_INFO_PACKET";
                        Debug.Log(msg);
                        examinePlayer = new examinDlog(parent);
                        break;

                    case constants.CONNECTION_DETAIL_PACKET:
                        msg += "CONNECTION_DETAIL_PACKET";
                        Debug.Log(msg);
                        detailDialog = new detailDlog(parent);
                        break;

                    case constants.ERROR_PACKET:
                        msg += "ERROR_PACKET";
                        Debug.Log(msg);
                        parent.errorDialog.bringUp("Server sent an error packet.",
                        "Error: " + parent.readString(), "The game will now terminate.");
                        lThreadQuit();
                        break;

                    default:
                        if (thePacket >= constants.NAME_PACKET && thePacket <= constants.VIRGIN_PACKET)
                        {
                            msg += "nonspecific valid packet";
                            Debug.Log(msg);
                            parent.status.UpdateStatusPane(thePacket);
                        }
                        else
                        {
                            msg += "unknown packet";
                            Debug.Log(msg);
                            parent.errorDialog.bringUp("Client received an unknown packet.",
                            "readString error: " + thePacket, "The game will now terminate.");
                            lThreadQuit();
                            threadquittime = true; //added
                        }
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception in java thread: " + e.Message + " || " + e.InnerException);
            Debug.LogError(e.StackTrace);
        }
    }


    internal void lThreadQuit()
    {
        /* close the socket */
        if (parent.socket != null)
        {
            try
            {
                parent.socket.close();
            }
            catch (IOException e)
            {
                Debug.Log("Error: " + e);
            }
            parent.socket = null;
        }

        /* remove the main windw */
        parent.f.setVisible(false);
        parent.f.dispose();

        /* destroy the listen thread */
        stop();
    }
}
