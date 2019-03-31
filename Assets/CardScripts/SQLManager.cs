using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine.UI;

public class SQLManager {

    private SqliteConnection connection;
    private SqliteCommand command;
    private SqliteDataReader reader;
 
    public void ConnectSQL()
    {
        string dbPath;
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.LinuxPlayer)
        {
            dbPath = Application.streamingAssetsPath + "/" + MakeCard.rule + "/" + MakeCard.sqlName;
            connection = new SqliteConnection("data source="+dbPath);
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            dbPath = MakeCard.androidsdcard + "/" + MakeCard.rule + "/" + MakeCard.sqlName;
            connection = new SqliteConnection("URI=file:" + dbPath);
        }
        connection.Open();
        command = connection.CreateCommand();
        //Debug.Log("数据库连接成功");
    }

    public SqliteDataReader ReadTable(string tableName, string NameorId)
    {
        if (NameorId.Equals("")) command.CommandText = "select * from " + tableName;
        else command.CommandText = "select * from " + tableName + " where name like '%" + NameorId + "%' or id='" + NameorId + "'";
        Debug.Log(command.CommandText);
        return command.ExecuteReader();
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
            command.CommandText += "'";
            command.CommandText += values[i];
            command.CommandText += "'";
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
        connection.Close();
        connection = null;
        //Debug.Log("已经断开数据库连接");
    }
}
