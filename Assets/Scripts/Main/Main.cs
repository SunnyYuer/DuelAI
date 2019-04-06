using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public Image progressImage;
    public Text progressText;

    public static string AndroidSdcard = "/sdcard/DuelAI";
    public static string rule = "default";//默认规则
    public static string sqlName = "cards.db";

    // Use this for initialization
    void Start () {
        if (Application.platform == RuntimePlatform.Android) AndroidUpdate();
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void AndroidUpdate()
    {
        GameObject progress = (GameObject)Instantiate(Resources.Load("Prefabs/ProgressBackground"), GameObject.Find("Canvas").transform);
        progressImage = GameObject.Find("ProgressImage").GetComponent<Image>();
        progressText = GameObject.Find("ProgressText").GetComponent<Text>();

        string path = AndroidSdcard;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        path = AndroidSdcard + "/" + rule;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        wwwGetFile(Application.streamingAssetsPath + "/" + rule + "/" + sqlName, path + "/" + sqlName);
        progressImage.fillAmount += 0.021f;
        progressText.text = (int)(progressImage.fillAmount * 100) + "%";

        path = AndroidSdcard + "/" + rule + "/pics";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        SQLManager sql = new SQLManager();
        sql.ConnectSQL();
        SqliteDataReader reader = sql.ReadCardsId("cards", "");
        while (reader.Read())
        {
            string id = reader.GetString(reader.GetOrdinal("id"));
            string cardjpg = id + ".jpg";
            wwwGetFile(Application.streamingAssetsPath + "/" + rule + "/pics/" + cardjpg, path + "/" + cardjpg);
            progressImage.fillAmount += 0.020f;
            progressText.text = (int)(progressImage.fillAmount * 100) + "%";
        }
        reader.Close();
        sql.CloseSQLConnection();

        Destroy(progress);
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
