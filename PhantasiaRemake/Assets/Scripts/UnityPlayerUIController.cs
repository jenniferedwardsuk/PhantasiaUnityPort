using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Threading;

public class UnityPlayerUIController : NetworkBehaviour {

    GameObject GameController;
    UnityGameController GameControllerScript;
    UnityJavaInterface JavaInterface;

    // Use this for initialization
    void Start ()
    {
        Debug.Log(Thread.CurrentThread.Name + ": starting player");
        GameController = GameObject.FindGameObjectWithTag("GameController");
        if (GameController)
            GameControllerScript = GameController.GetComponent<UnityGameController>();
        if (isLocalPlayer && GameControllerScript)
        {
            SubscribeFields();
            JavaInterface = new UnityJavaInterface();
            StartCoroutine(JavaSetup());
        }
        else
        {
            if (isLocalPlayer)
                Debug.LogError(Thread.CurrentThread.Name + ": Error: game controller script not found.");
        }
    }

    IEnumerator JavaSetup()
    {
        JavaInterface.StartClient(this);
        yield return null;
        JavaInterface.RefreshUI();
    }

    public bool retrieveAllUI = false; //for debug

	// Update is called once per frame
	void Update ()
    {
        if (isLocalPlayer)
        {
            if (JavaInterface != null)
            {
                if (UnityJavaInterface.startThread)
                {
                    Debug.Log(Thread.CurrentThread.Name + ": Starting listen thread");
                    StartCoroutine(RunThread(UnityJavaInterface.lthread)); //todo: replace with thread    //todo - freezes unity
                    UnityJavaInterface.startThread = false;
                }
                if (UnityJavaInterface.stopThread)
                {
                    Debug.Log(Thread.CurrentThread.Name + ": Stopping listen thread");
                    StopCoroutine(RunThread(UnityJavaInterface.lthread)); //todo: replace with thread
                    UnityJavaInterface.stopThread = false;
                }
            }

            //detect keyboard input
            if (Input.anyKeyDown
                && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2) && !Input.GetMouseButtonDown(3))
            {
                foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode))) //todo: slow? restrict to only expected keys?
                {
                    if (Input.GetKeyDown(kcode))
                    {
                        Debug.Log("KeyCode down: " + kcode);
                        //todo: ignore if chat messagebox is in use?
                        JavaInterface.KeyPressed(kcode);
                    }
                }
            }

            //todo
        }

        if (retrieveAllUI)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MainCanvas"))
            {
                UnityJavaInterface.RetrieveAllWanderingComponents(obj);
            }
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("PopupCanvas"))
            {
                UnityJavaInterface.RetrieveAllWanderingComponents(obj);
            }
        }
    }

    IEnumerator RunThread(lThread thread)
    {
        //thread.run();
        yield return null;
    }

    void SubscribeFields()
    {
        Debug.Log(Thread.CurrentThread.Name + ": subscribing buttons for player");

        GameControllerScript.ActionButton1.onClick.AddListener(delegate { ActionButton_Click(1); });
        GameControllerScript.ActionButton2.onClick.AddListener(delegate { ActionButton_Click(2); });
        GameControllerScript.ActionButton3.onClick.AddListener(delegate { ActionButton_Click(3); });
        GameControllerScript.ActionButton4.onClick.AddListener(delegate { ActionButton_Click(4); });
        GameControllerScript.ActionButton5.onClick.AddListener(delegate { ActionButton_Click(5); });
        GameControllerScript.ActionButton6.onClick.AddListener(delegate { ActionButton_Click(6); });
        GameControllerScript.ActionButton7.onClick.AddListener(delegate { ActionButton_Click(7); });
        GameControllerScript.ActionButton8.onClick.AddListener(delegate { ActionButton_Click(8); });

        GameControllerScript.CompassButtonNW.onClick.AddListener(delegate { CompassButton_Click(1); });
        GameControllerScript.CompassButtonN.onClick.AddListener(delegate { CompassButton_Click(2); });
        GameControllerScript.CompassButtonNE.onClick.AddListener(delegate { CompassButton_Click(3); });
        GameControllerScript.CompassButtonW.onClick.AddListener(delegate { CompassButton_Click(4); });
        GameControllerScript.CompassButtonRest.onClick.AddListener(delegate { CompassButton_Click(5); });
        GameControllerScript.CompassButtonE.onClick.AddListener(delegate { CompassButton_Click(6); });
        GameControllerScript.CompassButtonSW.onClick.AddListener(delegate { CompassButton_Click(7); });
        GameControllerScript.CompassButtonS.onClick.AddListener(delegate { CompassButton_Click(8); });
        GameControllerScript.CompassButtonSE.onClick.AddListener(delegate { CompassButton_Click(9); });

        GameControllerScript.ChatButtonSend.onClick.AddListener(delegate { ChatButton_Click(1); });
        GameControllerScript.ChatButtonClear.onClick.AddListener(delegate { ChatButton_Click(2); });

        GameControllerScript.DialogButtonOK.onClick.AddListener(delegate { DialogButton_Click(1); });
        GameControllerScript.DialogButtonCancel.onClick.AddListener(delegate { DialogButton_Click(2); });

        //Debug.Log("subscribed");
    }

    public void ActionButton_Click(int buttonNum)
    {
        Debug.Log(Thread.CurrentThread.Name + ": action button pressed: " + buttonNum);
        JavaInterface.ActionButton_Click(buttonNum);
    }
    public void CompassButton_Click(int buttonNum)
    {
        Debug.Log(Thread.CurrentThread.Name + ": compass button pressed: " + buttonNum);
        JavaInterface.CompassButton_Click(buttonNum);
    }
    public void ChatButton_Click(int buttonNum)
    {
        Debug.Log(Thread.CurrentThread.Name + ": chat button pressed: " + buttonNum);
        JavaInterface.ChatButton_Click(buttonNum);
    }
    public void DialogButton_Click(int buttonNum)
    {
        Debug.Log(Thread.CurrentThread.Name + ": dialog button pressed: " + buttonNum);
        JavaInterface.DialogButton_Click(buttonNum);
    }
}
