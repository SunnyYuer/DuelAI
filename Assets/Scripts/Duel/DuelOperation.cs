using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对lua开放的API
/// </summary>
public class DuelOperation : MonoBehaviour
{
    private Duel duel;
    public DuelDataManager duelData;
    public DuelCard thiscard;//当前运行效果的卡
    public bool activatable;//卡牌能否发动

    // Start is called before the first frame update
    void Start()
    {
        duel = gameObject.GetComponent<Duel>();
        duelData = Duel.duelData;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 设置当前运行效果的卡的位置
    /// </summary>
    /// <param name="duelcard"></param>
    public void SetThisCard(DuelCard duelcard)
    {
        thiscard = duelcard;
        activatable = true;
    }

    /// <summary>
    /// 设置当前运行效果的卡的位置
    /// </summary>
    /// <param name="duelcard"></param>
    /// <param name="position"></param>
    /// <param name="index"></param>
    public void SetThisCard(DuelCard duelcard, int position, int index)
    {
        thiscard = duelcard;
        thiscard.position = position;
        thiscard.index = index;
        activatable = true;
    }

    /// <summary>
    /// 检查效果能否发动
    /// </summary>
    /// <param name="effectEvent"></param>
    public void SetEffectEvent(int effectEvent)
    {
        activatable = duel.EffectCheck(effectEvent);
    }

    /// <summary>
    /// 设置可发动的效果，之后可以进行发动
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="cost"></param>
    public void SetActivatableEffect(int effect, bool cost = false)
    {
        activatable = duel.PhaseCheck();
        activatable = true;
    }

    /// <summary>
    /// 设置可连锁的效果，之后可选择以进行发动
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="cost"></param>
    public void SetChainableEffect(int effect, bool cost = false)
    {
        if (activatable) duel.SetChainableEffect(thiscard, effect, cost);
        activatable = true;
    }

    /// <summary>
    /// 抽卡
    /// </summary>
    /// <param name="who"></param>
    /// <param name="num"></param>
    public void DrawCard(int who, int num)
    {
        EventData eData = new EventData
        {
            gameEvent = GameEvent.drawcard,
            data = new Dictionary<string, object>
            {
                { "player", who },
                { "drawnum", num }
            }
        };
        duelData.eventDate[duelData.player].Add(eData);
    }

    /// <summary>
    /// 判断是否把这张卡抽到
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public bool DrawnCard(string card)
    {
        //判断刚刚是否有抽卡
        int drawNum = duelData.cardsJustDrawn[duelData.player].Count;
        if (drawNum == 0) return false;
        if (card.Equals(""))
        {
            //判断是否在手卡
            if (thiscard.position != CardPosition.handcard) return false;
            //判断是否在抽到的卡中
            int drawIndex = duelData.handcard[duelData.player].Count - drawNum;
            if (thiscard.index < drawIndex) return false;
            else return true;
        }
        else
        {
            return duelData.cardsJustDrawn[duelData.player].Contains(card);
        }
    }

    /// <summary>
    /// 把这张卡给对方观看
    /// </summary>
    public void ShowCard(string card)
    {
        if (card.Equals(""))
        {
            card = thiscard.card;
        }
        Debug.Log("给对方观看卡牌  " + duelData.cardDic[card].name);
    }

    /// <summary>
    /// 把这张卡特殊召唤
    /// </summary>
    /// <param name="card"></param>
    public void SpecialSummon(string card)
    {
        if (card.Equals(""))
        {
            if (thiscard.position == CardPosition.handcard)
            {
                EventData eData = new EventData
                {
                    gameEvent = GameEvent.specialsummon,
                    data = new Dictionary<string, object>
                    {
                        { "selectcard", thiscard }
                    }
                };
                duelData.eventDate[duelData.player].Add(eData);
            }
        }
    }
}
