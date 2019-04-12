using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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

public class Main : MonoBehaviour {

    public GameObject CardMaker;

    public GameObject progress;
    public Image progressImage;
    public Text progressText;
    private int cardcount = 0;
    private int curpro = 1;
    private int androidUpdate = 0;

    public static string streamAssetsPath;
    public static string AndroidSdcard = "/sdcard/DuelAI";
    public static string rule = "default";//默认规则
    public static string sqlName = "cards.db";
    public static string tableName = "cards";

    public static Dictionary<string, Sprite> spriteDic;

    // Use this for initialization
    void Start ()
    {
        streamAssetsPath = Application.streamingAssetsPath;
        spriteDic = new Dictionary<string, Sprite>();
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidInitialize();
            Thread update = new Thread(AndroidUpdate);
            update.Start();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (androidUpdate == 1)
        {
            progressImage.fillAmount = 1f * curpro / cardcount;
            progressText.text = curpro + "/" + cardcount;
            if (curpro >= cardcount)
            {
                Destroy(progress);
                androidUpdate = 2;
            }
        }
    }

    public void AndroidInitialize()
    {
        progress = (GameObject)Instantiate(Resources.Load("Prefabs/ProgressBackground"), GameObject.Find("Canvas").transform);
        progressImage = GameObject.Find("ProgressImage").GetComponent<Image>();
        progressText = GameObject.Find("ProgressText").GetComponent<Text>();

        string path = AndroidSdcard;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        path = AndroidSdcard + "/" + rule;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        wwwGetFile(streamAssetsPath + "/" + rule + "/" + sqlName, path + "/" + sqlName);
        path = AndroidSdcard + "/" + rule + "/pics";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        SQLManager sql = new SQLManager();
        sql.ConnectSQL();
        SqliteDataReader reader = sql.GetCardsCount(tableName, "");
        cardcount = int.Parse(reader.GetValue(0).ToString());
        reader.Close();
        sql.CloseSQLConnection();

        androidUpdate = 1;
    }

    public void AndroidUpdate()
    {
        string path = AndroidSdcard + "/" + rule + "/pics";
        SQLManager sql = new SQLManager();
        sql.ConnectSQL();
        SqliteDataReader reader = sql.ReadCardsId(tableName, "");
        while (reader.Read())
        {
            string id = reader.GetValue(0).ToString();
            string cardjpg = id + ".jpg";
            wwwGetFile(streamAssetsPath + "/" + rule + "/pics/" + cardjpg, path + "/" + cardjpg);
            curpro++;
        }
        reader.Close();
        sql.CloseSQLConnection();
    }

    public void wwwGetFile(string readpath, string writepath)
    {//把数据库从安装包复制到安卓可写路径中，注：在安装包中无法读写数据
        if (!File.Exists(writepath))
        {
            WWW www = new WWW(readpath);
            while (!www.isDone) { }
            if (string.IsNullOrEmpty(www.error)) File.WriteAllBytes(writepath, www.bytes);
        }
    }

    public void OnDIYButtonClick()
    {
        Instantiate(CardMaker, GameObject.Find("Canvas").transform);
    }
}
