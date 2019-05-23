using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Networking;
using phantasiaclasses;
using System.Threading;
using System;

public class UnityGameController : NetworkBehaviour {

    [SerializeField] internal Button ActionButton1;
    [SerializeField] internal Button ActionButton2;
    [SerializeField] internal Button ActionButton3;
    [SerializeField] internal Button ActionButton4;
    [SerializeField] internal Button ActionButton5;
    [SerializeField] internal Button ActionButton6;
    [SerializeField] internal Button ActionButton7;
    [SerializeField] internal Button ActionButton8;

    [SerializeField] internal Button CompassButtonN;
    [SerializeField] internal Button CompassButtonNE;
    [SerializeField] internal Button CompassButtonE;
    [SerializeField] internal Button CompassButtonSE;
    [SerializeField] internal Button CompassButtonS;
    [SerializeField] internal Button CompassButtonSW;
    [SerializeField] internal Button CompassButtonW;
    [SerializeField] internal Button CompassButtonNW;
    [SerializeField] internal Button CompassButtonRest;

    [SerializeField] internal Button ChatButtonSend;
    [SerializeField] internal Button ChatButtonClear;

    [SerializeField] internal Button DialogButtonOK;
    [SerializeField] internal Button DialogButtonCancel;

    NetworkManager networkManager;
    ServerManager serverManager;
    GameObject networkManagerObj;

    public static bool inSetup = true;

    // Use this for initialization
    void Start ()
    {
        StopApplication = false;
        editorIsPlaying = true;
        networkManagerObj = GameObject.FindGameObjectWithTag("NetworkManager");
        if (networkManagerObj)
        {
            networkManager = networkManagerObj.GetComponent<NetworkManager>();
            serverManager = networkManagerObj.GetComponent<ServerManager>();
        }
        if (networkManager && serverManager)
        {
            
        }
        else
        {
            Debug.LogError("Error: NetworkManager or ServerManager not found for UnityGameController.");
            return;
        }

        //label unity thread
        //unityThread = Thread.CurrentThread;
        //unityThread.Name = "UnityThread"; //gives 'can only set once' error on repeated play modes
        CLibPThread.unityThread = Thread.CurrentThread;

        //start main game thread //todo: encapsulate this in CLibPThread, it's only in here for StartMain which isn't needed
        ThreadStart childref = new ThreadStart(StartMain);
        Debug.Log("UnityGameController: creating main thread");
        childThread = new Thread(childref);
        childThread.Name = ServerThreadName;// + CLibPThread.nextThreadID;
        //add to phantasia's thread list (this is the main server thread)
        CLibPThread.pthread_t the_thread = new CLibPThread.pthread_t(childThread); //sets pid_t //ensuring Thread name has been set before running this
        CLibPThread.knownThreads.Add(childThread.Name, the_thread);
        //the_thread.associatedSocket = LinuxLibSocket.SocketListManager.AddSocket();//new LinuxLibSocket.LinuxSocket(the_thread.tID, LinuxLibSocket.SocketState.LISTEN); 
        childThread.Start();
    }

    public static string ServerThreadName = "ServerThread";

    //Thread unityThread;
    Thread childThread;

    internal static bool editorIsPlaying;

    [SyncVar] internal bool stopApplicationShared = false;
    private static bool stopApplication = false;

    internal static bool DebugLogEnqueueing = false;
    internal static bool MUTEX_DEBUG = false; //todo
    internal static bool SUSPEND_DEBUG = true;
    internal static bool SEND_DEBUG = true;
    internal static bool SEND_PACKET_DEBUG = true;
    internal static bool SEND_QUEUE_DEBUG = true;
    internal static bool RECEIVE_DEBUG = true;
    internal static bool RECEIVE_PACKET_DEBUG = true;

    internal static bool StopApplication
    {
        get
        {
            return stopApplication;
        }

        set
        {
            if (value == true)
                Debug.Log("setting stop flag");
            stopApplication = value;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopApplication = true;
        }

        if (StopApplication)
        {
            Debug.Log("UnityGameController: stop flag set; quitting application");

            //if (serverManager)
            //serverManager.stopApplicationShared = stopApplication;
            stopApplicationShared = StopApplication; //send stop command to clients

#if UNITY_EDITOR
            // Application.Quit() does not work in the editor
            OnApplicationQuit();
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
        else
        {
            //if (serverManager)
            //stopApplication = serverManager.stopApplicationShared;
            StopApplication = stopApplicationShared; //get any stop commands from clients
            //Debug.Log("UnityGameController: continuing happily");
        }
    }

    private void OnApplicationQuit()
    {
        StopApplication = true;
        editorIsPlaying = false;
        //Debug.Log("killing threads, current count: " + CLibPThread.knownThreads.Values.Count);
        foreach (CLibPThread.pthread_t thread in CLibPThread.knownThreads.Values)
        {
            thread.associatedThread.Interrupt(); //fails to end thread caught in infinite method call loop
            thread.associatedThread.Abort(); //generally works but throws catch-avoiding exception, and does not work for infinite while loops
            //mostly relying on threads checking for stopApplication and stopping themselves
        }
    }

    private void OnDestroy()
    {
        //if (editorIsPlaying) //don't be redundant
            OnApplicationQuit(); //kill threads
    }

    void StartMain()
    {
        try
        {
            char[] args = new char[1] { ' ' }; //no special args
            main CSrc = main.GetInstance();
            CSrc.mainmain(args.Length, args);
        }
        catch (Exception e)
        {
            if (!e.Message.Contains("Thread was being aborted") && !e.StackTrace.Contains("Thread:Sleep_internal")) // this is for debug, filtering out 'thread aborted while socket selecting/sleeping' or exceptions
            {
                Debug.LogError("Exception in server thread: " + e.Message + " || " + e.InnerException);
                Debug.LogError(e.StackTrace);
            }
            else
            {
                Debug.Log("<color=red>Exception in server thread: " + e.Message + " || " + e.InnerException + " ||</color>");
                Debug.Log(e.StackTrace);
            }
        }
    }

    //public static void AbortThread()
    //{
    //    System.Threading.Thread.CurrentThread.Abort();
    //}
}
