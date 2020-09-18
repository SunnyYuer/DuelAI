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
    public CardEffect thiseffect; // 当前检查的效果
    public bool precheck; // 发动效果和支付代价前预先检查能否执行
    private bool activatable; // 卡牌能否发动

    public void Initialize(Duel duel)
    {
        this.duel = duel;
        duelData = Duel.duelData;
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
    /// 设置当前效果
    /// </summary>
    /// <param name="effect"></param>
    public void SetThisEffect(int effect)
    {
        thiseffect = thiscard.cardeffect[effect - 1];
    }

    /// <summary>
    /// 创建卡牌效果
    /// </summary>
    /// <param name="effectType"></param>
    /// <returns></returns>
    public CardEffect CreateEffect(int effectType)
    {
        CardEffect cardEffect = new CardEffect
        {
            duelcard = thiscard,
            effect = thiscard.cardeffect.Count + 1,
            effectType = effectType,
            limit = new List<EffectLimit>(),
        };
        cardEffect.speed = duel.GetCardSpeed(cardEffect);
        thiscard.cardeffect.Add(cardEffect);
        return cardEffect;
    }

    /// <summary>
    /// 设置决斗时的卡牌效果
    /// </summary>
    public void SetDuelEffect()
    {
        if (activatable)
        {
            if (thiseffect.effectType < EffectType.activate)
            {
                if (duel.ChainCheck(thiseffect))
                    duelData.activatableEffect.Add(thiseffect);
            }
            if (thiseffect.effectType == EffectType.continuous)
            {
                duelData.immediateEffect.Add(thiseffect);
                if (thiseffect.contime != null)
                {
                    if (thiseffect.contime.turn >= 0)
                        thiseffect.contime.toturn = thiseffect.contime.turn + duelData.turnNum;
                    else
                        thiseffect.contime.toturn = 999;
                    duelData.buffeffect.Add(thiseffect);
                }
            }
        }
        activatable = true; // 一张卡可能有多个效果能发动
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
            if (duelcard.position == CardPosition.handcard && duelcard.Equals(thiscard) && thiseffect.effectType == EffectType.cantrigger)
            {
                thiseffect.SetLimit(-1, LimitType.specialsummonself, 1);
                if (!duel.LimitCheck(thiscard, thiseffect.effect, LimitType.specialsummonself)) activatable = false;
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
    /// 那之后，时点过时
    /// </summary>
    public void AfterThat()
    {
        if (precheck) return;
        EventData eData = new EventData
        {
            oplayer = duelData.opWho,
            gameEvent = GameEvent.afterthat,
        };
        duelData.eventDate.Add(eData);
    }

    /// <summary>
    /// 攻击力变更
    /// </summary>
    /// <param name="atk"></param>
    public void AttackNew(int atk)
    {
        if (precheck) return;
        thiscard.atk = atk;
    }

    /// <summary>
    /// 那个发动无效
    /// </summary>
    public DuelCard ActivateInvalid()
    {
        if (precheck) return duelData.chainEffect[0].duelcard;
        EventData eData = new EventData
        {
            oplayer = duelData.opWho,
            gameEvent = GameEvent.activateinvalid,
        };
        duelData.eventDate.Add(eData);
        return duelData.chainEffect[1].duelcard;
    }

    /// <summary>
    /// 破坏卡
    /// </summary>
    /// <param name="duelcard"></param>
    public void DestroyCard(DuelCard duelcard)
    {
        if (precheck) return;
        EventData eData = new EventData
        {
            oplayer = duelData.opWho,
            gameEvent = GameEvent.effectdestroy,
            data = new Dictionary<string, object>
            {
                { "card", duelcard },
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
        List<DuelCard> tcards = new List<DuelCard>();
        if (targetcard is DuelCard)
            tcards.Add(targetcard as DuelCard);
        else
            tcards.AddRange(duel.GetTargetCard(targetcard as TargetCard));
        foreach (DuelCase duelcase in duelData.duelcase)
        {
            if (duelcase.gameEvent == gameEvent)
            {
                switch (gameEvent)
                {
                    case GameEvent.battledestroy:
                        foreach (DuelCard duelcard in duelcase.card)
                        {
                            if (tcards.Contains(duelcard)) return true;
                        }
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
        List<DuelCard> tcards = new List<DuelCard>();
        if (targetcard is DuelCard)
            tcards.Add(targetcard as DuelCard);
        else
            tcards.AddRange(duel.GetTargetCard(targetcard as TargetCard));
        foreach (DuelCase duelcase in duelData.duelcase)
        {
            if (duelcase.timepoint == 0 && duelcase.gameEvent == gameEvent)
            {
                switch (gameEvent)
                {
                    case GameEvent.drawcard:
                    case GameEvent.activatecard:
                        foreach (DuelCard duelcard in duelcase.card)
                        {
                            if (tcards.Contains(duelcard)) return true;
                        }
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
