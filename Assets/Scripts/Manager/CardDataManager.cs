using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataManager
{
    public class Card
    {
        public string id;
        public string name;
        public string type;//卡牌类型
        public string series;//卡牌系列

        public string attribute;//怪兽属性
        public int level;//怪兽星级
        public string race;//怪兽种族
        public int atk;
        public int def;

        public string describe;
        public string code;
    }

    public Dictionary<string, Card> cardDic;
    private SQLManager sql;

    public CardDataManager()
    {
        cardDic = new Dictionary<string, Card>();
    }

    public void LoadCardData(List<string> cardsid)
    {
        sql = new SQLManager();
        sql.ConnectSQL();
        foreach (string cardid in cardsid)
        {
            if (cardDic.ContainsKey(cardid)) continue;
            Card card = new Card();
            SqliteDataReader reader = sql.ReadCardsAll(Main.tableName, cardid);
            while (reader.Read())
            {
                card.id = reader["id"].ToString();
                card.name = reader["name"].ToString();
                card.type = reader["type"].ToString();
                card.series = reader["series"].ToString();
                string attris = reader["attribute"].ToString();
                if(!attris.Equals("")) GetCardAttributes(card, attris);
                card.describe = reader["describe"].ToString();
                card.code = reader["code"].ToString();
            }
            reader.Close();
            cardDic.Add(cardid, card);
        }
        sql.CloseSQLConnection();
    }

    public void GetCardAttributes(Card card, string attris)
    {
        string[] attributes = attris.Split(';');
        foreach (string attri in attributes)
        {
            string[] values = attri.Split('=');
            if (values[0].Equals("attribute")) card.attribute = values[1];
            if (values[0].Equals("level")) card.level = int.Parse(values[1]);
            if (values[0].Equals("race")) card.race = values[1];
            if (values[0].Equals("atk")) card.atk = int.Parse(values[1]);
            if (values[0].Equals("def")) card.def = int.Parse(values[1]);
        }
    }
}
