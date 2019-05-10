using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Threading;

public class ServerManager : NetworkManager
{

    // Server callbacks

    public override void OnServerConnect(NetworkConnection conn)
    {

        Debug.Log("A client connected to the server: " + conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {

        NetworkServer.DestroyPlayersForConnection(conn);

        if (conn.lastError != NetworkError.Ok)
        {

            if (LogFilter.logError) { Debug.LogError("ServerDisconnected due to error: " + conn.lastError); }

        }

        Debug.Log("A client disconnected from the server: " + conn);

    }

    public override void OnServerReady(NetworkConnection conn)
    {

        NetworkServer.SetClientReady(conn);

        Debug.Log("Client is set to the ready state (ready to receive state updates): " + conn);

    }

    public static int ServerSocketNum = 99;
    public static short latestPlayerID = 99;


    internal static short PickupLatestPlayerID()
    {
        short ID = latestPlayerID;
        //latestPlayerID = -1; //don't reset
        return ID;
    }
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {

        GameObject player = (GameObject)GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        
        Debug.Log("created new network player with ID " + playerControllerId);

        latestPlayerID = playerControllerId;

        //add player to UnityCServer instance list
        UnityCServerInterface unityCintf = UnityCServerInterface.GetInstance();
        if (!unityCintf)
        {
            Debug.LogError("No CServerInstance available for new player event");
        }
        else
        {
            unityCintf.OnNewPlayer(player.GetComponent<UnityPlayerController>());
        }

        //signal main server thread to process new connection
        if (UnityGameController.ServerThreadName != null && CLibPThread.knownThreads.ContainsKey(UnityGameController.ServerThreadName))
        {
            Debug.Log("Setting SIGIO_ISPENDING on pthread for " + UnityGameController.ServerThreadName);
            //CLibPThread.knownThreads[UnityGameController.ServerThreadName].associatedSocket.CanRead = true;
            CLibPThread.knownThreads[UnityGameController.ServerThreadName].SetSignal(LinuxLibSIG.SIGIO, true);

        }
        else
        {
            Debug.LogError("Could not find main server thread for SIGIO on player add");
            UnityGameController.StopApplication = true;
        }

        LinuxLibSocket.LinuxSocket socket = LinuxLibSocket.SocketListManager.FileDescriptors.Find(x => x.FileDescriptor == ServerSocketNum); //server socket
        if (socket != null)
        {
            Debug.Log("Adding new connection to server socket pending list");
            socket.AddPendingConnection(playerControllerId);
        }
        else
        {
            Debug.LogError("Server socket not found for new player conn request");
        }
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        //remove player from UnityCServer instance list
        UnityCServerInterface unityCintf = UnityCServerInterface.GetInstance();
        if (!unityCintf)
        {
            Debug.LogError("No CServerInstance available for new player event");
        }
        else
        {
            if (player.gameObject != null)
            {
                unityCintf.OnPlayerDisconnect(player.gameObject.GetComponent<UnityPlayerController>());
            }
            //else if (player != null)
            //{
            //    unityCintf.OnPlayerDisconnect(player.playerControllerId);
            //}

        }

        if (player.gameObject != null)
            NetworkServer.Destroy(player.gameObject);
    }

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {

        Debug.Log("Server network error occurred: " + (NetworkError)errorCode);

    }

    public override void OnStartHost()
    {

        Debug.Log("Host has started");

    }

    public override void OnStartServer()
    {

        Debug.Log("Server has started");

    }

    public override void OnStopServer()
    {

        Debug.Log("Server has stopped");

    }

    public override void OnStopHost()
    {

        Debug.Log("Host has stopped");

    }

    // Client callbacks

    public override void OnClientConnect(NetworkConnection conn)

    {

        base.OnClientConnect(conn);

        Debug.Log("Connected successfully to server, now to set up other stuff for the client...");

    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {

        StopClient();

        if (conn.lastError != NetworkError.Ok)

        {

            if (LogFilter.logError) { Debug.LogError("ClientDisconnected due to error: " + conn.lastError); }

        }

        Debug.Log("Client disconnected from server: " + conn);

    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {

        Debug.Log("Client network error occurred: " + (NetworkError)errorCode);

    }

    public override void OnClientNotReady(NetworkConnection conn)
    {

        Debug.Log("Server has set client to be not-ready (stop getting state updates)");

    }

    public override void OnStartClient(NetworkClient client)
    {

        Debug.Log("Client has started");

    }

    public override void OnStopClient()
    {

        Debug.Log("Client has stopped");

    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {

        base.OnClientSceneChanged(conn);

        Debug.Log("Server triggered scene change and we've done the same, do any extra work here for the client...");

    }
}
