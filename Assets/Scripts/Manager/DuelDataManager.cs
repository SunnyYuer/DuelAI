using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelDataManager
{
    // 场上数据
    public int playerNum;
    public int areaNum; // 场上怪兽区域或魔法陷阱区域的数量
    public int turnNum;
    public int duelPhase;
    public int player; // 当前回合的玩家，0或2为友方，1或3为敌方
    public int opWho; // 在效果处理的玩家
    public CardDataManager cardData;
    public Dictionary<string, Card> cardDic;
    public List<DuelBuff> duelbuff;
    public List<EventData> eventDate;
    public List<DuelRecord> record;

    // 玩家数据
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
    public int[] normalsummon; // 这回合已通常召唤的次数
    public List<string>[] cardsJustDrawn;

    // 效果连锁
    public List<CardEffect> chainEffect;
    public List<CardEffect> waitEffect; // 不入连锁，等待连锁完后要发动的效果
    public bool effectChain; // 是否正在连锁

    // 临时保存
    public List<CardEffect> activatableEffect;
    public int placeSelect; // 选择卡牌放置的位置

    public DuelDataManager(int peopleNum)
    {
        playerNum = peopleNum;
        areaNum = 5;
        InitialArray();
        cardData = new CardDataManager();
        duelbuff = new List<DuelBuff>();
        eventDate = new List<EventData>();
        record = new List<DuelRecord>();
        chainEffect = new List<CardEffect>();
        waitEffect = new List<CardEffect>();
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
        normalsummon = new int[playerNum];
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
        Duel.cardDic = cardDic;
    }

    public void SortCard(List<DuelCard> cardlist)
    { 
        for (int i = 0; i < cardlist.Count; i++)
        {
            cardlist[i].index = i;
        }
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
public class DuelCard : Card
{
    public int owner; // 原本持有者
    public int controller; // 控制者
    public int position;
    public int index;
    public int mean; // 在场上的表示形式
    public int appearturn; // 出现在场上的回合
    // 回合值
    public int meanchange; // 表示形式变更次数
    public int battledeclare; // 战斗宣言

    public void SetCard(Card card)
    {
        id = card.id;
        name = card.name;
        type = card.type;
        series = card.series;
        attribute = card.attribute;
        level = card.level;
        race = card.race;
        atk = card.atk;
        def = card.def;
        describe = card.describe;
    }
}

public class CardLocation
{
    public int controller;
    public int position;
    public int index;

    public void SetLocation(DuelCard duelcard)
    {
        controller = duelcard.controller;
        position = duelcard.position;
        index = duelcard.index;
    }

    public DuelCard FindDuelCard(DuelDataManager duelData)
    {
        if (position == CardPosition.handcard)
        {
            return duelData.handcard[controller][index];
        }
        if (position == CardPosition.monster)
        {
            return duelData.monster[controller][index];
        }
        if (position == CardPosition.magictrap)
        {
            return duelData.magictrap[controller][index];
        }
        if (position == CardPosition.field)
        {
            return duelData.fieldcard[controller % 2];
        }
        return null;
    }
}

/// <summary>
/// 决斗时产生的增减益状态
/// </summary>
public class DuelBuff
{
    public DuelCard fromcard;
    public object targetcard;
    public int conturn; // buff持续回合数，0当前回合，n持续n回合，-1永续
    public int conphase; // buff持续到哪个阶段
    public int bufftype;
    public object buff;

    public void SetTargetCard(object targetcard)
    {
        this.targetcard = targetcard;
    }

    public void SetConTime(int conturn, int conphase)
    {
        this.conturn = conturn;
        this.conphase = conphase;
    }

    public void SetBuff(int bufftype, object buff)
    {
        this.bufftype = bufftype;
        this.buff = buff;
    }
}

public class TargetCard
{
    public int side;
    public List<int> position;
    public Dictionary<int, object> target;
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
/// 决斗行动记录
/// </summary>
public class DuelRecord
{
    public int action;
    public List<CardLocation> card;

    public DuelRecord(int action)
    {
        card = new List<CardLocation>();
        this.action = action;
    }

    public void AddCard(DuelCard duelcard)
    {
        if (duelcard == null) return;
        CardLocation cardlocal = new CardLocation();
        cardlocal.SetLocation(duelcard);
        card.Add(cardlocal);
    }
}
