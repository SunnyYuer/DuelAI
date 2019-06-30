﻿#undef UNITY_EDITOR

using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

/*
Windows:
Application.dataPath                   =DuelAI/Duel_Data
Application.streamingAssetsPath =DuelAI/Duel_Data/StreamingAssets
Application.persistentDataPath    =C:/Users/yuer/AppData/LocalLow/yuer/DuelAI

Android:
Application.dataPath                   =/data/app/com.yuer.DuelAI-2/base.apk
Application.streamingAssetsPath =jar:file:///data/app/com.yuer.DuelAI-2/base.apk!/assets
Application.persistentDataPath    =/storage/emulated/0/Android/data/com.yuer.DuelAI/files

Linux:
Application.dataPath                   =DuelAI/Duel_Data
Application.streamingAssetsPath =DuelAI/Duel_Data/StreamingAssets
Application.persistentDataPath    =/home/yuer/.config/unity3d/yuer/DuelAI
*/

public class Main : MonoBehaviour
{
    public GameObject mainLayout;

    public static string rulePath;
    public string AndroidSdcard = "/sdcard/DuelAI";
    public string rule;//规则default, duel
    public static string sqlName = "cards.db";
    public static string tableName = "cards";

    private string zipFilePath;
    private string unZipDir;
    private int zipFileVersion = 1;
    private int saveZipVersion;
    private DateTime saveVersionTime;
    private int versionTextTime;
    private int updateDone = 0;

    // Use this for initialization
    void Start()
    {
        CardSpriteManager.Initialize();
        Instantiate(mainLayout, GameObject.Find("Canvas").transform);
        Instantiate(Resources.Load("Prefabs/FPSText"), GameObject.Find("Canvas").transform);
        //PlayerPrefs.DeleteAll();
        ChangeRule();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateDone == 1)
        {
            SaveVersionInfo();
            updateDone = 0;
        }
    }

    public void UpdateRulePath()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        rulePath = Application.dataPath + "/.." + "/" + rule;
#elif UNITY_ANDROID
        rulePath = AndroidSdcard + "/" + rule;
#endif
    }

    public void ChangeRule()
    {
        UpdateRulePath();
#if UNITY_EDITOR || UNITY_STANDALONE
        zipFilePath = Application.streamingAssetsPath + "/" + rule + ".zip";
        unZipDir = Application.dataPath + "/..";
#elif UNITY_ANDROID
        unZipDir = AndroidSdcard;
        zipFilePath = AndroidSdcard + "/" + rule + ".zip";
        if (!Directory.Exists(unZipDir)) Directory.CreateDirectory(unZipDir);
        saveZipVersion = PlayerPrefs.GetInt(rule+"version", 0);
#endif
        saveVersionTime = TimeTool.GetDateTime(PlayerPrefs.GetInt(rule+"versionTime", 0));
        Debug.Log(saveVersionTime);
        Instantiate(Resources.Load("Prefabs/ProgressBackground"), GameObject.Find("Canvas").transform);
        Thread update = new Thread(UpdateAssets);
        update.Start();
    }

    public void SaveVersionInfo()
    {
        PlayerPrefs.SetInt(rule + "versionTime", versionTextTime);
#if UNITY_ANDROID
        PlayerPrefs.SetInt(rule + "version", zipFileVersion);
#endif
    }

    public void UpdateAssets()
    {
#if UNITY_ANDROID
        if (!File.Exists(zipFilePath) || saveZipVersion < zipFileVersion)
        {
            Progress.title = "复制资源";
            GetFile(Application.streamingAssetsPath + "/" + rule + ".zip", zipFilePath);
            Progress.progress++;
        }
#endif
        Progress.title = "读取资源";
        Progress.progress = 0;
        int num = GetUpdateNum();
        Progress.progress++;
        if (num > 0)
        {
            Progress.title = "解压资源";
            Progress.overallpro = num;
            Progress.progress = 0;
            UnzipRuleAssets();
        }
        Progress.destroy = 1;
        updateDone = 1;
    }

    public void GetFile(string readpath, string writepath)
    {//把数据库从安装包复制到安卓可写路径中，注：在安装包中无法读写数据
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

    public int GetUpdateNum()
    {
        int num = 0;
        ZipInputStream zipStream = new ZipInputStream(File.OpenRead(zipFilePath));
        ZipEntry zipEntry;
        while ((zipEntry = zipStream.GetNextEntry()) != null)
        {
            if (zipEntry.Name.Equals(rule + "/version.txt"))
            {
                versionTextTime = TimeTool.GetUnixTimeStamp(zipEntry.DateTime);
                //Debug.Log(versionTextTime);
            }
            string fileName = unZipDir + "/" + zipEntry.Name;
            if (!fileName.EndsWith("/"))
            {
                if (zipEntry.DateTime.Ticks <= saveVersionTime.Ticks && File.Exists(fileName)) break;
                num++;
                Debug.Log(fileName);
            }
        }
        zipStream.Close();
        return num;
    }

    public void UnzipRuleAssets()
    {
        ZipInputStream zipStream = new ZipInputStream(File.OpenRead(zipFilePath));
        ZipEntry zipEntry;
        while ((zipEntry = zipStream.GetNextEntry()) != null)
        {
            //最后写入zip文件的会被先读出来
            string fileName = unZipDir + "/" + zipEntry.Name;
            string dirName = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);
            if (!fileName.EndsWith("/"))
            {
                //有文件修改时间比存储的版本时间早则结束解压
                if (zipEntry.DateTime.Ticks <= saveVersionTime.Ticks && File.Exists(fileName)) break;
                FileStream streamWriter = File.Create(fileName);
                int size = 2048;
                byte[] data = new byte[size];
                while (true)
                {
                    size = zipStream.Read(data, 0, data.Length);
                    if (size > 0) streamWriter.Write(data, 0, size);
                    else break;
                }
                streamWriter.Close();
                Progress.progress++;
            }
        }
        zipStream.Close();
    }
}
