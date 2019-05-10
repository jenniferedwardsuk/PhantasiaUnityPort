using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityKeyListener : MonoBehaviour {

    public pClient client;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //todo
    }

    public void SetClient(pClient targetClient)
    {
        client = targetClient;
    }
}
