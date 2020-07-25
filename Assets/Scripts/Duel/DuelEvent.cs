using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对lua开放的API
/// </summary>
public class DuelEvent : MonoBehaviour
{
    private Duel duel;
    public DuelDataManager duelData;
    public DuelCard thiscard; // 当前卡
    public CardEffect cardEffect; // 当前检查的效果
    public bool precheck; // 发动效果和支付代价前预先检查能否执行
    private bool activatable; // 卡牌能否发动

    // Start is called before the first frame update
    void Start()
    {
        duel = gameObject.GetComponent<Duel>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 设置当前卡
    /// </summary>
    /// <param name="duelcard"></param>
    public void SetThisCard(DuelCard duelcard)
    {
        thiscard = duelcard;
        activatable = true;
    }

    /// <summary>
    /// 设置启动效果
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="cost"></param>
    public void SetStartupEffect(int effect, bool cost = false)
    {
        cardEffect = new CardEffect
        {
            duelcard = thiscard,
            effect = effect,
            effectType = EffectType.startup,
            speed = 1,
            cost = cost
        };
        if (!duel.ActivateCheck(cardEffect))
            activatable = false;
        if (activatable)
            duelData.activatableEffect.Add(cardEffect);
        activatable = true; // 一张卡可能有多个效果能发动
    }

    private void SetTriggerEffect(int effectType, int effect, bool cost)
    {
        int speed = 1;
        if (thiscard.type.Contains(CardType.monster) && thiscard.position == CardPosition.handcard && !thiscard.infopublic)
        { // 从手卡发动的怪兽的诱发效果，尽管咒文速度是1，实际处理时当作2速
            speed = 2;
        }
        cardEffect = new CardEffect
        {
            duelcard = thiscard,
            effect = effect,
            effectType = effectType,
            speed = speed,
            cost = cost
        };
        if (!duel.ActivateCheck(cardEffect))
            activatable = false;
        if (activatable)
            duelData.activatableEffect.Add(cardEffect);
        activatable = true; // 一张卡可能有多个效果能发动
    }

    /// <summary>
    /// 设置选发诱发效果，之后可以进行发动
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="cost"></param>
    public void SetCanTriggerEffect(int effect, bool cost = false)
    {
        SetTriggerEffect(EffectType.cantrigger, effect, cost);
    }

    /// <summary>
    /// 设置必发诱发效果，之后可以进行发动
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="cost"></param>
    public void SetMustTriggerEffect(int effect, bool cost = false)
    {
        SetTriggerEffect(EffectType.musttrigger, effect, cost);
    }

    /// <summary>
    /// 设置永续效果
    /// </summary>
    /// <param name="buff"></param>
    public void SetContinuousEffect(DuelBuff buff)
    {
        if (!duelData.duelbuff.Contains(buff))
        {
            duelData.duelbuff.Add(buff);
            duel.BuffEffect(buff);
        }
    }

    /// <summary>
    /// 创建buff
    /// </summary>
    /// <returns></returns>
    public DuelBuff CreateDuelBuff(int effect)
    {
        DuelBuff buff = duel.GetDuelBuff(thiscard, effect);
        if (buff != null) return buff;
        buff = new DuelBuff
        {
            fromcard = thiscard,
            effect = effect
        };
        return buff;
    }

    /// <summary>
    /// 抽卡
    /// </summary>
    /// <param name="num"></param>
    /// <param name="oplayer"></param>
    public void DrawCard(int num, int oplayer)
    {
        if (precheck) return;
        EventData eData = new EventData
        {
            oplayer = oplayer,
            gameEvent = GameEvent.drawcard,
            data = new Dictionary<string, object>
            {
                { "drawnum", num }
            }
        };
        duelData.eventDate.Add(eData);
    }

    public void DrawCard(int num)
    {
        DrawCard(num, duelData.opWho);
    }

    /// <summary>
    /// 丢弃卡
    /// </summary>
    /// <param name="cardlist"></param>
    public void DisCard(List<DuelCard> discardlist)
    {
        if (precheck) return;
        EventData eData = new EventData
        {
            oplayer = duelData.opWho,
            gameEvent = GameEvent.discard,
            data = new Dictionary<string, object>
            {
                { "discardlist", discardlist },
            }
        };
        duelData.eventDate.Add(eData);
    }

    /// <summary>
    /// 丢弃全部手卡
    /// </summary>
    /// <param name="oplayer"></param>
    /// <returns></returns>
    public int DisCardAll(int oplayer)
    {
        List<DuelCard> discardlist = new List<DuelCard>();
        discardlist.AddRange(duelData.handcard[oplayer]);
        if (precheck) return discardlist.Count;
        EventData eData = new EventData
        {
            oplayer = oplayer,
            gameEvent = GameEvent.discard,
            data = new Dictionary<string, object>
            {
                { "discardlist", discardlist },
            }
        };
        duelData.eventDate.Add(eData);
        return discardlist.Count;
    }

    /// <summary>
    /// 从目标卡选取n张进行操作
    /// </summary>
    /// <param name="targetcard"></param>
    /// <param name="num"></param>
    /// <param name="gameEvent"></param>
    public void SelectCard(TargetCard targetcard, int num, int gameEvent)
    {
        List<DuelCard> targetlist = duel.GetTargetCard(targetcard);
        if (precheck)
        {
            if (targetlist.Count == 0) activatable = false;
            if (gameEvent == GameEvent.specialsummon)
            {
                if (!duel.SpecialSummonCheck()) activatable = false;
            }
            return;
        }
        EventData eData = new EventData
        {
            oplayer = duelData.opWho,
            gameEvent = GameEvent.selectcard,
            data = new Dictionary<string, object>
            {
                { "targetlist", targetlist },
                { "num", num },
                { "gameEvent", gameEvent },
            }
        };
        duelData.eventDate.Add(eData);
    }

    /// <summary>
    /// 把这张卡给对方观看
    /// </summary>
    /// <param name="duelcard"></param>
    public void ShowCard(DuelCard duelcard)
    {
        if (precheck) return;
        EventData eData = new EventData
        {
            oplayer = duelData.opWho,
            gameEvent = GameEvent.showcard,
            data = new Dictionary<string, object>
            {
                { "showcard", duelcard },
            }
        };
        duelData.eventDate.Add(eData);
    }

    /// <summary>
    /// 手卡的一只怪兽通常召唤
    /// </summary>
    /// <param name="duelcard"></param>
    public void NormalSummon(DuelCard duelcard)
    {
        if (precheck) return;
        EventData eData = new EventData
        {
            oplayer = duelData.opWho,
            gameEvent = GameEvent.normalsummon,
            data = new Dictionary<string, object>
            {
                { "handcard", duelcard }
            }
        };
        duelData.eventDate.Add(eData);
    }

    /// <summary>
    /// 把这张卡特殊召唤
    /// </summary>
    /// <param name="duelcard"></param>
    public void SpecialSummon(DuelCard duelcard)
    {
        if (precheck)
        {
            if (!duel.SpecialSummonCheck()) activatable = false;
            if (duelcard.position == CardPosition.handcard && duelcard.Equals(thiscard) && cardEffect.effectType == EffectType.cantrigger)
            {
                duel.AddLimit(-1, cardEffect, LimitType.specialsummonself, 1);
                if (!duel.LimitCheck(cardEffect)) activatable = false;
            }
            return;
        }
        EventData eData = new EventData
        {
            oplayer = duelData.opWho,
            gameEvent = GameEvent.specialsummon,
            data = new Dictionary<string, object>
            {
                { "monstercard", duelcard },
            }
        };
        duelData.eventDate.Add(eData);
    }

    /// <summary>
    /// 把这些卡特殊召唤
    /// </summary>
    /// <param name="cardlist"></param>
    public void SpecialSummon(List<DuelCard> cardlist)
    {
        if (precheck) return;
        foreach (DuelCard duelcard in cardlist)
        {
            SpecialSummon(duelcard);
        }
    }

    /// <summary>
    /// 盖放魔法陷阱
    /// </summary>
    /// <param name="duelcard"></param>
    public void SetMagicTrap(DuelCard duelcard)
    {
        if (precheck) return;
        EventData eData = new EventData
        {
            oplayer = duelData.opWho,
            gameEvent = GameEvent.setmagictrap,
            data = new Dictionary<string, object>
            {
                { "magictrapcard", duelcard },
                { "mean", CardMean.facedownmgt },
            }
        };
        duelData.eventDate.Add(eData);
    }

    /// <summary>
    /// 怪兽变更表示形式
    /// </summary>
    /// <param name="duelcard"></param>
    /// <param name="mean"></param>
    public void ChangeMean(DuelCard duelcard, int mean = 0)
    {
        if (precheck) return;
        EventData eData = new EventData
        {
            oplayer = duelData.opWho,
            gameEvent = GameEvent.changemean,
            data = new Dictionary<string, object>
            {
                { "monstercard", duelcard },
                { "monstermean", mean }
            }
        };
        duelData.eventDate.Add(eData);
    }

    /// <summary>
    /// 场合的判断
    /// </summary>
    /// <param name="targetcard"></param>
    /// <param name="gameEvent"></param>
    /// <returns></returns>
    public bool InCase(object targetcard, int gameEvent)
    {
        foreach (DuelCase duelcase in duelData.duelcase)
        {
            if (duelcase.gameEvent == gameEvent)
            {
                DuelCard tcard = null;
                if (targetcard is DuelCard)
                {
                    tcard = targetcard as DuelCard;
                }
                else
                { 
                }
                switch (gameEvent)
                {
                    case GameEvent.battledestroy:
                        if (duelcase.card.Contains(tcard))
                            return true;
                        break;
                    default:
                        break;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 时点的判断
    /// </summary>
    /// <param name="targetcard"></param>
    /// <param name="gameEvent"></param>
    /// <returns></returns>
    public bool InTimePoint(object targetcard, int gameEvent)
    {
        foreach (DuelCase duelcase in duelData.duelcase)
        {
            if (duelcase.timepoint == 0 && duelcase.gameEvent == gameEvent)
            {
                DuelCard tcard = null;
                if (targetcard is DuelCard)
                {
                    tcard = targetcard as DuelCard;
                }
                else
                {
                }
                switch (gameEvent)
                {
                    case GameEvent.drawcard:
                        if (duelcase.card.Contains(tcard))
                            return true;
                        break;
                    default:
                        break;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 这张卡是否在与怪兽进行战斗
    /// </summary>
    /// <returns></returns>
    public bool ThisCardIsBattle()
    {
        List<DuelCard> duelcard = duel.GetLastBattleCard();
        if (duelcard.Count == 1) return false;
        return duelcard.Contains(thiscard);
    }

    /// <summary>
    /// 获取正在与之进行战斗的怪兽
    /// </summary>
    /// <returns></returns>
    public DuelCard GetAntiMonster()
    {
        List<DuelCard> duelcard = duel.GetLastBattleCard();
        if (duelcard[0].Equals(thiscard))
            return duelcard[1];
        else
            return duelcard[0];
    }

    /// <summary>
    /// 获取当前的玩家操作顺序
    /// </summary>
    /// <returns></returns>
    public List<int> GetPlayerOrder()
    {
        return duel.GetPlayerOrder();
    }
}
