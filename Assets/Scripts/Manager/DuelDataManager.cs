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
    public bool effectChain;//是否正在连锁
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
    public Dictionary<string, Card> cardDic;

    //临时保存
    public List<EventData>[] eventDate;
    public List<string>[] cardsJustDrawn;
    public List<CardEffect> chainableEffect;

    public DuelDataManager(int peopleNum)
    {
        duelPeopleNum = peopleNum;
        InitialDeck();
        cardData = new CardDataManager();
        chainableEffect = new List<CardEffect>();
        turnNum = 0;
        opWhoOwn = 0;//0或2
        opWhoOps = 1;//1或3
        effectChain = false;
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
        eventDate = new List<EventData>[duelPeopleNum];
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
            eventDate[i] = new List<EventData>();
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
        cardDic = cardData.cardDic;
    }

    public bool IsPlayerOwn()
    {
        if (player == 0 || player == 2)
            return true;
        else
            return false;
    }

    public void ChangePlayer()
    {
        if (player == opWhoOwn) player = opWhoOps;
        else player = opWhoOwn;
    }
}

/// <summary>
/// 游戏事件产生的数据
/// </summary>
public class EventData
{
    public int gameEvent;
    public int player;
    public int drawNum;
    public DuelCard selectcard;
}

/// <summary>
/// 记录卡牌的位置
/// </summary>
public class DuelCard
{
    public string card;
    public int position;
    public int index;
}

/// <summary>
/// 可发动的卡牌效果
/// </summary>
public class CardEffect : DuelCard
{
    public int effect;
    public bool cost;
}

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
