using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine.UI;

public class SQLManager {

    private SqliteConnection connection;
    private SqliteCommand command;
    private SqliteDataReader reader;
    public string sqlName = "cards.db";
    private string dbPath;
 
    public void ConnectSQL()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.LinuxPlayer)
        {
            dbPath = Application.streamingAssetsPath + "/" + sqlName;
            connection = new SqliteConnection("data source="+dbPath);
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            dbPath = Application.persistentDataPath + "/" + sqlName;
            if (!File.Exists(dbPath))
            {//把数据库从安装包复制到安卓可写路径中，注：sqlite不能在安装包中读取数据
                WWW loader = new WWW("jar:file://" + Application.dataPath + "!/assets/" + sqlName);
                while (!loader.isDone) { }
                File.WriteAllBytes(dbPath, loader.bytes);
            }
            connection = new SqliteConnection("URI=file:" + dbPath);
        }
        connection.Open();
        command = connection.CreateCommand();
        Debug.Log("数据库连接成功");
    }

    public SqliteDataReader InsertData(string tableName, string[] fieldNames, object[] values)
    {
        command.CommandText = "insert into " + tableName + "(";

        for (int i = 0; i < fieldNames.Length; i++)
        {
            command.CommandText += fieldNames[i];
            if (i < fieldNames.Length - 1)
            {
                command.CommandText += ",";
            }
        }

        command.CommandText += ")" + "values (";

        for (int i = 0; i < values.Length; i++)
        {
            command.CommandText += "\"";
            command.CommandText += values[i];
            command.CommandText += "\"";

            if (i < values.Length - 1)
            {
                command.CommandText += ",";
            }
        }

        command.CommandText += ")";

        Debug.Log(command.CommandText);

        return command.ExecuteReader();

    }

    public void CloseSQLConnection()
    {
        if (command != null)
        {
            command.Cancel();
        }

        if (reader != null)
        {
            reader.Close();
        }

        if (connection != null)
        {
            connection.Close();

        }
        command = null;
        reader = null;
        connection = null;
        Debug.Log("已经断开数据库连接");
    }
}
