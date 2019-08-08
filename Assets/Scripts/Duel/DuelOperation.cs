using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelOperation : MonoBehaviour
{
    private Duel duel;
    public DuelDataManager duelData;

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
    /// 设置可连锁的效果，之后可选择以进行发动
    /// </summary>
    public void SetChainableEffect(string card, int effect)
    {
        duel.SetChainableEffect(card, effect);
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
}
