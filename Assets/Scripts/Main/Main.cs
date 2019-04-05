using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnDIYButtonClick()
    {
        Instantiate(Resources.Load("Prefabs/CardMakerImage"), GameObject.Find("Canvas").transform);
    }
}
