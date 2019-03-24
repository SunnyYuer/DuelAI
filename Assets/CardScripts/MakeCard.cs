using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class MakeCard : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public void OnSaveClick()
    {
        SQLManager sql = new SQLManager();
        sql.ConnectSQL();
        string name = GameObject.Find("InputField1").GetComponent<InputField>().text;
        string id = GameObject.Find("InputField2").GetComponent<InputField>().text;
        Dropdown dp = GameObject.Find("Dropdown").GetComponent<Dropdown>();
        string type = dp.options[dp.value].text;
        string describe = GameObject.Find("InputField").GetComponent<InputField>().text;
        sql.InsertData("cards", new string[] { "id", "name", "type", "describe" }, new string[] { id, name, type, describe });
        sql.CloseSQLConnection();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
