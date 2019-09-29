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
    //当前运行效果的卡
    public int cardPosition;
    public int cardIndex;

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
    /// <param name="position"></param>
    /// <param name="index"></param>
    public void SetCardLocation(int position, int index)
    {
        cardPosition = position;
        cardIndex = index;
    }

    /// <summary>
    /// 设置可连锁的效果，之后可选择以进行发动
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="cost"></param>
    public void SetChainableEffect(int effect, bool cost = false)
    {
        duel.SetChainableEffect(effect, cost);
    }

    /// <summary>
    /// 自己抽卡
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public IEnumerator DrawCardOwn(int num)
    {
        yield return StartCoroutine(duel.DrawCardOwn(num));
        duel.EffectChain(duelData.opWhoOwn);
    }

    /// <summary>
    /// 对方抽卡
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public IEnumerator DrawCardOps(int num)
    {
        yield return StartCoroutine(duel.DrawCardOps(num));
        duel.EffectChain(duelData.opWhoOps);
    }

    /// <summary>
    /// 判断是否把这张卡抽到
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public bool DrawThisCard(string card)
    {
        return duelData.cardsJustDrawn[duelData.player].Contains(card);
    }

    /// <summary>
    /// 把这张卡给对方观看
    /// </summary>
    public void ShowCard(string card)
    {
        if (card.Equals(""))
        {
            card = duelData.handcard[duelData.player][cardIndex];
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
            if (cardPosition == CardPosition.handcard)
            {
                duel.SpecialSummonFromHandOwn(cardIndex);
            }
        }
    }
}
