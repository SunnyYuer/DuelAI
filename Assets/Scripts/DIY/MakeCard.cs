using Mono.Data.Sqlite;
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

    // Use this for initialization
    void Start ()
    {
        sql = new SQLManager();
        sql.ConnectSQL();
        if (Application.platform == RuntimePlatform.Android)
            picspath = Main.AndroidSdcard + "/" + Main.rule + "/pics/";
        else
            picspath = Application.streamingAssetsPath + "/" + Main.rule + "/pics/";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnQuitClick();
    }

    public void OnSearchClick()
    {
        RectTransform cardlist = GameObject.Find("CardList").GetComponent<RectTransform>();
        for (int i = 0; i < cardlist.childCount; i++)
        {
            Destroy(cardlist.GetChild(i).gameObject);
        }
        cardlist.sizeDelta = new Vector2(0, 5);

        string nameorid = GameObject.Find("SearchInputField").GetComponent<InputField>().text;
        float cardheight = card.GetComponent<RectTransform>().rect.height;
        SqliteDataReader reader = sql.ReadCardsAll("cards", nameorid);
        while (reader.Read())
        {
            cardlist.sizeDelta = new Vector2(0, cardlist.rect.height + cardheight + 5);
            string id = reader.GetString(reader.GetOrdinal("id"));
            string name = reader.GetString(reader.GetOrdinal("name"));
            string cardjpg = id + ".jpg";
            GameObject cardnext = Instantiate(card, cardlist);
            cardnext.GetComponentsInChildren<Text>()[0].text = name;
            cardnext.GetComponentsInChildren<Text>()[3].text = id;
            cardnext.GetComponentsInChildren<Image>()[1].sprite = getCardSprite(cardjpg);
        }
        reader.Close();
    }

    public void OnCardIdValueChanged(string id)
    {
        GameObject.Find("CardBigImage").GetComponent<Image>().sprite = getCardSprite(id + ".jpg");
    }

    public Sprite getCardSprite(string cardjpg)
    {
        string cardpath = picspath + cardjpg;
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
        SqliteDataReader reader = sql.ReadCardsId("cards",id);
        bool hasrows = reader.HasRows;
        reader.Close();
        if (hasrows)
            sql.UpdateCard("cards", new string[] { "name", "type", "describe" }, new string[] { name, type, describe }, id).Close();
        else
            sql.InsertCard("cards", new string[] { "id", "name", "type", "describe" }, new string[] { id, name, type, describe }).Close();
    }

    public void OnQuitClick()
    {
        sql.CloseSQLConnection();
        Destroy(gameObject);
    }
}
