using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/*
Windows:
Application.dataPath                   =/Duel_Data
Application.streamingAssetsPath =/Duel_Data/StreamingAssets
Application.persistentDataPath    =C:/Users/yuer/AppData/LocalLow/yuer/DuelAI

Android:
Application.dataPath                   =/data/app/com.yuer.DuelAI-2/base.apk
Application.streamingAssetsPath =jar:file:///data/app/com.yuer.DuelAI-2/base.apk!/assets
Application.persistentDataPath    =/storage/emulated/0/Android/data/com.yuer.DuelAI/files

Linux:
Application.dataPath                   =/Duel_Data
Application.streamingAssetsPath =/Duel_Data/StreamingAssets
Application.persistentDataPath    =/home/yuer/.config/unity3d/yuer/DuelAI
*/

public class MakeCard : MonoBehaviour
{
    public GameObject card;
    public static string androidsdcard = "/sdcard/DuelAI";
    public static string rule = "fair";//默认规则
    public static string sqlName = "cards.db";
    private SQLManager sql;
    private string picspath;

    // Use this for initialization
    void Start ()
    {
        if (Application.platform == RuntimePlatform.Android) AndroidUpdateDatabase();
        sql = new SQLManager();
        sql.ConnectSQL();
        if (Application.platform == RuntimePlatform.Android) AndroidUpdatePics();
        if (Application.platform == RuntimePlatform.Android)
            picspath = androidsdcard + "/" + rule + "/pics/";
        else
            picspath = Application.streamingAssetsPath + "/" + rule + "/pics/";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AndroidUpdateDatabase()
    {
        string path = androidsdcard;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        path = androidsdcard + "/" + rule;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        wwwGetFile(Application.streamingAssetsPath + "/" + rule + "/" + sqlName, path + "/" + sqlName);
    }

    public void AndroidUpdatePics()
    {
        string path = androidsdcard + "/" + rule + "/pics";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        SqliteDataReader reader = sql.ReadTable("cards", "");
        while (reader.Read())
        {
            string id = reader.GetString(reader.GetOrdinal("id"));
            string cardjpg = id + ".jpg";
            wwwGetFile(Application.streamingAssetsPath + "/" + rule + "/pics/" + cardjpg, path + "/" + cardjpg);
        }
        reader.Close();
    }

    public void wwwGetFile(string readpath, string writepath)
    {//把数据库从安装包复制到安卓可写路径中，注：在安装包中无法读写数据
        if (!File.Exists(writepath))
        {
            WWW www = new WWW(readpath);
            while (!www.isDone) { }
            if(string.IsNullOrEmpty(www.error)) File.WriteAllBytes(writepath, www.bytes);
        }
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
        float cardheight = card.GetComponent<RectTransform>().rect.height;
        SqliteDataReader reader = sql.ReadTable("cards", nameorid);
        while (reader.Read())
        {
            cardlist.sizeDelta = new Vector2(0, cardlist.rect.height + cardheight + 5);
            string id = reader.GetString(reader.GetOrdinal("id"));
            string name = reader.GetString(reader.GetOrdinal("name"));
            string cardjpg = id + ".jpg";
            GameObject cardnext = Instantiate(card, cardlist);
            cardnext.SetActive(true);
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
        SqliteDataReader reader = sql.InsertData("cards", new string[] { "id", "name", "type", "describe" }, new string[] { id, name, type, describe });
        reader.Close();
    }
}
