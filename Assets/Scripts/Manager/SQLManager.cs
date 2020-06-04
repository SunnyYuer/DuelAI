using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine.UI;

public class SQLManager {

    private SqliteConnection connection;
    private SqliteCommand command;
 
    public void ConnectSQL()
    {
        string dbPath = Main.rulePath + "/" + Main.sqlName;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
        connection = new SqliteConnection("data source=" + dbPath);
#elif UNITY_ANDROID
        connection = new SqliteConnection("URI=file:" + dbPath);
#endif
        connection.Open();
        command = connection.CreateCommand();
        //Debug.Log("数据库连接成功");
    }

    public SqliteDataReader ReadCardsAll(string tableName, string nameorid)
    {
        if (nameorid.Equals("")) command.CommandText = "select * from " + tableName;
        else command.CommandText = "select * from " + tableName + " where name like '%" + nameorid + "%' or id='" + nameorid + "'";
        //Debug.Log(command.CommandText);
        return command.ExecuteReader();
    }

    public SqliteDataReader ReadCardsAllLimit(string tableName, string nameorid, int limit, int offset)
    {
        if (nameorid.Equals("")) command.CommandText = "select * from " + tableName + " limit " + limit + " offset " + offset;
        else command.CommandText = "select * from " + tableName + " where name like '%" + nameorid + "%' or id='" + nameorid + "'"
                + " limit " + limit + " offset " + offset;
        //Debug.Log(command.CommandText);
        return command.ExecuteReader();
    }

    public SqliteDataReader ReadCardsId(string tableName, string Id)
    {
        if (Id.Equals("")) command.CommandText = "select id from " + tableName;
        else command.CommandText = "select id from " + tableName + " where id='" + Id + "'";
        Debug.Log(command.CommandText);
        return command.ExecuteReader();
    }

    public SqliteDataReader GetCardsCount(string tableName, string nameorid)
    {
        if (nameorid.Equals("")) command.CommandText = "select count(*) from " + tableName;
        else command.CommandText = "select count(*) from " + tableName + " where name like '%" + nameorid + "%' or id='" + nameorid + "'";
        //Debug.Log(command.CommandText);
        return command.ExecuteReader();
    }

    public SqliteDataReader InsertCard(string tableName, string[] fieldNames, object[] values)
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
        command.CommandText += ")" + " values(";
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

    public SqliteDataReader UpdateCard(string tableName, string[] fieldNames, object[] values, string id)
    {
        command.CommandText = "update " + tableName + " set ";
        for (int i = 0; i < fieldNames.Length; i++)
        {
            command.CommandText += fieldNames[i];
            command.CommandText += "='";
            command.CommandText += values[i];
            command.CommandText += "'";
            if (i < fieldNames.Length - 1)
            {
                command.CommandText += ", ";
            }
        }
        command.CommandText += " where id='" + id + "'";
        Debug.Log(command.CommandText);
        return command.ExecuteReader();
    }

    public void CloseSQLConnection()
    {
        command.Dispose();
        connection.Close();
        //Debug.Log("已经断开数据库连接");
    }
}
