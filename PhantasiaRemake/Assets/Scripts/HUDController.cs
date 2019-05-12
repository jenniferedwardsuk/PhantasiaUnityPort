using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class HUDController : MonoBehaviour {

    [SerializeField] Canvas JavaCanvas;
    [SerializeField] Button StartButton;
    NetworkManagerHUD unityHUD;
    public NetworkManager manager;

    static HUDController instance;
    internal static HUDController GetInstance()
    {
        if (instance)
            return instance;
        else
            return null;
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    // Use this for initialization
    void Start ()
    {
        StartButton.onClick.AddListener(StartButtonClicked);
        JavaCanvas.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnDestroy()
    {
        if (StartButton)
            StartButton.onClick.RemoveListener(StartButtonClicked);
    }

    public void StartButtonClicked()
    {
        JavaCanvas.gameObject.SetActive(true);
        if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
            manager.StartHost();
    }

    public void Disconnect()
    {
        if (NetworkServer.active && NetworkClient.active)
            manager.StopHost();

        StartButton.gameObject.SetActive(false); //todo: debug editor; hiding button until able to restart cleanly
        Application.Quit(); //todo: debug general; quit until able to restart cleanly
    }
}
