﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Duel : MonoBehaviour
{
    public GameObject mainLayout;
    public Text LPOwn;
    public Text LPOps;
    public DeckOwn deckOwn;
    public DeckOps deckOps;
    public HandCardOwn handOwn;
    public HandCardOps handOps;
    private MonsterOwn monserOwn;
    private MonsterOps monserOps;
    public GameObject endTurnButton;
    public Text phaseText;
    public GameObject battleButton;
    public static CardSpriteManager spriteManager;
    public static Sprite UIMask;
    private DuelOperation duelOperate;
    public static DuelDataManager duelData;
    public LuaCode luaCode;
    public AI ai;

    private bool changePhase;
    private CardEffect activateEffect;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        duelData = new DuelDataManager(2);
        luaCode = new LuaCode();
        spriteManager = new CardSpriteManager();
        ai = new AI(duelData);
        duelOperate = gameObject.GetComponent<DuelOperation>();
        UIMask = GameObject.Find("DeckImageOwn").GetComponent<Image>().sprite;//保存UIMask
        monserOwn = GameObject.Find("MonsterAreaOwn").GetComponent<MonsterOwn>();
        monserOps = GameObject.Find("MonsterAreaOps").GetComponent<MonsterOps>();
        //读取卡组
        ReadDeckFile();
        //加载卡组数据
        duelData.LoadDeckData();
        luaCode.SetCode(duelData.cardData.allcode);
        //放置卡组
        deckOwn.DeckUpdate(0);
        deckOps.DeckUpdate(1);
        //初始化先攻，阶段，生命值
        duelData.player = 0;
        duelData.opWho = 0;
        duelData.duelPhase = 0;
        changePhase = true;
        LPOwnUpdate(8000);
        LPOpsUpdate(8000);
        //各自起手5张卡
        StartCoroutine(DrawCardOwn(0, 5));
        StartCoroutine(DrawCardOps(1, 5));
        yield return new WaitForSeconds(1);
        //决斗开始
        StartCoroutine(DuelPhase());
        StartCoroutine(Game());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnQuitClick()
    {
        StopAllCoroutines();
        luaCode.Close();
        Destroy(gameObject);
        Instantiate(mainLayout, GameObject.Find("Canvas").transform);
    }

    public void ReadDeckFile()
    {
        string deckpath = Main.rulePath + "/deck/mycard.ydk";
        string[] strs = File.ReadAllLines(deckpath);
        int i = 0;
        while (!strs[i].Equals("#main")) i++;
        i++;
        while (!strs[i].Equals("#extra"))
        {
            int rmindex = strs[i].IndexOf('#');
            if (rmindex >= 0) strs[i] = strs[i].Remove(rmindex);
            duelData.deck[0].Add(strs[i]);
            duelData.deck[1].Add(strs[i]);
            i++;
        }
        i++;
        while (!strs[i].Equals("!side"))
        {
            int rmindex = strs[i].IndexOf('#');
            if (rmindex >= 0) strs[i] = strs[i].Remove(rmindex);
            duelData.extra[0].Add(strs[i]);
            duelData.extra[1].Add(strs[i]);
            i++;
        }
    }

    public void LPOwnUpdate(int lp)
    {
        duelData.LP[0] = lp;
        LPOwn.text = "LP  " + lp;
    }

    public void LPOpsUpdate(int lp)
    {
        duelData.LP[1] = lp;
        LPOps.text = "LP  " + lp;
    }

    public bool IsPlayerOwn(int who)
    {
            return ai.IsPlayerOwn(who);
    }

    public int GetOppPlayer(int who)
    {
        return ai.GetOppPlayer(who);
    }

    private IEnumerator DuelPhase()
    {
        while (true)
        {
            if (changePhase)
            {
                changePhase = false;
                if (duelData.duelPhase == 0)
                {
                    duelData.turnNum++;
                    if (IsPlayerOwn(duelData.player)) phaseText.text = "我的回合";
                    else phaseText.text = "对方回合";
                    changePhase = true;
                }
                if (duelData.duelPhase == 1)
                {
                    phaseText.text = "抽卡阶段";
                    duelOperate.DrawCard(0, 1);
                }
                if (duelData.duelPhase == 2)
                {
                    phaseText.text = "准备阶段";
                    changePhase = true;
                }
                if (duelData.duelPhase == 3)
                {
                    phaseText.text = "主一阶段";
                    ChangeBattleButtonText();
                }
                if (duelData.duelPhase == 4)
                {
                    phaseText.text = "战斗阶段";
                    ChangeBattleButtonText();
                    yield return Battle();
                }
                if (duelData.duelPhase == 5)
                {
                    phaseText.text = "主二阶段";
                }
                if (duelData.duelPhase == 6)
                {
                    phaseText.text = "结束阶段";
                    changePhase = true;
                }
                PhaseButtonShow();
            }
            if (changePhase)
            {
                yield return new WaitForSeconds(1);
                duelData.duelPhase++;
                if (duelData.duelPhase >= 7)
                {
                    duelData.duelPhase = 0;
                    duelData.ChangeNextPlayer();
                }
            }
            yield return null;
        }
    }

    public void ChangePhase(int phase)
    {
        changePhase = true;
        duelData.duelPhase = phase;
    }

    public void PhaseButtonShow()
    {
        if (duelData.duelPhase >= 3 && duelData.duelPhase <= 4) battleButton.SetActive(true);
        else battleButton.SetActive(false);
        if (duelData.duelPhase >= 3 && duelData.duelPhase <= 5) endTurnButton.SetActive(true);
        else endTurnButton.SetActive(false);
    }

    private void OnEndTurnButtonClick()
    {
        ChangePhase(6);
    }

    private void ChangeBattleButtonText()
    {
        Text buttonText = battleButton.GetComponentInChildren<Text>();
        if (duelData.duelPhase == 3) buttonText.text = "开始战斗";
        if (duelData.duelPhase == 4) buttonText.text = "结束战斗";
    }

    private void OnBattleButtonClick()
    {
        if (duelData.duelPhase == 4) ChangePhase(5);
        if (duelData.duelPhase == 3) ChangePhase(4);
    }

    private IEnumerator Game()
    {
        int effectPhase = 0;
        while (true)
        {
            yield return null;
            int player = duelData.opWho;
            if (effectPhase > 0) ActivateEffect(activateEffect, ref effectPhase);
            List<EventData> eDataList = duelData.eventDate[player];
            if (eDataList.Count == 0) continue;
            EventData eData = eDataList[0];
            if (eData.gameEvent == GameEvent.drawcard)
            {
                int drawplayer = (int)eData.data["player"];
                int drawnum = (int)eData.data["drawnum"];
                if (drawplayer == 0 || drawplayer == 1)
                {//自己或对方抽卡
                    if (IsPlayerOwn(player)) yield return DrawCardOwn(player, drawnum);
                    else yield return DrawCardOps(player, drawnum);
                }
                if (drawplayer == 2)
                {//双方同时抽卡
                    if (IsPlayerOwn(player))
                    {
                        StartCoroutine(DrawCardOps(GetOppPlayer(player), drawnum));
                        yield return DrawCardOwn(player, drawnum);
                    }
                    else
                    {
                        StartCoroutine(DrawCardOwn(GetOppPlayer(player), drawnum));
                        yield return DrawCardOps(player, drawnum);
                    }
                }
                duelData.effectChain = true;
            }
            if (eData.gameEvent == GameEvent.specialsummon)
            {
                DuelCard selectcard = eData.data["selectcard"] as DuelCard;
                yield return SelectMonsterPlace();
                int mean = SelectMonsterMeans();
                if (selectcard.position == CardPosition.handcard)
                {
                    if (IsPlayerOwn(player))
                    {
                        SpecialSummonFromHandOwn(selectcard, MonsterOwn.placeSelect, mean);
                    }
                    else
                    {
                        SpecialSummonFromHandOps(selectcard, MonsterOps.placeSelect, mean);
                    }
                }
                duelData.effectChain = true;
            }
            if (duelData.effectChain)
            {
                yield return EffectChain();
                if (activateEffect != null)
                {
                    if (activateEffect.cost) effectPhase = 1;
                    else effectPhase = 2;
                }
                else
                {
                    duelData.effectChain = false;
                }
            }
            if (!duelData.effectChain)
            {
                if (duelData.duelPhase == 1)
                {
                    ChangePhase(2);
                }
            }
            eDataList.RemoveAt(0);
        }
    }

    public void ActivateEffect(CardEffect cardEffect, ref int phase)
    {
        duelOperate.SetThisCard(cardEffect.duelcard);
        if (phase == 1)
        {//发动阶段1，支付发动代价
            luaCode.Run(luaCode.CostFunStr(cardEffect));
        }
        if (phase == 2)
        {//发动阶段2，发动效果
            luaCode.Run(luaCode.EffectFunStr(cardEffect));
            cardEffect = null;
        }
        if (phase == 1) phase++;
        else phase = 0;
    }

    public IEnumerator EffectChain()
    {
        ScanEffect();
        SetCardOutLine();
        activateEffect = null;
        if (duelData.chainableEffect.Count > 0)
        {
            yield return WantChain();
            if (Tip.select == 1)
            {//由玩家选择或者AI选择
                int select = 0;
                activateEffect = duelData.chainableEffect[select];
            }
            CutCardOutLine();
        }
    }

    public void ScanEffect()
    {
        int player = duelData.opWho;
        int i;
        for(i = 0; i < duelData.handcard[player].Count; i++)
        {
            duelOperate.SetThisCard(duelData.handcard[player][i], CardPosition.handcard, i);
            luaCode.Run("c"+ duelData.handcard[player][i].card);
        }
    }

    public bool EffectCheck(int gameEvent)
    {//检查效果能否发动
        if (gameEvent == GameEvent.specialsummon)
        {
            List<int> place = GetMonsterPlace();
            if (place.Count == 0) return false;
        }
        return true;
    }

    public bool PhaseCheck()
    {
        if (duelData.effectChain) return false;
        return true;
    }

    public void SetChainableEffect(DuelCard duelcard, int effect, bool cost)
    {
        CardEffect cardEffect = new CardEffect
        {
            duelcard = duelcard,
            effect = effect,
            cost = cost
        };
        duelData.chainableEffect.Add(cardEffect);
    }

    public void SetCardOutLine()
    {
        if (!IsPlayerOwn(duelData.opWho)) return;
        foreach (CardEffect cardEffect in duelData.chainableEffect)
        {
            if (cardEffect.duelcard.position == CardPosition.handcard)
            {
                handOwn.SetOutLine(cardEffect.duelcard.index);
            }
        }
    }

    public void CutCardOutLine()
    {
        if (!IsPlayerOwn(duelData.opWho))
        {
            duelData.chainableEffect.Clear();
            return;
        }
        foreach (CardEffect cardEffect in duelData.chainableEffect)
        {
            if (cardEffect.duelcard.position == CardPosition.handcard)
            {
                handOwn.CutOutLine(cardEffect.duelcard.index);
            }
        }
        duelData.chainableEffect.Clear();
    }

    private IEnumerator WantChain()
    {
        //由玩家选择或者AI选择
        /*
        if (duelData.IsPlayerOwn())
        {
            Tip.content = "是否连锁？";
            GameObject tipObject = Instantiate(Resources.Load("Prefabs/TipBackground"), transform) as GameObject;
            Tip tip = tipObject.GetComponent<Tip>();
            yield return tip.WaitForSelect();
        }
        else
        {
            Tip.select = 1;
        }
        */
        Tip.select = 1;
        yield return null;
    }

    private IEnumerator Battle()
    {
        int player = duelData.player;//攻击方
        int playerOpp = GetOppPlayer(player);//被攻击方
        int target = ai.GetAttackTarget();
        for (int i = 0; i < duelData.areaNum; i++)
        {
            if (duelData.monster[player][i] != null)
            {
                if (target == -1)
                {//直接攻击对方
                    int atk = duelData.cardDic[duelData.monster[player][i].card].atk;
                    if (IsPlayerOwn(player)) LPOpsUpdate(duelData.LP[1] - atk);
                    else LPOwnUpdate(duelData.LP[0] - atk);
                }
                else
                {//攻击选定的目标
                    int atk1 = duelData.cardDic[duelData.monster[player][i].card].atk;
                    int atk2 = duelData.cardDic[duelData.monster[playerOpp][target].card].atk;
                    if (atk1 > atk2)
                    {
                        if (IsPlayerOwn(player)) LPOpsUpdate(duelData.LP[1] - (atk1 - atk2));
                        else LPOwnUpdate(duelData.LP[0] - (atk1 - atk2));
                    }
                    if (atk1 == atk2)
                    {
                        
                    }
                    if (atk1 < atk2)
                    {
                        if (IsPlayerOwn(player)) LPOwnUpdate(duelData.LP[0] - (atk2 - atk1));
                        else LPOpsUpdate(duelData.LP[1] - (atk2 - atk1));
                    }
                }
            }
        }
        yield return null;
    }

    private IEnumerator DrawCardOwn(int playerOwn, int num)
    {
        duelData.cardsJustDrawn[playerOwn].Clear();
        while (num > 0)
        {
            yield return new WaitForSeconds(0.1f);
            handOwn.AddHandCardFromDeck(playerOwn);
            DuelCard duelcard = new DuelCard
            {
                card = duelData.deck[playerOwn][0],
                buffList = new List<DuelBuff>()
            };
            duelData.handcard[playerOwn].Add(duelcard);
            duelData.cardsJustDrawn[playerOwn].Add(duelData.deck[playerOwn][0]);
            duelData.deck[playerOwn].RemoveAt(0);
            deckOwn.DeckUpdate(playerOwn);
            num--;
        }
    }

    private IEnumerator DrawCardOps(int playerOps, int num)
    {
        duelData.cardsJustDrawn[playerOps].Clear();
        while (num > 0)
        {
            yield return new WaitForSeconds(0.1f);
            handOps.AddHandCardFromDeck(playerOps);
            DuelCard duelcard = new DuelCard
            {
                card = duelData.deck[playerOps][0],
                buffList = new List<DuelBuff>()
            };
            duelData.handcard[playerOps].Add(duelcard);
            duelData.cardsJustDrawn[playerOps].Add(duelData.deck[playerOps][0]);
            duelData.deck[playerOps].RemoveAt(0);
            deckOps.DeckUpdate(playerOps);
            num--;
        }
    }

    public List<int> GetMonsterPlace()
    {
        int player = duelData.opWho;
        List<int> place = new List<int>();
        for (int i = 0; i < duelData.areaNum; i++)
        {
            if (duelData.monster[player][i] == null)
            {
                place.Add(i);
            }
        }
        return place;
    }

    private IEnumerator SelectMonsterPlace()
    {
        List<int> place = GetMonsterPlace();
        //由玩家选择或者AI选择
        int select = 0;
        if (IsPlayerOwn(duelData.opWho))
        {
            //yield return monserOwn.MonsterPlace(place);
            MonsterOwn.placeSelect = place[select];
        }
        else
        {
            MonsterOps.placeSelect = place[select];
        }
        yield return null;
    }

    private int SelectMonsterMeans()
    {//特殊召唤时的怪物表示选择
        //由玩家选择或者AI选择
        return 2;
    }

    public void NormalSummonFromHandOwn(DuelCard duelcard, int position, int mean)
    {
        handOwn.RemoveHandCard(duelcard.index);
        monserOwn.ShowMonsterCard(duelcard, position, mean);
        duelData.monster[duelData.opWho][position] = duelcard;
        duelData.handcard[duelData.opWho].RemoveAt(duelcard.index);
    }

    public void NormalSummonFromHandOps(DuelCard duelcard, int position, int mean)
    {
        handOps.RemoveHandCard(duelcard.index);
        monserOps.ShowMonsterCard(duelcard, position, mean);
        duelData.monster[duelData.opWho][position] = duelcard;
        duelData.handcard[duelData.opWho].RemoveAt(duelcard.index);
    }

    private void SpecialSummonFromHandOwn(DuelCard duelcard, int position, int mean)
    {
        handOwn.RemoveHandCard(duelcard.index);
        monserOwn.ShowMonsterCard(duelcard, position, mean);
        duelData.monster[duelData.opWho][position] = duelcard;
        duelData.handcard[duelData.opWho].RemoveAt(duelcard.index);
    }

    private void SpecialSummonFromHandOps(DuelCard duelcard, int position, int mean)
    {
        handOps.RemoveHandCard(duelcard.index);
        monserOps.ShowMonsterCard(duelcard, position, mean);
        duelData.monster[duelData.opWho][position] = duelcard;
        duelData.handcard[duelData.opWho].RemoveAt(duelcard.index);
    }
}
