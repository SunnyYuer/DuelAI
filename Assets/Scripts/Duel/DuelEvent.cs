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
        if (!duel.ActivateCheck(thiscard, effect, EffectType.startup))
            activatable = false;
        if (activatable)
            duel.SetActivatableEffect(thiscard, effect, 1, cost);
        activatable = true; // 一张卡可能有多个效果能发动
    }

    /// <summary>
    /// 设置诱发效果，之后可以进行发动
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="cost"></param>
    public void SetTriggerEffect(int effect, bool cost = false)
    {
        if (!duel.ActivateCheck(thiscard, effect, EffectType.trigger))
            activatable = false;
        if (activatable)
            duel.SetActivatableEffect(thiscard, effect, 1, cost);
        activatable = true; // 一张卡可能有多个效果能发动
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
    /// <param name="side"></param>
    public void DrawCard(int num, int side = PlayerSide.own)
    {
        int oplayer = duel.GetSidePlayer(side);
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
    /// <param name="side"></param>
    /// <returns></returns>
    public int DisCardAll(int side = PlayerSide.own)
    {
        int oplayer = duel.GetSidePlayer(side);
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
        Debug.Log("给对方观看卡牌  " + duelcard.name);
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
        int count = duelData.duelcase.Count;
        if (count == 0) return false;
        DuelCase duelcase = duelData.duelcase[count - 1];
        if (duelcase.gameEvent != gameEvent) return false;
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
}
