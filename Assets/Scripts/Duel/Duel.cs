using System.Collections;
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
    public GraveOwn graveOwn;
    public GraveOps graveOps;
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
        LPOwnUpdate(8000);
        LPOpsUpdate(8000);
        //各自起手5张卡
        StartCoroutine(DrawCardOwn(0, 5));
        StartCoroutine(DrawCardOps(1, 5));
        yield return new WaitForSeconds(1);
        //决斗开始
        StartCoroutine(DuelPhase(duelData.duelPhase));
        StartCoroutine(Game());
    }

    // Update is called once per frame
    void Update()
    {
        PhaseButtonShow();
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

    private IEnumerator DuelPhase(int phase)
    {
        duelData.duelPhase = phase;
        switch (duelData.duelPhase)
        {
            case 0:
                duelData.turnNum++;
                if (IsPlayerOwn(duelData.player)) phaseText.text = "我的回合";
                else phaseText.text = "对方回合";
                StartCoroutine(EndPhase());
                break;
            case 1:
                phaseText.text = "抽卡阶段";
                duelOperate.DrawCard(0, 1);
                yield return WaitGameEvent();
                yield return EffectChain();
                StartCoroutine(EndPhase());
                break;
            case 2:
                phaseText.text = "准备阶段";
                StartCoroutine(EndPhase());
                break;
            case 3:
                phaseText.text = "主一阶段";
                ChangeBattleButtonText();
                break;
            case 4:
                phaseText.text = "战斗阶段";
                ChangeBattleButtonText();
                yield return Battle();
                break;
            case 5:
                phaseText.text = "主二阶段";
                break;
            case 6:
                phaseText.text = "结束阶段";
                StartCoroutine(EndPhase());
                break;
            default:
                break;
        }
    }

    public IEnumerator EndPhase()
    {
        yield return new WaitForSeconds(1);
        duelData.duelPhase++;
        if (duelData.duelPhase >= 7)
        {
            duelData.duelPhase = 0;
            duelData.ChangeNextPlayer();
        }
        StartCoroutine(DuelPhase(duelData.duelPhase));
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
        StartCoroutine(DuelPhase(6));
    }

    private void ChangeBattleButtonText()
    {
        Text buttonText = battleButton.GetComponentInChildren<Text>();
        if (duelData.duelPhase == 3) buttonText.text = "开始战斗";
        if (duelData.duelPhase == 4) buttonText.text = "结束战斗";
    }

    private void OnBattleButtonClick()
    {
        if (duelData.duelPhase == 4) StartCoroutine(DuelPhase(5));
        if (duelData.duelPhase == 3) StartCoroutine(DuelPhase(4));
    }

    private IEnumerator Game()
    {
        while (true)
        {
            yield return null;
            if (duelData.eventDate.Count == 0) continue;
            EventData eData = duelData.eventDate[0];
            duelData.opWho = eData.oplayer;
            int player = eData.oplayer;
            switch (eData.gameEvent)
            {
                case GameEvent.drawcard:
                    int drawplayer = (int)eData.data["drawplayer"];
                    int drawnum = (int)eData.data["drawnum"];
                    if (drawplayer == 0)
                    {//自己抽卡
                        if (IsPlayerOwn(player)) yield return DrawCardOwn(player, drawnum);
                        else yield return DrawCardOps(player, drawnum);
                    }
                    if (drawplayer == 1)
                    {//对方抽卡
                        if (IsPlayerOwn(player)) yield return DrawCardOps(GetOppPlayer(player), drawnum);
                        else yield return DrawCardOwn(GetOppPlayer(player), drawnum);
                    }
                    if (drawplayer == 2)
                    {//双方同时抽卡
                     //同时行动时，先处理回合玩家
                        player = duelData.player;
                        if (IsPlayerOwn(player))
                        {
                            yield return DrawCardOwn(player, drawnum);
                            yield return DrawCardOps(GetOppPlayer(player), drawnum);
                        }
                        else
                        {
                            yield return DrawCardOps(player, drawnum);
                            yield return DrawCardOwn(GetOppPlayer(player), drawnum);
                        }
                    }
                    break;
                case GameEvent.specialsummon:
                    DuelCard monstercard = eData.data["monstercard"] as DuelCard;
                    yield return SelectMonsterPlace();
                    int mean = SelectMonsterMeans();
                    if (monstercard.position == CardPosition.handcard)
                    {
                        if (IsPlayerOwn(player))
                        {
                            SpecialSummonFromHandOwn(monstercard, MonsterOwn.placeSelect, mean);
                        }
                        else
                        {
                            SpecialSummonFromHandOps(monstercard, MonsterOps.placeSelect, mean);
                        }
                    }
                    break;
                default:
                    break;
            }
            duelData.eventDate.RemoveAt(0);
        }
    }

    public IEnumerator WaitGameEvent()
    {
        while (duelData.eventDate.Count > 0)
        {
            yield return null;
        }
    }

    public IEnumerator PayCost(CardEffect cardEffect)
    {
        duelOperate.SetThisCard(cardEffect.duelcard);
        luaCode.Run(luaCode.CostFunStr(cardEffect));
        yield return WaitGameEvent();
    }

    public IEnumerator ActivateEffect(CardEffect cardEffect)
    {
        duelOperate.SetThisCard(cardEffect.duelcard);
        luaCode.Run(luaCode.EffectFunStr(cardEffect));
        yield return WaitGameEvent();
    }

    public IEnumerator EffectChain()
    {
        bool chain = true;
        while (chain)
        {
            chain = false;
            ScanEffect();
            SetCardOutLine();
            if (duelData.activatableEffect.Count > 0)
            {
                CardEffect activateEffect = null;
                yield return WantActivate();
                if (Tip.select == 1)
                {//由玩家选择或者AI选择
                    int select = 0;
                    activateEffect = duelData.activatableEffect[select];
                    chain = true;
                }
                CutCardOutLine();
                if (chain)
                {
                    int opWho = duelData.opWho;
                    yield return PayCost(activateEffect);
                    duelData.chainEffect.Add(activateEffect);
                    duelData.opWho = GetOppPlayer(opWho);
                }
            }
        }
        while (duelData.chainEffect.Count > 0)
        {
            duelData.opWho = duelData.chainEffect[0].duelcard.controller;
            yield return ActivateEffect(duelData.chainEffect[0]);
            duelData.chainEffect.RemoveAt(0);
        }
    }

    public void ScanEffect()
    {
        int player = duelData.opWho;
        Debug.Log("扫描效果 player="+player);
        int i;
        for(i = 0; i < duelData.handcard[player].Count; i++)
        {
            duelOperate.SetThisCard(duelData.handcard[player][i]);
            luaCode.Run("c"+ duelData.handcard[player][i].card);
        }
    }

    public void SetActivatableEffect(DuelCard duelcard, int effect, int speed, bool cost)
    {
        CardEffect cardEffect = new CardEffect
        {
            duelcard = duelcard,
            effect = effect,
            speed = speed,
            cost = cost
        };
        duelData.activatableEffect.Add(cardEffect);
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

    public void SetCardOutLine()
    {
        if (!IsPlayerOwn(duelData.opWho)) return;
        foreach (CardEffect cardEffect in duelData.activatableEffect)
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
            duelData.activatableEffect.Clear();
            return;
        }
        foreach (CardEffect cardEffect in duelData.activatableEffect)
        {
            if (cardEffect.duelcard.position == CardPosition.handcard)
            {
                handOwn.CutOutLine(cardEffect.duelcard.index);
            }
        }
        duelData.activatableEffect.Clear();
    }

    private IEnumerator WantActivate()
    {
        //由玩家选择或者AI选择
        /*
        if (duelData.IsPlayerOwn())
        {
            Tip.content = "是否发动？";
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
                owner = playerOwn,
                controller = playerOwn,
                position = CardPosition.handcard,
                index = duelData.handcard[playerOwn].Count,
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
                owner = playerOps,
                controller = playerOps,
                position = CardPosition.handcard,
                index = duelData.handcard[playerOps].Count,
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
    {//选择怪兽放置
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
    {//特殊召唤时的怪兽表示选择
        //由玩家选择或者AI选择
        if (IsPlayerOwn(duelData.opWho))
            return CardMean.faceupatk;
        else
            return CardMean.faceupatk;
    }

    public void NormalSummonFromHandOwn(DuelCard duelcard, int place, int mean)
    {
        handOwn.RemoveHandCard(duelcard.index);
        duelData.handcard[duelData.opWho].RemoveAt(duelcard.index);
        duelcard.position = CardPosition.monster;
        duelcard.index = place;
        duelcard.mean = mean;
        duelData.monster[duelData.opWho][place] = duelcard;
        monserOwn.ShowMonsterCard(duelcard);
    }

    public void NormalSummonFromHandOps(DuelCard duelcard, int place, int mean)
    {
        handOps.RemoveHandCard(duelcard.index);
        duelData.handcard[duelData.opWho].RemoveAt(duelcard.index);
        duelcard.position = CardPosition.monster;
        duelcard.index = place;
        duelcard.mean = mean;
        duelData.monster[duelData.opWho][place] = duelcard;
        monserOps.ShowMonsterCard(duelcard);
    }

    private void SpecialSummonFromHandOwn(DuelCard duelcard, int place, int mean)
    {
        handOwn.RemoveHandCard(duelcard.index);
        duelData.handcard[duelData.opWho].RemoveAt(duelcard.index);
        duelcard.position = CardPosition.monster;
        duelcard.index = place;
        duelcard.mean = mean;
        duelData.monster[duelData.opWho][place] = duelcard;
        monserOwn.ShowMonsterCard(duelcard);
    }

    private void SpecialSummonFromHandOps(DuelCard duelcard, int place, int mean)
    {
        handOps.RemoveHandCard(duelcard.index);
        duelData.handcard[duelData.opWho].RemoveAt(duelcard.index);
        duelcard.position = CardPosition.monster;
        duelcard.index = place;
        duelcard.mean = mean;
        duelData.monster[duelData.opWho][place] = duelcard;
        monserOps.ShowMonsterCard(duelcard);
    }

    private IEnumerator Battle()
    {
        int atkplayer = duelData.player;//攻击方
        int antiplayer = GetOppPlayer(atkplayer);//被攻击方
        for (int i = 0; i < duelData.areaNum; i++)
        {
            DuelCard atkmonster = duelData.monster[atkplayer][i];
            if (atkmonster == null) continue;
            //没有表侧攻击表示就不能攻击，特殊情况再添加
            if (atkmonster.mean != CardMean.faceupatk) continue;
            //选择攻击目标
            int target = ai.GetAttackTarget();
            if (target == -1)
            {//直接攻击对方
                int atk = duelData.cardDic[atkmonster.card].atk;
                if (IsPlayerOwn(atkplayer)) LPOpsUpdate(duelData.LP[1] - atk);
                else LPOwnUpdate(duelData.LP[0] - atk);
            }
            else
            {//攻击选定的目标
                DuelCard antimonster = duelData.monster[antiplayer][target];
                if (antimonster.mean == CardMean.faceupatk)
                {//对方的怪兽处于攻击表示
                    int atk1 = duelData.cardDic[atkmonster.card].atk;
                    int atk2 = duelData.cardDic[antimonster.card].atk;
                    if (atk1 > atk2)
                    {
                        if (IsPlayerOwn(atkplayer)) LPOpsUpdate(duelData.LP[1] - (atk1 - atk2));
                        else LPOwnUpdate(duelData.LP[0] - (atk1 - atk2));
                        DestroyCard(antimonster, 0);
                    }
                    if (atk1 == atk2)
                    {
                        DestroyCard(atkmonster, 0);
                        DestroyCard(antimonster, 0);
                    }
                    if (atk1 < atk2)
                    {
                        if (IsPlayerOwn(atkplayer)) LPOwnUpdate(duelData.LP[0] - (atk2 - atk1));
                        else LPOpsUpdate(duelData.LP[1] - (atk2 - atk1));
                        DestroyCard(atkmonster, 0);
                    }
                }
                else
                {//对方的怪兽处于防御表示
                    int atk1 = duelData.cardDic[atkmonster.card].atk;
                    int def2 = duelData.cardDic[antimonster.card].def;
                    if (atk1 > def2)
                    {
                        DestroyCard(antimonster, 0);
                    }
                    if (atk1 <= def2)
                    {
                        if (IsPlayerOwn(atkplayer)) LPOwnUpdate(duelData.LP[0] - (def2 - atk1));
                        else LPOpsUpdate(duelData.LP[1] - (def2 - atk1));
                    }
                }
            }
        }
        yield return null;
    }

    public void DestroyCard(DuelCard card, int way)
    {
        if (IsPlayerOwn(card.controller)) monserOwn.HideMonsterCard(card);
        else monserOps.HideMonsterCard(card);
        duelData.monster[card.controller][card.index] = null;
        duelData.grave[card.owner].Insert(0, card.card);
        if (IsPlayerOwn(card.owner)) graveOwn.GraveUpdate(card.owner);
        else graveOps.GraveUpdate(card.owner);
    }
}
