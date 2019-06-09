using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

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

public class Main : MonoBehaviour
{
    public GameObject mainLayout;

    public static string rulePath;
    public string AndroidSdcard = "/sdcard/DuelAI";
    public string rule = "default";//默认规则
    public static string sqlName = "cards.db";
    public static string tableName = "cards";

    // Use this for initialization
    void Start()
    {
        UpdateRulePath();
        CardSpriteManager.Initialize();
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidInitialize();
        Thread update = new Thread(AndroidUpdate);
        update.Start();
#endif
        Instantiate(mainLayout, GameObject.Find("Canvas").transform);
        Instantiate(Resources.Load("Prefabs/FPSText"), GameObject.Find("Canvas").transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateRulePath()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        rulePath = Application.streamingAssetsPath + "/" + rule;
#elif UNITY_ANDROID
        rulePath = AndroidSdcard + "/" + rule;
#endif
    }

    public void AndroidInitialize()
    {
        string path = AndroidSdcard;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        path = rulePath;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        GetFile(Application.streamingAssetsPath + "/" + rule + "/" + sqlName, path + "/" + sqlName);
        path = rulePath + "/pics";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        path = rulePath + "/deck";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        SQLManager sql = new SQLManager();
        sql.ConnectSQL();
        SqliteDataReader reader = sql.GetCardsCount(tableName, "");
        Progress.overallpro = int.Parse(reader.GetValue(0).ToString());
        Progress.progress = 0;
        reader.Close();
        sql.CloseSQLConnection();

        Instantiate(Resources.Load("Prefabs/ProgressBackground"), GameObject.Find("Canvas").transform);
    }

    public void AndroidUpdate()
    {
        string path = rulePath + "/pics";
        SQLManager sql = new SQLManager();
        sql.ConnectSQL();
        SqliteDataReader reader = sql.ReadCardsId(tableName, "");
        while (reader.Read())
        {
            string id = reader.GetValue(0).ToString();
            string cardjpg = id + ".jpg";
            GetFile(Application.streamingAssetsPath + "/" + rule + "/pics/" + cardjpg, path + "/" + cardjpg);
            Progress.progress++;
        }
        reader.Close();
        sql.CloseSQLConnection();
    }

    public void GetFile(string readpath, string writepath)
    {//把数据库从安装包复制到安卓可写路径中，注：在安装包中无法读写数据
        if (!File.Exists(writepath))
        {
            /*
            WWW www = new WWW(readpath);
            while (!www.isDone) { }
            if (string.IsNullOrEmpty(www.error)) File.WriteAllBytes(writepath, www.bytes);
            */
            UnityWebRequest webRequest = UnityWebRequest.Get(readpath);
            webRequest.SendWebRequest();
            while (!webRequest.isDone) { }
            if (string.IsNullOrEmpty(webRequest.error))
                File.WriteAllBytes(writepath, webRequest.downloadHandler.data);
        }
    }
}
