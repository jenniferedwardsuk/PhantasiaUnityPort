using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfNotEditor : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if !UNITY_EDITOR
        this.gameObject.SetActive(false);
#endif
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
