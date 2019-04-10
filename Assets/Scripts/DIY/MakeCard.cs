using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MakeCard : MonoBehaviour
{
    public GameObject card;
    private SQLManager sql;
    private string picspath;
    private int lastcardnum = 0;
    private Dictionary<string, Sprite> spriteList;

    // Use this for initialization
    void Start ()
    {
        sql = new SQLManager();
        sql.ConnectSQL();
        if (Application.platform == RuntimePlatform.Android)
            picspath = Main.AndroidSdcard + "/" + Main.rule + "/pics/";
        else
            picspath = Main.streamAssetsPath + "/" + Main.rule + "/pics/";
        spriteList = new Dictionary<string, Sprite>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnQuitClick();
        if (lastcardnum > 0)
            showpics();
    }

    public void OnSearchClick()
    {
        DateTime dt1 = DateTime.Now;

        if (CardClick.lastclickedcard != null)
            CardClick.lastclickedcard.GetComponent<Image>().color = Color.white;

        RectTransform cardlist = GameObject.Find("CardList").GetComponent<RectTransform>();
        int instancards = cardlist.childCount;//已实例化的卡片数量

        string nameorid = GameObject.Find("SearchInputField").GetComponent<InputField>().text;
        SqliteDataReader reader = sql.GetCardsCount(Main.tableName, nameorid);
        int cardnum = int.Parse(reader.GetValue(0).ToString());
        GameObject.Find("ResultText").GetComponent<Text>().text = cardnum.ToString();
        if (cardnum > 1000) cardnum = 1000;
        reader.Close();
        GameObject.Find("ResultText").GetComponent<Text>().text = cardnum.ToString() + "/" + GameObject.Find("ResultText").GetComponent<Text>().text;
        float cardheight = card.GetComponent<RectTransform>().rect.height;
        cardlist.sizeDelta = new Vector2(0, cardheight * cardnum + 5);

        reader = sql.ReadCardsAllLimit(Main.tableName, nameorid, cardnum, 0);
        int num = 0;
        while (reader.Read())
        {
            string id = reader.GetString(reader.GetOrdinal("id"));
            string name = reader.GetString(reader.GetOrdinal("name"));
            GameObject cardnext;
            if (num < instancards)
            {
                cardnext = cardlist.GetChild(num).gameObject;
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
        for (int i = num; i < lastcardnum; i++)
        {
            cardlist.GetChild(i).gameObject.SetActive(false);
        }
        lastcardnum = cardnum;

        DateTime dt2 = DateTime.Now;
        TimeSpan ts = dt2.Subtract(dt1);
        Debug.Log(ts.TotalMilliseconds);
    }

    public void showpics()
    {
        RectTransform cardlist = GameObject.Find("CardList").GetComponent<RectTransform>();
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
            string id = abs[abs.Length - 1];
            if (!spriteList.ContainsKey(id))
                cardnext.GetComponentsInChildren<Image>()[1].sprite = getCardSprite(id);
        }
    }

    public void OnCardIdValueChanged(string id)
    {
        GameObject.Find("CardBigImage").GetComponent<Image>().sprite = getCardSprite(id);
    }

    public Sprite getCardSprite(string id)
    {
        if (spriteList.ContainsKey(id))
            return spriteList[id];
        string cardpath = picspath + id + ".jpg";
        Sprite sprite = null;
        if (File.Exists(cardpath))
        {
            FileStream files = new FileStream(cardpath, FileMode.Open);
            byte[] imgByte = new byte[files.Length];
            files.Read(imgByte, 0, imgByte.Length);
            files.Close();
            Texture2D texture = new Texture2D(236, 344);
            texture.LoadImage(imgByte);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            spriteList.Add(id, sprite);
        }
        return sprite;
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
        Resources.UnloadUnusedAssets();//释放掉不再使用的Texture2D和Sprite
        sql.CloseSQLConnection();
        Destroy(gameObject);
    }
}
