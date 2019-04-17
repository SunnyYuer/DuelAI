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
    private string picspath;
    private List<string> cardidList;
    private List<string> cardnameList;
    private Dictionary<string, Sprite> spriteList;

    // Use this for initialization
    void Start ()
    {
        sql = new SQLManager();
        sql.ConnectSQL();
        cardidList = new List<string>();
        cardnameList = new List<string>();
        spriteList = new Dictionary<string, Sprite>();
        if (Application.platform == RuntimePlatform.Android)
            picspath = Main.AndroidSdcard + "/" + Main.rule + "/pics/";
        else
            picspath = Main.streamAssetsPath + "/" + Main.rule + "/pics/";
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
            cardnext.GetComponentsInChildren<Image>()[1].sprite = getCardSprite(cardidList[num], true);
        }
        showcard = false;
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
}
