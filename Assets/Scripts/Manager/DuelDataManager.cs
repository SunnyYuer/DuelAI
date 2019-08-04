using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelDataManager
{
    public int duelPeopleNum;
    public int turnNum;
    public int whoTurn;//0自己回合，1对方回合
    public int duelPhase;
    public int opWhoOwn;//友方谁在操作
    public int opWhoOps;//敌方谁在操作
    public int player;//在操作的玩家
    public List<string>[] deck;
    public List<string>[] extra;
    public List<string>[] grave;
    public List<string>[] except;
    public List<string>[] handcard;
    public string[][] monster;
    public string[][] magictrap;
    public string[] field;
    public string[][] special;
    public CardDataManager cardData;

    /// <summary>
    /// 卡牌发动效果以及受效果影响的卡牌记录
    /// </summary>
    public class EffectRecord
    {
        public int turnNum;
        public int duelPhase;
        public int player;
        public string card;
        public int effect;
        public List<string> effectCard;
    }

    /// <summary>
    /// 可连锁的效果
    /// </summary>
    public class EffectPosition
    {
        public string card;
        /// <summary>
        /// 0  手卡
        /// 1  怪兽区域
        /// 2  魔法陷阱区域
        /// 3  场地卡区域
        /// 4  墓地
        /// 5  特殊区域
        /// </summary>
        public int position;
        public int index;
        public int effect;
    }

    public List<string>[] cardsJustDrawn;
    public List<EffectPosition> effectPosition;

    public DuelDataManager(int peopleNum)
    {
        duelPeopleNum = peopleNum;
        InitialDeck();
        cardData = new CardDataManager();
        turnNum = 0;
        opWhoOwn = 0;//0或2
        opWhoOps = 1;//1或3
    }

    public void InitialDeck()
    {
        deck = new List<string>[duelPeopleNum];
        extra = new List<string>[duelPeopleNum];
        grave = new List<string>[duelPeopleNum];
        except = new List<string>[duelPeopleNum];
        handcard = new List<string>[duelPeopleNum];
        monster = new string[duelPeopleNum][];
        magictrap = new string[duelPeopleNum][];
        cardsJustDrawn = new List<string>[duelPeopleNum];
        for (int i = 0; i < duelPeopleNum; i++)
        {
            deck[i] = new List<string>();
            extra[i] = new List<string>();
            grave[i] = new List<string>();
            except[i] = new List<string>();
            handcard[i] = new List<string>();
            monster[i] = new string[5];
            magictrap[i] = new string[5];
            cardsJustDrawn[i] = new List<string>();
        }
        field = new string[2];
    }

    public void LoadDeckData()
    {
        for (int i = 0; i < duelPeopleNum; i++)
        {
            cardData.LoadCardData(deck[i]);
            cardData.LoadCardData(extra[i]);
        }
        /*
        string allcode = "";
        foreach (CardDataManager.Card card in cardDataManager.cardDic.Values)
        {
            allcode += card.code;
        }
        */
    }
}
