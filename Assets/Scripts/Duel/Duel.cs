﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Duel : MonoBehaviour
{
    public GameObject mainLayout;
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

    private List<ChainableEffect> chainableEffect;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        duelOperate = gameObject.GetComponent<DuelOperation>();
        duelData = new DuelDataManager(2);
        luaCode = new LuaCode();
        spriteManager = new CardSpriteManager();
        ai = new AI();
        UIMask = GameObject.Find("DeckImageOwn").GetComponent<Image>().sprite;//保存UIMask
        monserOwn = GameObject.Find("MonsterAreaOwn").GetComponent<MonsterOwn>();
        monserOps = GameObject.Find("MonsterAreaOps").GetComponent<MonsterOps>();
        chainableEffect = new List<ChainableEffect>();
        //读取卡组
        ReadDeckFile();
        //加载卡组数据
        duelData.LoadDeckData();
        luaCode.SetCode(duelData.cardData.allcode);
        //放置卡组
        deckOwn.DeckUpdate();
        deckOps.DeckUpdate();
        //初始化回合和阶段
        duelData.whoTurn = 0;
        StartCoroutine(ChangePhase(0));
        //各自起手5张卡
        StartCoroutine(DrawCardOwn(5));
        StartCoroutine(DrawCardOps(5));
        //决斗开始
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnQuitClick()
    {
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

    public IEnumerator ChangePhase(int phase)
    {
        if (phase >= 7)
        {
            phase = 0;
            duelData.whoTurn = 1 - duelData.whoTurn;
        }
        duelData.duelPhase = phase;
        PhaseButtonShow();
        if (duelData.duelPhase == 0)
        {
            if (duelData.whoTurn == 0) phaseText.text = "我的回合";
            else phaseText.text = "对方回合";
            StartCoroutine(PhaseWait());
        }
        if (duelData.duelPhase == 1)
        {
            phaseText.text = "抽卡阶段";
            if (duelData.whoTurn == 0)
                yield return duelOperate.DrawCardOwn(1);
            else
                yield return duelOperate.DrawCardOps(1);
            StartCoroutine(PhaseWait());
        }
        if (duelData.duelPhase == 2)
        {
            phaseText.text = "准备阶段";
            StartCoroutine(PhaseWait());
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
        }
        if (duelData.duelPhase == 5)
        {
            phaseText.text = "主二阶段";
        }
        if (duelData.duelPhase == 6)
        {
            phaseText.text = "结束阶段";
            StartCoroutine(PhaseWait());
        }
    }

    public IEnumerator PhaseWait()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(ChangePhase(++duelData.duelPhase));
    }

    public void PhaseButtonShow()
    {
        if (duelData.duelPhase >= 3 && duelData.duelPhase <= 4) battleButton.SetActive(true);
        else battleButton.SetActive(false);
        if (duelData.duelPhase >= 3 && duelData.duelPhase <= 5) endTurnButton.SetActive(true);
        else endTurnButton.SetActive(false);
    }

    public void OnEndTurnButtonClick()
    {
        StartCoroutine(ChangePhase(6));
    }

    public void ChangeBattleButtonText()
    {
        Text buttonText = battleButton.GetComponentInChildren<Text>();
        if (duelData.duelPhase == 3) buttonText.text = "开始战斗";
        if (duelData.duelPhase == 4) buttonText.text = "结束战斗";
    }

    public void OnBattleButtonClick()
    {
        if (duelData.duelPhase == 4) StartCoroutine(ChangePhase(5));
        if (duelData.duelPhase == 3) StartCoroutine(ChangePhase(4));
    }

    public IEnumerator EffectChain(int player)
    {
        duelData.player = player;
        ScanEffect(player);
        SetCardOutLine();
        if (duelData.chainableEffect.Count > 0)
        {
            yield return WantChain();
            if (Tip.select == 1) SelectCardActivate();
            else CutCardOutLine();
        }
        duelData.cardsJustDrawn[player].Clear();
        duelData.chainableEffect.Clear();
    }

    public void ScanEffect(int player)
    {
        int i;
        for(i = 0; i < duelData.handcard[player].Count; i++)
        {
            duelOperate.SetCardLocation(CardPosition.handcard, i);
            luaCode.Run("c"+ duelData.handcard[player][i]);
            while (chainableEffect.Count > 0)
            {
                ChainableEffect cEffect = new ChainableEffect
                {
                    card = duelData.handcard[player][i],
                    effect = chainableEffect[0].effect,
                    cost = chainableEffect[0].cost,
                    position = CardPosition.handcard,
                    index = i
                };
                duelData.chainableEffect.Add(cEffect);
                chainableEffect.RemoveAt(0);
            }
        }
    }

    public void SetChainableEffect(int effect, bool cost)
    {
        ChainableEffect cEffect = new ChainableEffect
        {
            effect = effect,
            cost = cost
        };
        chainableEffect.Add(cEffect);
    }

    public void SetCardOutLine()
    {
        if (!duelData.IsPlayerOwn()) return;
        foreach(ChainableEffect cEffect in duelData.chainableEffect)
        {
            if (cEffect.position == CardPosition.handcard)
            {
                handOwn.SetOutLine(cEffect.index);
            }
        }
    }

    public void CutCardOutLine()
    {
        if (!duelData.IsPlayerOwn()) return;
        foreach (ChainableEffect cEffect in duelData.chainableEffect)
        {
            if (cEffect.position == CardPosition.handcard)
            {
                handOwn.CutOutLine(cEffect.index);
            }
        }
    }

    public IEnumerator WantChain()
    {
        //由玩家选择或者AI选择
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
    }

    public void SelectCardActivate()
    {
        //由玩家选择或者AI选择
        int select = 0;
        ChainableEffect cEffect = duelData.chainableEffect[select];
        duelOperate.SetCardLocation(cEffect.position, cEffect.index);
        if (cEffect.cost)
        {
            luaCode.Run(luaCode.CostFunStr(cEffect));
        }
        CutCardOutLine();
        luaCode.Run(luaCode.EffectFunStr(cEffect));
    }

    public IEnumerator DrawCardOwn(int num)
    {
        int player = duelData.opWhoOwn;
        while (num > 0)
        {
            yield return new WaitForSeconds(0.1f);
            handOwn.AddHandCardFromDeck();
            duelData.handcard[player].Add(duelData.deck[player][0]);
            duelData.cardsJustDrawn[player].Add(duelData.deck[player][0]);
            duelData.deck[player].RemoveAt(0);
            deckOwn.DeckUpdate();
            num--;
        }
    }

    public IEnumerator DrawCardOps(int num)
    {
        int player = duelData.opWhoOps;
        while (num > 0)
        {
            yield return new WaitForSeconds(0.1f);
            handOps.AddHandCardFromDeck();
            duelData.handcard[player].Add(duelData.deck[player][0]);
            duelData.cardsJustDrawn[player].Add(duelData.deck[player][0]);
            duelData.deck[player].RemoveAt(0);
            deckOps.DeckUpdate();
            num--;
        }
    }

    public List<int> GetMonsterPlace(int player)
    {
        List<int> place = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            if (string.IsNullOrEmpty(duelData.monster[player][i]))
            {
                place.Add(i);
            }
        }
        return place;
    }

    public void NormalSummonFromHandOwn(int index, int position)
    {
        handOwn.RemoveHandCard(index);
        monserOwn.ShowMonsterCard(index, position);
        duelData.monster[duelData.opWhoOwn][position] = duelData.handcard[duelData.opWhoOwn][index];
        duelData.handcard[duelData.opWhoOwn].RemoveAt(index);
    }

    public void NormalSummonFromHandOps(int index, int position)
    {
        handOps.RemoveHandCard(index);
        monserOps.ShowMonsterCard(index, position);
        duelData.monster[duelData.opWhoOps][position] = duelData.handcard[duelData.opWhoOps][index];
        duelData.handcard[duelData.opWhoOps].RemoveAt(index);
    }

    public void SpecialSummonFromHandOwn(int index)
    {
        //由玩家选择或者AI选择
        int position = monserOwn.MonsterPlace(GetMonsterPlace(duelData.opWhoOwn));
        handOwn.RemoveHandCard(index);
        monserOwn.ShowMonsterCard(index, position);
        duelData.monster[duelData.opWhoOwn][position] = duelData.handcard[duelData.opWhoOwn][index];
        duelData.handcard[duelData.opWhoOwn].RemoveAt(index);
    }

    public void SpecialSummonFromHandOps(int index)
    {
        //由玩家选择或者AI选择
        int position = 2;
        handOps.RemoveHandCard(index);
        monserOps.ShowMonsterCard(index, position);
        duelData.monster[duelData.opWhoOps][position] = duelData.handcard[duelData.opWhoOps][index];
        duelData.handcard[duelData.opWhoOps].RemoveAt(index);
    }
}
