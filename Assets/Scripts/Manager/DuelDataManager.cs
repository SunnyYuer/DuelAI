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
    public List<DuelCase> duelcase;
    public List<DuelBuff> duelbuff;
    public List<EventData> eventDate;
    public List<DuelRecord> record;

    // 玩家数据
    public int[] LP;
    public List<DuelCard>[] deck;
    public List<DuelCard>[] extra;
    public List<DuelCard>[] grave;
    public List<DuelCard>[] except;
    public List<DuelCard>[] handcard;
    public DuelCard[][] monster;
    public DuelCard[][] magictrap;
    public DuelCard[] fieldcard;
    public DuelCard[][] special;
    public int[] normalsummon; // 这回合已通常召唤的次数
    public List<Limit>[] limit;

    // 效果连锁
    public List<DuelEffect> chainEffect;
    public List<DuelEffect> waitEffect; // 不入连锁，等待连锁完后要发动的效果
    public bool effectChain; // 是否正在连锁

    // 临时保存
    public List<DuelEffect> activatableEffect;
    public int placeSelect; // 选择卡牌放置的位置

    public DuelDataManager(int peopleNum)
    {
        playerNum = peopleNum;
        areaNum = 5;
        InitialArray();
        cardData = new CardDataManager();
        duelcase = new List<DuelCase>();
        duelbuff = new List<DuelBuff>();
        eventDate = new List<EventData>();
        record = new List<DuelRecord>();
        chainEffect = new List<DuelEffect>();
        waitEffect = new List<DuelEffect>();
        activatableEffect = new List<DuelEffect>();
        turnNum = 0;
        effectChain = false;
    }

    public void InitialArray()
    {
        deck = new List<DuelCard>[playerNum];
        extra = new List<DuelCard>[playerNum];
        grave = new List<DuelCard>[playerNum];
        except = new List<DuelCard>[playerNum];
        handcard = new List<DuelCard>[playerNum];
        monster = new DuelCard[playerNum][];
        magictrap = new DuelCard[playerNum][];
        normalsummon = new int[playerNum];
        limit = new List<Limit>[playerNum];
        for (int i = 0; i < playerNum; i++)
        {
            deck[i] = new List<DuelCard>();
            extra[i] = new List<DuelCard>();
            grave[i] = new List<DuelCard>();
            except[i] = new List<DuelCard>();
            handcard[i] = new List<DuelCard>();
            monster[i] = new DuelCard[areaNum];
            magictrap[i] = new DuelCard[areaNum];
            limit[i] = new List<Limit>();
        }
        LP = new int[2];
        fieldcard = new DuelCard[2];
    }

    public void SortCard(List<DuelCard> cardlist)
    { 
        for (int i = 0; i < cardlist.Count; i++)
        {
            cardlist[i].index = i;
        }
    }

    public List<DuelCard> GetAllCards(int player)
    {
        List<DuelCard> allcards = new List<DuelCard>();
        allcards.AddRange(deck[player]);
        allcards.AddRange(extra[player]);
        allcards.AddRange(grave[player]);
        allcards.AddRange(except[player]);
        allcards.AddRange(handcard[player]);
        foreach (DuelCard duelcard in monster[player])
        {
            if (duelcard != null) allcards.Add(duelcard);
        }
        foreach (DuelCard duelcard in magictrap[player])
        {
            if (duelcard != null) allcards.Add(duelcard);
        }
        if (fieldcard[player] != null) allcards.Add(fieldcard[player]);
        return allcards;
    }
}

/// <summary>
/// 决斗时的卡牌
/// </summary>
public class DuelCard : Card
{
    public List<CardEffect> cardeffect;
    public int owner; // 原本持有者
    public int controller; // 控制者
    public int position;
    public int index;
    public int mean; // 在场上的表示形式
    public bool infopublic; // 情报是否已公开
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
        code = card.code;
    }

    public DuelCard Clone()
    {
        return MemberwiseClone() as DuelCard;
    }
}

/// <summary>
/// 卡牌上的效果
/// </summary>
public class CardEffect
{
    public int effect;
    public int effectType;
    public bool condition;
    public bool cost;
    public bool position; // 是否有位置判断，没有就是默认位置
    public int limitRange;
    public int limitType;
    public int limitCount;

    public CardEffect(int effect, int effectType)
    {
        this.effect = effect;
        this.effectType = effectType;
    }

    public void SetCondition()
    {
        condition = true;
    }

    public void SetCost()
    {
        cost = true;
    }

    public void SetPosition()
    {
        position = true;
    }
}

/// <summary>
/// 场合与时点
/// </summary>
public class DuelCase
{
    public int gameEvent;
    public int timepoint; // 0 当前时点 1 过去时点
    public int player;
    public List<DuelCard> card;
    public List<DuelCard> old;

    public DuelCase(int gameEvent)
    {
        card = new List<DuelCard>();
        old = new List<DuelCard>();
        this.gameEvent = gameEvent;
    }
}

/// <summary>
/// 决斗时产生的增减益状态
/// </summary>
public class DuelBuff
{
    public DuelCard fromcard;
    public int effect;
    public int conturn; // buff持续到n回合，-1永续
    public int conphase; // buff持续到哪个阶段
    public int bufftype;

    public void SetConTime(int conturn, int conphase)
    {
        this.conturn = conturn;
        this.conphase = conphase;
    }

    public void SetBuff(int bufftype)
    {
        this.bufftype = bufftype;
    }
}

public class TargetCard
{
    public int side;
    public List<int> position;
    public Dictionary<int, List<object>> target;

    public TargetCard()
    {
        position = new List<int>();
        target = new Dictionary<int, List<object>>();
    }

    public void SetSide(int side)
    {
        this.side = side;
    }

    public void SetPosition(int position)
    {
        this.position.Add(position);
    }

    public void SetTarget(int key, object value)
    {
        if (target.ContainsKey(key))
        {
            target[key].Add(value);
        }
        else
        {
            List<object> values = new List<object>();
            values.Add(value);
            target.Add(key, values);
        }
    }
}

/// <summary>
/// 游戏事件产生的数据
/// </summary>
public class EventData
{
    public int oplayer;//执行事件的玩家
    public int gameEvent;
    public Dictionary<string, object> data;
}

/// <summary>
/// 可发动的卡牌效果
/// </summary>
public class DuelEffect
{
    public DuelCard duelcard;
    public int effect;
    public int effectType;
    public int speed;
    public bool cost;
    public int limitType;
}

/// <summary>
/// 发动与使用限制
/// </summary>
public class Limit
{
    public int range;
    public DuelEffect cardEffect;
    public int type;
    public int max;
    public int count;
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
