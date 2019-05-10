using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class UnityPlayerController : NetworkBehaviour
{
    static UnityPlayerController instance;

    private void Awake()
    {
        
    }

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer)//instance == null)
        {
            instance = this;

            //todo: should do this first time only? irrelevant until other versions exist or cookie features are used
            ParamNum = "1004";
            ParamHash = "0";
            //PlayerPrefs.SetString("num", ParamNum); //client version
            //PlayerPrefs.SetString("hash", ParamHash); //machine ID //0 => no machine ID, cookies disabled. see io.Do_handshake 'if a machine number was passed'     //todo: set a machine ID to allow cookie features

            if (!PlayerPrefs.HasKey("num"))
            {
                PlayerPrefs.SetString("num", ParamNum);
            }
            else
            {
                ParamNum = PlayerPrefs.GetString("num", ParamNum);
            }

            if (!PlayerPrefs.HasKey("hash"))
            {
                PlayerPrefs.SetString("hash", ParamHash);
            }
            else
            {
                ParamHash = PlayerPrefs.GetString("hash", ParamHash);
            }
            PlayerPrefs.Save();
        }
        else
        {
            //Debug.LogError("Tried to create more than one player"); //todo: this would limit game to one player only
                                                                      //instead, only setting instance if islocalplayer
            //DestroyImmediate(this);
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (!isLocalPlayer)
            return;
        
        //todo: move to coroutine if performance suffers
        while (pendingData.Count > 0)
        {
            string datastring = Encoding.ASCII.GetString(pendingData[0]);
            Debug.Log("................................................UNITYPLAYERCONTROLLER DELIVERING DATA: " + datastring);
            CmdSendDataToServer(pendingData[0]);
            pendingData.RemoveAt(0);
        }

    }

    internal static UnityPlayerController GetInstance()
    {
        if (instance)
            return instance;
        else
        {
            Debug.LogError("Tried to get local player before player was instantiated");
            return null;
        }
    }

    [Command]
    internal void CmdSendDataToServer(byte[] message) //this method runs on the server when called by the client
    {
        UnityCServerInterface.ReceiveDataFromClient(this.playerControllerId, message);
    }

    internal void ReceiveDataFromServer(byte[] message)
    {
        if (isLocalPlayer)
        {
            //Debug.Log("Player " + this.playerControllerId + " received message");
            UnityJavaInterface.WaitingData = message; //set message as waiting data for java thread  //this should only run on client
        }
    }
    
    internal List<byte[]> pendingData = new List<byte[]> { };

    //return PlayerPrefs.GetString("num", "0"); // 1004 = correct version (currently)
    //return PlayerPrefs.GetString("hash", "0"); // 0 = cookies disabled
    public string ParamNum { get; internal set; }
    public string ParamHash { get; internal set; }
}
