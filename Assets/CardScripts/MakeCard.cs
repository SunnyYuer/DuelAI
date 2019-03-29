using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class MakeCard : MonoBehaviour {

    public GameObject card;

    private SQLManager sql;

    // Use this for initialization
    void Start ()
    {
        sql = new SQLManager();
        sql.ConnectSQL();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSearchClick()
    {
        RectTransform cardlist = GameObject.Find("CardList").GetComponent<RectTransform>();
        for (int i = 1; i < cardlist.childCount; i++)
        {
            Destroy(cardlist.GetChild(i).gameObject);
        }
        cardlist.sizeDelta = new Vector2(0, 5);

        string nameorid = GameObject.Find("SearchInputField").GetComponent<InputField>().text;
        SqliteDataReader reader = sql.ReadTable("cards", nameorid);
        while (reader.Read())
        {
            string name = reader.GetString(reader.GetOrdinal("name"));
            GameObject cardnext = Instantiate(card, cardlist);
            cardnext.SetActive(true);
            cardnext.GetComponentInChildren<Text>().text = name;
            cardlist.sizeDelta = new Vector2(0, cardlist.rect.height+105);
        }
        reader.Close();
    }

    public void OnSaveClick()
    {
        string name = GameObject.Find("CardNameInputField").GetComponent<InputField>().text;
        string id = GameObject.Find("CardIdInputField").GetComponent<InputField>().text;
        Dropdown dp = GameObject.Find("Dropdown").GetComponent<Dropdown>();
        string type = dp.options[dp.value].text;
        string describe = GameObject.Find("DescribeInputField").GetComponent<InputField>().text;
        SqliteDataReader reader = sql.InsertData("cards", new string[] { "id", "name", "type", "describe" }, new string[] { id, name, type, describe });
        reader.Close();
    }
}
