using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MakeCard : MonoBehaviour
{
    public RectTransform cardlist;
    public GameObject card;
    public GameObject refreshtext;
    public static bool hasRefreshtext;
    private int lastcardnum = 0;
    private int cardtotalnum;
    private string nameorid;
    private SQLManager sql;
    private bool showcardpics;
    private SpriteManager spriteManager;

    // Use this for initialization
    void Start ()
    {
        sql = new SQLManager();
        sql.ConnectSQL();
        spriteManager = new SpriteManager();
        hasRefreshtext = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnQuitClick();
        if (showcardpics)
            showpics();
    }

    public void OnSearchClick()
    {
        if (CardListEvent.lastclickedcard != null)
            CardListEvent.lastclickedcard.GetComponent<Image>().color = Color.white;

        nameorid = GameObject.Find("SearchInputField").GetComponent<InputField>().text;
        SqliteDataReader reader = sql.GetCardsCount(Main.tableName, nameorid);
        int cardnum = int.Parse(reader.GetValue(0).ToString());
        reader.Close();
        cardtotalnum = cardnum;
        if (cardnum > 1000) cardnum = 1000;
        showcardlist(cardnum);
    }

    public void showcardlist(int cardnum)
    {
        DateTime dt1 = DateTime.Now;

        int instancards = cardlist.childCount;//已实例化的卡牌数量
        if (hasRefreshtext) instancards--;
        if (((cardtotalnum==cardnum) || ((cardnum-instancards)>=1000)) && hasRefreshtext)
        {
            Destroy(cardlist.GetChild(instancards).gameObject);
            hasRefreshtext = false;
        }
        GameObject.Find("ResultText").GetComponent<Text>().text = cardnum.ToString() + "/" + cardtotalnum.ToString();
        cardlist.sizeDelta = new Vector2(0, 90*cardnum + 5);

        int offset = (cardnum - 1) / 1000 * 1000;
        SqliteDataReader reader = sql.ReadCardsAllLimit(Main.tableName, nameorid, cardnum-offset, offset);
        int num = 0;
        while (reader.Read())
        {
            string id = reader.GetValue(0).ToString();
            string name = reader.GetString(reader.GetOrdinal("name"));
            GameObject cardnext;
            if ((num+offset) < instancards)
            {
                cardnext = cardlist.GetChild(num+offset).gameObject;
                cardnext.SetActive(true);
            }
            else
            {
                cardnext = Instantiate(card, cardlist);
            }
            Text cardAbstract = cardnext.GetComponentInChildren<Text>();
            cardAbstract.text = "";
            cardAbstract.text += name + "\n";
            cardAbstract.text += id;
            num++;
        }
        reader.Close();
        for (int i = (num+offset); i < lastcardnum; i++)
        {
            cardlist.GetChild(i).gameObject.SetActive(false);
        }
        lastcardnum = cardnum;
        if ((cardtotalnum > cardnum) && !hasRefreshtext)
        {
            hasRefreshtext = true;
            Instantiate(refreshtext, cardlist);
        }
        showcardpics = true;

        DateTime dt2 = DateTime.Now;
        TimeSpan ts = dt2.Subtract(dt1);
        Debug.Log("用时 " + ts.TotalMilliseconds + "ms");
    }

    public void OnScrollbarValueChanged()
    {
        showcardpics = true;
    }

    public void showpics()
    {
        float cardheight = card.GetComponent<RectTransform>().rect.height;
        float topposition = cardlist.anchoredPosition.y;
        float bottomposition = topposition + cardheight * 7;
        //Debug.Log(topposition+" "+ bottomposition);
        int topnum = (int)(topposition / cardheight);
        if (topnum < 0) topnum = 0;
        int bottomnum = (int)((bottomposition - 1.0f) / cardheight);
        if (bottomnum >= lastcardnum) bottomnum = lastcardnum - 1;
        //Debug.Log(topnum + " " + bottomnum);
        for (int i = topnum; i <= bottomnum; i++)
        {
            GameObject cardnext = cardlist.GetChild(i).gameObject;
            string cardAbstract = cardnext.GetComponentInChildren<Text>().text;
            string[] abs = cardAbstract.Split('\n');
            if (abs.Length == 1) continue;//refreshtext此时还未完全销毁
            string id = abs[abs.Length - 1];
            cardnext.GetComponentsInChildren<Image>()[1].sprite = spriteManager.GetCardSprite(id, true);
        }
        showcardpics = false;
    }

    public void OnCardIdValueChanged(string id)
    {
        GameObject.Find("CardBigImage").GetComponent<Image>().sprite = spriteManager.GetCardSprite(id, false);
    }

    public void OnSaveClick()
    {
        string name = GameObject.Find("CardNameInputField").GetComponent<InputField>().text;
        string id = GameObject.Find("CardIdInputField").GetComponent<InputField>().text;
        Dropdown dp = GameObject.Find("Dropdown").GetComponent<Dropdown>();
        string type = dp.options[dp.value].text;
        string describe = GameObject.Find("DescribeInputField").GetComponent<InputField>().text;
        SqliteDataReader reader = sql.ReadCardsId(Main.tableName, id);
        bool hasrows = reader.HasRows;
        reader.Close();
        if (hasrows)
            sql.UpdateCard(Main.tableName, new string[] { "name", "type", "describe" }, new string[] { name, type, describe }, id).Close();
        else
            sql.InsertCard(Main.tableName, new string[] { "id", "name", "type", "describe" }, new string[] { id, name, type, describe }).Close();
    }

    public void OnQuitClick()
    {
        sql.CloseSQLConnection();
        Destroy(gameObject);
        Resources.UnloadUnusedAssets();//释放掉不再使用的Texture2D和Sprite
    }
}
