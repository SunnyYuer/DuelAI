﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelDataManager
{
    public int duelPeopleNum;
    public int turnNum;
    public int duelPhase;
    public int player;//当前回合的玩家，0或2为友方，1或3为敌方
    public int opWho;//在效果处理的玩家
    public bool effectChain;//是否正在连锁
    public List<string>[] deck;
    public List<string>[] extra;
    public List<string>[] grave;
    public List<string>[] except;
    public List<DuelCard>[] handcard;
    public DuelCard[][] monster;
    public DuelCard[][] magictrap;
    public DuelCard[] field;
    public DuelCard[][] special;
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
        effectChain = false;
    }

    public void InitialDeck()
    {
        deck = new List<string>[duelPeopleNum];
        extra = new List<string>[duelPeopleNum];
        grave = new List<string>[duelPeopleNum];
        except = new List<string>[duelPeopleNum];
        handcard = new List<DuelCard>[duelPeopleNum];
        monster = new DuelCard[duelPeopleNum][];
        magictrap = new DuelCard[duelPeopleNum][];
        eventDate = new List<EventData>[duelPeopleNum];
        cardsJustDrawn = new List<string>[duelPeopleNum];
        for (int i = 0; i < duelPeopleNum; i++)
        {
            deck[i] = new List<string>();
            extra[i] = new List<string>();
            grave[i] = new List<string>();
            except[i] = new List<string>();
            handcard[i] = new List<DuelCard>();
            monster[i] = new DuelCard[5];
            magictrap[i] = new DuelCard[5];
            eventDate[i] = new List<EventData>();
            cardsJustDrawn[i] = new List<string>();
        }
        field = new DuelCard[2];
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

    public void ChangeNextPlayer()
    {
        player++;
        if (player == duelPeopleNum) player = 0;
        opWho = player;
    }
}

/// <summary>
/// 决斗时的卡牌
/// </summary>
public class DuelCard
{
    public string card;
    public int position;
    public int index;
    public List<DuelBuff> buffList;
}

/// <summary>
/// 决斗时产生的增减益状态
/// </summary>
public class DuelBuff
{
    public DuelCard fromcard;//buff来源
    public Dictionary<int, object> buff;
    public int turns;//buff持续回合类型
}

/// <summary>
/// 游戏事件产生的数据
/// </summary>
public class EventData
{
    public int gameEvent;
    public Dictionary<string, object> data;
}

/// <summary>
/// 可发动的卡牌效果
/// </summary>
public class CardEffect
{
    public DuelCard duelcard;
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
