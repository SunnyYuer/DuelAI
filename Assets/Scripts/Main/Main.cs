using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
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

    public GameObject Duel;
    public GameObject DeckEditor;
    public GameObject CardMaker;
    public GameObject FPSText;

    public static string streamAssetsPath;
    public static string AndroidSdcard = "/sdcard/DuelAI";
    public static string rule = "duel";//默认规则
    public static string sqlName = "cards.cdb";
    public static string tableName = "texts";

    private float currentTime = 0;
    private float lateTime = 0;
    private float framesNum = 0;
    private float fps = 0;

    // Use this for initialization
    void Start ()
    {
        streamAssetsPath = Application.streamingAssetsPath;
        CardSpriteManager.Initialize();
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidInitialize();
            Thread update = new Thread(AndroidUpdate);
            update.Start();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        currentTime += Time.deltaTime;
        framesNum++;
        if (currentTime - lateTime >= 1.0f)
        {
            fps = framesNum / (currentTime - lateTime);
            FPSText.GetComponent<Text>().text = "FPS：" + fps;
            lateTime = currentTime;
            framesNum = 0;
        }
    }

    public void AndroidInitialize()
    {
        string path = AndroidSdcard;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        path = AndroidSdcard + "/" + rule;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        GetFile(streamAssetsPath + "/" + rule + "/" + sqlName, path + "/" + sqlName);
        path = AndroidSdcard + "/" + rule + "/pics";
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
        string path = AndroidSdcard + "/" + rule + "/pics";
        SQLManager sql = new SQLManager();
        sql.ConnectSQL();
        SqliteDataReader reader = sql.ReadCardsId(tableName, "");
        while (reader.Read())
        {
            string id = reader.GetValue(0).ToString();
            string cardjpg = id + ".jpg";
            GetFile(streamAssetsPath + "/" + rule + "/pics/" + cardjpg, path + "/" + cardjpg);
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

    public void OnStartGameButtonClick()
    {
        Instantiate(Duel, GameObject.Find("Canvas").transform);
        FPSText.transform.SetAsLastSibling();
    }

    public void OnEditDeckButtonClick()
    {
        Instantiate(DeckEditor, GameObject.Find("Canvas").transform);
        FPSText.transform.SetAsLastSibling();
    }

    public void OnDIYButtonClick()
    {
        Instantiate(CardMaker, GameObject.Find("Canvas").transform);
        FPSText.transform.SetAsLastSibling();
    }

    public void OnQuitGameClick()
    {
        Application.Quit();
    }
}
