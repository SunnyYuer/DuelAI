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
    private string picspath;
    private bool showcardpics;
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
        if (CardClick.lastclickedcard != null)
            CardClick.lastclickedcard.GetComponent<Image>().color = Color.white;

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

        int instancards = cardlist.childCount;//已实例化的卡片数量
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
            cardnext.GetComponentsInChildren<Image>()[1].sprite = getCardSprite(id, true);
        }
        showcardpics = false;
    }

    public void OnCardIdValueChanged(string id)
    {
        GameObject.Find("CardBigImage").GetComponent<Image>().sprite = getCardSprite(id, false);
    }

    public Sprite getCardSprite(string id, bool small)
    {
        if (Main.spriteDic.ContainsKey(id) && small)
            return Main.spriteDic[id];
        if (spriteList.ContainsKey(id) && !small)
            return spriteList[id];
        string cardpath = picspath + id + ".jpg";
        Sprite sprite = null;
        if (File.Exists(cardpath))
        {
            FileStream files = new FileStream(cardpath, FileMode.Open);
            byte[] imgByte = new byte[files.Length];
            files.Read(imgByte, 0, imgByte.Length);
            files.Close();
            Texture2D texture;
            if (Application.platform == RuntimePlatform.Android)
                texture = new Texture2D(236, 344, TextureFormat.ETC_RGB4, false);
            else
                texture = new Texture2D(236, 344, TextureFormat.DXT1, false);
            texture.LoadImage(imgByte);
            if (small) texture = ScaleTexture(texture, 59, 86);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            if (small) Main.spriteDic.Add(id, sprite);
            else
            {
                if (spriteList.Count >= 100)
                {
                    spriteList.Clear();//大图最多保存100张
                    Resources.UnloadUnusedAssets();
                }
                spriteList.Add(id, sprite);
            }
        }
        return sprite;
    }

    public Texture2D ScaleTexture(Texture2D sourceTex, int targetWidth, int targetHeight)
    {
        Texture2D destTex = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);
        Color[] destPix = new Color[destTex.width * destTex.height];

        // For each pixel in the destination texture...
        for (int y = 0; y < destTex.height; y++)
        {
            for (int x = 0; x < destTex.width; x++)
            {
                // Calculate the fraction of the way across the image
                // that this pixel positon corresponds to.
                float xFrac = x * 1.0f / (destTex.width - 1);
                float yFrac = y * 1.0f / (destTex.height - 1);

                // Get the non-integer pixel positions using GetPixelBilinear.
                destPix[y * destTex.width + x] = sourceTex.GetPixelBilinear(xFrac, yFrac);
            }
        }

        // Copy the pixel data to the destination texture and apply the change.
        destTex.SetPixels(destPix);
        destTex.Apply();
        Destroy(sourceTex);
        return destTex;
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
