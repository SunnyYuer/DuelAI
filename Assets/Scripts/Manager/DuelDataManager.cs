using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelDataManager
{
    public int playerNum;
    public int areaNum;//场上怪兽区域或魔法陷阱区域的数量
    public int turnNum;
    public int duelPhase;
    public int player;//当前回合的玩家，0或2为友方，1或3为敌方
    public int opWho;//在效果处理的玩家
    public int[] LP;
    public List<string>[] deck;
    public List<string>[] extra;
    public List<string>[] grave;
    public List<string>[] except;
    public List<DuelCard>[] handcard;
    public DuelCard[][] monster;
    public DuelCard[][] magictrap;
    public DuelCard[] fieldcard;
    public DuelCard[][] special;
    public CardDataManager cardData;
    public Dictionary<string, Card> cardDic;

    //效果连锁
    public List<CardEffect> chainEffect;
    public List<CardEffect> waitEffect;//不入连锁，等待连锁完后要发动的效果
    public bool effectChain;//是否正在连锁

    //临时保存
    public List<CardEffect> activatableEffect;
    public List<EventData> eventDate;
    public List<string>[] cardsJustDrawn;

    public DuelDataManager(int peopleNum)
    {
        playerNum = peopleNum;
        areaNum = 5;
        InitialArray();
        cardData = new CardDataManager();
        chainEffect = new List<CardEffect>();
        waitEffect = new List<CardEffect>();
        eventDate = new List<EventData>();
        activatableEffect = new List<CardEffect>();
        turnNum = 0;
        effectChain = false;
    }

    public void InitialArray()
    {
        deck = new List<string>[playerNum];
        extra = new List<string>[playerNum];
        grave = new List<string>[playerNum];
        except = new List<string>[playerNum];
        handcard = new List<DuelCard>[playerNum];
        monster = new DuelCard[playerNum][];
        magictrap = new DuelCard[playerNum][];
        cardsJustDrawn = new List<string>[playerNum];
        for (int i = 0; i < playerNum; i++)
        {
            deck[i] = new List<string>();
            extra[i] = new List<string>();
            grave[i] = new List<string>();
            except[i] = new List<string>();
            handcard[i] = new List<DuelCard>();
            monster[i] = new DuelCard[areaNum];
            magictrap[i] = new DuelCard[areaNum];
            cardsJustDrawn[i] = new List<string>();
        }
        LP = new int[2];
        fieldcard = new DuelCard[2];
    }

    public void LoadDeckData()
    {
        for (int i = 0; i < playerNum; i++)
        {
            cardData.LoadCardData(deck[i]);
            cardData.LoadCardData(extra[i]);
        }
        cardDic = cardData.cardDic;
    }

    public void ChangeNextPlayer()
    {
        player++;
        if (player == playerNum) player = 0;
        opWho = player;
    }
}

/// <summary>
/// 决斗时的卡牌
/// </summary>
public class DuelCard
{
    public string card;
    public int owner;//原本持有者
    public int controller;//控制者
    public int position;
    public int index;
    public int mean;//在场上的表示形式
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
    public int oplayer;//产生事件的玩家
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
    public int speed;
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
