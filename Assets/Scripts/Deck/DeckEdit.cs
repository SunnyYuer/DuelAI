using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DeckEdit : MonoBehaviour {

    public RectTransform cardlist;
    public Scrollbar scrollbar;
    private SQLManager sql;
    private bool showcard = false;
    private int showcardnum = 0;
    private int cardtotalnum;
    private float scrollvalue;
    private List<string> cardidList;
    private List<string> cardnameList;
    private CardSpriteManager spriteManager;

    // Use this for initialization
    void Start ()
    {
        sql = new SQLManager();
        sql.ConnectSQL();
        cardidList = new List<string>();
        cardnameList = new List<string>();
        spriteManager = new CardSpriteManager();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (showcard)
            showcards();
    }

    public void OnSearchClick()
    {
        DateTime dt1 = DateTime.Now;

        string nameorid = GameObject.Find("SearchInputField").GetComponent<InputField>().text;
        SqliteDataReader reader = sql.GetCardsCount(Main.tableName, nameorid);
        cardtotalnum = int.Parse(reader.GetValue(0).ToString());
        reader.Close();
        showcardnum = cardtotalnum;//cardlist里要显示的卡片数量
        if (cardtotalnum > 7) showcardnum = 7;
        GameObject.Find("ResultText").GetComponent<Text>().text = cardtotalnum.ToString() + "/" + cardtotalnum.ToString();
        cardlist.sizeDelta = new Vector2(0, 90 * showcardnum);

        float scrollbarsize = 1F * showcardnum / cardtotalnum;
        if (scrollbarsize < 0.05) scrollbarsize = 0.05F;
        scrollbar.size = scrollbarsize;
        scrollbar.value = 1;
        scrollvalue = 0;

        cardidList.Clear();
        cardnameList.Clear();
        reader = sql.ReadCardsAll(Main.tableName, nameorid);
        while (reader.Read())
        {
            cardidList.Add(reader.GetValue(0).ToString());
            cardnameList.Add(reader.GetString(reader.GetOrdinal("name")));
        }
        reader.Close();
        for (int i = 0; i < 7; i++)
        {
            if(i < showcardnum) cardlist.GetChild(i).gameObject.SetActive(true);
            else cardlist.GetChild(i).gameObject.SetActive(false);
        }
        showcard = true;

        DateTime dt2 = DateTime.Now;
        TimeSpan ts = dt2.Subtract(dt1);
        Debug.Log("用时 " + ts.TotalMilliseconds + "ms");
    }

    public void OnScrollbarValueChanged(float value)
    {
        scrollvalue = 1F - value;
        showcard = true;
    }

    public void showcards()
    {
        for (int i = 0; i < showcardnum; i++)
        {
            GameObject cardnext = cardlist.GetChild(i).gameObject;
            Text cardAbstract = cardnext.GetComponentInChildren<Text>();
            int num = (int)(scrollvalue * (cardtotalnum - 7)) + i;
            //Debug.Log(num);
            cardAbstract.text = "";
            cardAbstract.text += cardnameList[num] + "\n";
            cardAbstract.text += cardidList[num];
            cardnext.GetComponentsInChildren<Image>()[1].sprite = spriteManager.getCardSprite(cardidList[num], true);
        }
        showcard = false;
    }

    public void OnQuitClick()
    {
        sql.CloseSQLConnection();
        Destroy(gameObject);
    }
}
