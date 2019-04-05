using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tip : MonoBehaviour {

    public static string title = "提示";
    public static string content;

    // Use this for initialization
    void Start () {
        GameObject.Find("MessageTitle").GetComponent<Text>().text = title;
        GameObject.Find("MessageContent").GetComponent<Text>().text = content;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTipButtonClick()
    {
        Destroy(gameObject);
    }
}
