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
    public static SpriteManager spriteManager;
    public static Sprite UIMask;
    private DuelEvent duelEvent;
    public static DuelDataManager duelData;
    public static Dictionary<string, Card> cardDic;
    public LuaCode luaCode;
    public DuelAI duelAI;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        duelData = new DuelDataManager(2);
        luaCode = new LuaCode();
        spriteManager = new SpriteManager();
        duelEvent = gameObject.GetComponent<DuelEvent>();
        duelAI = new DuelAI(this, duelEvent);
        UIMask = GameObject.Find("DeckImageOwn").GetComponent<Image>().sprite;//保存UIMask
        monserOwn = GameObject.Find("MonsterAreaOwn").GetComponent<MonsterOwn>();
        monserOps = GameObject.Find("MonsterAreaOps").GetComponent<MonsterOps>();
        //加载卡组数据
        ReadDeckFile();
        duelEvent.duelData = duelData;
        luaCode.SetCode(duelData.cardData.allcode);
        //放置卡组
        deckOwn.DeckUpdate(0);
        deckOps.DeckUpdate(1);
        //初始化先攻，阶段，生命值
        duelData.player = 0;
        duelData.opWho = 0;
        duelData.duelPhase = 0;
        LPUpdate(0, 8000);
        LPUpdate(1, 8000);
        //各自起手5张卡
        yield return DrawCard(0, 5);
        yield return DrawCard(1, 5);
        //决斗开始
        StartCoroutine(DuelPhase(duelData.duelPhase));
        StartCoroutine(Game());
    }

    // Update is called once per frame
    void Update()
    {
        PhaseButtonShow();
    }

    public void OnQuitClick()
    {
        StopAllCoroutines();
        luaCode.Close();
        Destroy(gameObject);
        Instantiate(mainLayout, GameObject.Find("Canvas").transform);
    }

    public void ReadDeckFile()
    {
        cardDic = duelData.cardData.cardDic;
        string[] deckpath = new string[duelData.playerNum];
        deckpath[0] = Main.rulePath + "/deck/mycard.ydk";
        deckpath[1] = Main.rulePath + "/deck/mycard.ydk";
        for (int player = 0; player < duelData.playerNum; player++)
        {
            string[] strs = File.ReadAllLines(deckpath[player]);
            List<string> deck = new List<string>();
            List<string> extra = new List<string>();
            int i = 0;
            while (!strs[i].Equals("#main")) i++;
            i++;
            while (!strs[i].Equals("#extra"))
            {
                int rmindex = strs[i].IndexOf('#');
                if (rmindex >= 0) strs[i] = strs[i].Remove(rmindex);
                deck.Add(strs[i]);
                i++;
            }
            i++;
            while (!strs[i].Equals("!side"))
            {
                int rmindex = strs[i].IndexOf('#');
                if (rmindex >= 0) strs[i] = strs[i].Remove(rmindex);
                extra.Add(strs[i]);
                i++;
            }
            duelData.cardData.LoadCardData(deck);
            duelData.cardData.LoadCardData(extra);
            foreach (string card in deck)
            {
                if (!cardDic.ContainsKey(card)) continue;
                DuelCard duelcard = new DuelCard
                {
                    owner = player,
                    controller = player,
                    position = CardPosition.deck,
                    index = duelData.deck[player].Count,
                    mean = CardMean.faceup,
                };
                duelcard.SetCard(cardDic[card]);
                duelData.deck[player].Add(duelcard);
            }
            foreach (string card in extra)
            {
                if (!cardDic.ContainsKey(card)) continue;
                DuelCard duelcard = new DuelCard
                {
                    owner = player,
                    controller = player,
                    position = CardPosition.extra,
                    index = duelData.extra[player].Count,
                    mean = CardMean.faceup,
                };
                duelcard.SetCard(cardDic[card]);
                duelData.extra[player].Add(duelcard);
            }
        }
    }

    public void LPUpdate(int player, int change)
    {
        player %= 2;
        duelData.LP[player] += change;
        if (player == 0)
            LPOwn.text = "LP  " + duelData.LP[player];
        else
            LPOps.text = "LP  " + duelData.LP[player];
    }

    private IEnumerator DuelPhase(int phase)
    {
        duelData.duelPhase = phase;
        switch (duelData.duelPhase)
        {
            case GamePhase.turnstart:
                duelData.turnNum++;
                if (IsPlayerOwn(duelData.player)) phaseText.text = "我的回合";
                else phaseText.text = "对方回合";
                Debug.Log("玩家" + duelData.player);
                yield return new WaitForSeconds(1);
                StartCoroutine(EndPhase());
                break;
            case GamePhase.draw:
                phaseText.text = "抽卡阶段";
                Debug.Log("抽卡阶段");
                yield return new WaitForSeconds(1);
                duelEvent.DrawCard(0, 1);
                yield return WaitEventChain();
                StartCoroutine(EndPhase());
                break;
            case GamePhase.standby:
                phaseText.text = "准备阶段";
                Debug.Log("准备阶段");
                yield return new WaitForSeconds(1);
                StartCoroutine(EndPhase());
                break;
            case GamePhase.main1:
                phaseText.text = "主一阶段";
                ChangeBattleButtonText();
                Debug.Log("主一阶段");
                yield return new WaitForSeconds(1);
                yield return DuelAI();
                break;
            case GamePhase.battle:
                phaseText.text = "战斗阶段";
                ChangeBattleButtonText();
                Debug.Log("战斗阶段");
                yield return new WaitForSeconds(1);
                yield return EffectChain();
                yield return DuelAI();
                break;
            case GamePhase.main2:
                phaseText.text = "主二阶段";
                Debug.Log("主二阶段");
                yield return new WaitForSeconds(1);
                yield return DuelAI();
                break;
            case GamePhase.end:
                phaseText.text = "结束阶段";
                Debug.Log("结束阶段");
                yield return new WaitForSeconds(1);
                StartCoroutine(EndPhase());
                break;
            default:
                break;
        }
    }

    private IEnumerator EndPhase(int phase = 0)
    {
        if (duelData.duelPhase >= GamePhase.draw)
        {
            yield return EffectChain();
            BuffRefresh();
            if (duelData.duelPhase == GamePhase.end)
                TurnEndReset();
        }
        if (phase == 0)
            duelData.duelPhase += 10;
        else
            duelData.duelPhase = phase;
        if (duelData.duelPhase > GamePhase.end)
        {
            duelData.duelPhase = GamePhase.turnstart;
            duelData.ChangeNextPlayer();
        }
        StartCoroutine(DuelPhase(duelData.duelPhase));
    }

    private void PhaseButtonShow()
    {
        if (duelData.duelPhase == GamePhase.main1 || duelData.duelPhase == GamePhase.battle)
            battleButton.SetActive(true);
        else
            battleButton.SetActive(false);
        if (duelData.duelPhase == GamePhase.main1 || duelData.duelPhase == GamePhase.battle ||
            duelData.duelPhase == GamePhase.main2)
            endTurnButton.SetActive(true);
        else
            endTurnButton.SetActive(false);
    }

    public void OnEndTurnButtonClick()
    {
        StartCoroutine(EndPhase(GamePhase.end));
    }

    private void ChangeBattleButtonText()
    {
        Text buttonText = battleButton.GetComponentInChildren<Text>();
        if (duelData.duelPhase == GamePhase.main1) buttonText.text = "开始战斗";
        if (duelData.duelPhase == GamePhase.battle) buttonText.text = "结束战斗";
    }

    public void OnBattleButtonClick()
    {
        if (duelData.duelPhase == GamePhase.battle)
            StartCoroutine(EndPhase(GamePhase.main2));
        if (duelData.duelPhase == GamePhase.main1)
            StartCoroutine(EndPhase(GamePhase.battle));
    }

    public IEnumerator Battle(DuelCard atkmonster)
    {
        int atkplayer = atkmonster.controller; // 攻击方
        int antiplayer = GetOppPlayer(atkplayer); // 被攻击方
        // 战斗步骤
        duelData.duelPhase = GamePhase.battlestep;
        int target = duelAI.GetAttackTarget();
        atkmonster.battledeclare++;
        DuelCard antimonster = null;
        if (target != -1) antimonster = duelData.monster[antiplayer][target];
        DuelRecord record = new DuelRecord(PlayerAction.battle);
        record.AddCard(atkmonster);
        record.AddCard(antimonster);
        duelData.record.Add(record);
        Debug.Log("战斗步骤");
        yield return new WaitForSeconds(1);
        yield return EffectChain();
        BuffRefresh();
        // 伤害步骤开始时
        duelData.duelPhase = GamePhase.damageStepStart;
        Debug.Log("伤害步骤开始时");
        yield return EffectChain();
        BuffRefresh();
        // 伤害计算前
        duelData.duelPhase = GamePhase.damageCalBefore;
        Debug.Log("伤害计算前");
        yield return EffectChain();
        BuffRefresh();
        // 伤害计算时
        duelData.duelPhase = GamePhase.damageCalculate;
        Debug.Log("伤害计算时");
        yield return EffectChain();
        int destroycard = 0;
        if (target == -1)
        { // 直接攻击对方
            int atk = atkmonster.atk;
            LPUpdate(antiplayer, -atk);
        }
        else
        { // 攻击选定的目标
            if (antimonster.mean == CardMean.faceupatk)
            { // 对方的怪兽处于攻击表示
                int atk1 = atkmonster.atk;
                int atk2 = antimonster.atk;
                if (atk1 > atk2)
                {
                    LPUpdate(antiplayer, -(atk1 - atk2));
                    destroycard = 2;
                }
                if (atk1 == atk2)
                {
                    if (atk1 != 0)
                    { // 攻击力为0的怪兽不造成伤害
                        destroycard = 3;
                    }
                }
                if (atk1 < atk2)
                {
                    LPUpdate(atkplayer, -(atk2 - atk1));
                    destroycard = 1;
                }
            }
            else
            { // 对方的怪兽处于防御表示
                int atk1 = atkmonster.atk;
                int def2 = antimonster.def;
                if (atk1 > def2)
                {
                    destroycard = 2;
                }
                if (atk1 <= def2)
                {
                    LPUpdate(atkplayer, -(def2 - atk1));
                }
            }
        }
        BuffRefresh();
        // 伤害计算后
        duelData.duelPhase = GamePhase.damageCalAfter;
        Debug.Log("伤害计算后");
        yield return EffectChain();
        BuffRefresh();
        // 伤害步骤终了时
        duelData.duelPhase = GamePhase.damageStepEnd;
        Debug.Log("伤害步骤终了时");
        if (destroycard == 1)
        {
            CardLeave(atkmonster, GameEvent.battledestroy);
        }
        if (destroycard == 2)
        {
            CardLeave(antimonster, GameEvent.battledestroy);
        }
        if (destroycard == 3)
        {
            CardLeave(atkmonster, GameEvent.battledestroy);
            CardLeave(antimonster, GameEvent.battledestroy);
        }
        yield return EffectChain();
        BuffRefresh();
        duelData.duelPhase = GamePhase.battle;
    }

    private IEnumerator Game()
    {
        int intdata1;
        int intdata2;
        DuelCard duelcarddata;
        List<DuelCard> cardlistdata;
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
                    intdata1 = (int)eData.data["drawplayer"];
                    intdata2 = (int)eData.data["drawnum"];
                    if (intdata1 == 0)
                    { // 自己抽卡
                        yield return DrawCard(player, intdata2);
                    }
                    if (intdata1 == 1)
                    { // 对方抽卡
                        yield return DrawCard(GetOppPlayer(player), intdata2);
                    }
                    if (intdata1 == 2)
                    { // 双方同时抽卡
                    // 同时行动时，先处理回合玩家
                        player = duelData.player;
                        yield return DrawCard(player, intdata2);
                        yield return DrawCard(GetOppPlayer(player), intdata2);
                    }
                    break;
                case GameEvent.selectcard:
                    cardlistdata = eData.data["targetlist"] as List<DuelCard>;
                    intdata1 = (int)eData.data["num"];
                    intdata2 = (int)eData.data["gameEvent"];
                    yield return SelectCard(cardlistdata, intdata1);
                    if (intdata2 == GameEvent.specialsummon)
                    {
                        duelEvent.SpecialSummon(cardlistdata);
                    }
                    break;
                case GameEvent.normalsummon:
                    duelcarddata = eData.data["handcard"] as DuelCard;
                    yield return SelectMonsterPlace();
                    intdata1 = SelectMonsterMean(eData.gameEvent);
                    NormalSummonFromHand(duelcarddata, duelData.placeSelect, intdata1);
                    break;
                case GameEvent.specialsummon:
                    duelcarddata = eData.data["monstercard"] as DuelCard;
                    yield return SelectMonsterPlace();
                    intdata1 = SelectMonsterMean(eData.gameEvent);
                    SpecialSummon(duelcarddata, duelData.placeSelect, intdata1);
                    break;
                case GameEvent.changemean:
                    duelcarddata = eData.data["monstercard"] as DuelCard;
                    intdata1 = (int)eData.data["monstermean"];
                    ChangeMean(duelcarddata, intdata1);
                    break;
                default:
                    break;
            }
            duelData.eventDate.RemoveAt(0);
        }
    }

    private IEnumerator WaitGameEvent()
    {
        while (duelData.eventDate.Count > 0)
        {
            yield return null;
        }
    }

    private IEnumerator WaitEventChain()
    {
        while (duelData.eventDate.Count > 0 || duelData.effectChain)
        {
            yield return null;
        }
    }

    private IEnumerator PayCost(CardEffect cardEffect)
    {
        Debug.Log("玩家" + cardEffect.duelcard.controller + " 卡牌 " + cardEffect.duelcard.name + " 的效果" + cardEffect.effect + " 支付代价");
        duelEvent.SetThisCard(cardEffect.duelcard);
        luaCode.Run(luaCode.CostFunStr(cardEffect.duelcard, cardEffect.effect));
        yield return WaitGameEvent();
    }

    private IEnumerator ActivateEffect(CardEffect cardEffect)
    {
        Debug.Log("玩家" + cardEffect.duelcard.controller + " 卡牌 " + cardEffect.duelcard.name + " 的效果" + cardEffect.effect + " 生效");
        duelEvent.SetThisCard(cardEffect.duelcard);
        luaCode.Run(luaCode.EffectFunStr(cardEffect.duelcard, cardEffect.effect));
        yield return WaitGameEvent();
    }

    public void BuffEffect(DuelBuff buff)
    { // 让buff生效
        Debug.Log("玩家" + buff.fromcard.controller + " 卡牌 " + buff.fromcard.name + " 的效果" + buff.effect + " 生效");
        duelEvent.SetThisCard(buff.fromcard);
        luaCode.Run(luaCode.EffectFunStr(buff.fromcard, buff.effect));
    }

    private IEnumerator EffectChain()
    {
        duelData.effectChain = true;
        bool chain = true;
        int scannum = 0;
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
                { // 由玩家选择或者AI选择
                    int select = 0;
                    activateEffect = duelData.activatableEffect[select];
                    chain = true;
                }
                CutCardOutLine();
                if (chain)
                {
                    Debug.Log("玩家" + activateEffect.duelcard.controller + " 卡牌 " + activateEffect.duelcard.name + " 的效果" + activateEffect.effect + " 发动");
                    int opWho = duelData.opWho;
                    if (activateEffect.cost) yield return PayCost(activateEffect);
                    duelData.chainEffect.Insert(0, activateEffect);
                    duelData.opWho = GetOppPlayer(opWho);
                    yield return new WaitForSeconds(1);
                }
            }
            if (!chain && scannum == 0)
            { // 回合玩家不发动，再看敌对玩家发不发动
                duelData.opWho = GetOppPlayer(duelData.opWho);
                chain = true;
            }
            scannum++;
        }
        while (duelData.chainEffect.Count > 0)
        {
            duelData.opWho = duelData.chainEffect[0].duelcard.controller;
            yield return ActivateEffect(duelData.chainEffect[0]);
            duelData.chainEffect.RemoveAt(0);
            yield return new WaitForSeconds(1);
        }
        duelData.opWho = duelData.player;
        duelData.duelcase.Clear();
        duelData.effectChain = false;
    }

    private void ScanEffect()
    {
        int player = duelData.opWho;
        Debug.Log("扫描效果 player=" + player);
        duelEvent.precheck = true;
        foreach (DuelCard duelcard in duelData.handcard[player])
        {
            duelEvent.SetThisCard(duelcard);
            luaCode.Run("c" + duelcard.id);
        }
        foreach (DuelCard duelcard in duelData.monster[player])
        {
            if (duelcard != null)
            {
                duelEvent.SetThisCard(duelcard);
                luaCode.Run("c" + duelcard.id);
            }
        }
        foreach (DuelCard duelcard in duelData.grave[player])
        {
            duelEvent.SetThisCard(duelcard);
            luaCode.Run("c" + duelcard.id);
        }
        duelEvent.precheck = false;
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
        if (IsPlayerOwn(duelData.opWho))
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
        if (IsPlayerOwn(duelData.opWho))
        {
            Tip.select = 1;
        }
        else
        {
            Tip.select = 1;
        }
        yield return null;
    }

    private IEnumerator DuelAI()
    {
        while (!duelAI.done)
        {
            duelAI.Run();
            if (duelData.duelPhase > GamePhase.battle && duelData.duelPhase < GamePhase.main2)
            {
                while (duelData.duelPhase != GamePhase.battle)
                {
                    yield return null;
                }
            }
            yield return WaitEventChain();
        }
        duelAI.done = false;
    }

    public IEnumerator SelectCard(List<DuelCard> cardlist, int num)
    {
        List<DuelCard> targetlist = new List<DuelCard>();
        for (int i = 0; i < num; i++)
        { // 由玩家选择或者AI选择
            targetlist.Add(cardlist[i]);
        }
        cardlist.Clear();
        cardlist.AddRange(targetlist);
        yield return null;
    }

    private IEnumerator SelectMonsterPlace()
    { // 选择怪兽放置
        List<int> place = GetMonsterPlace();
        // 由玩家选择或者AI选择
        int select = 0;
        if (IsPlayerOwn(duelData.opWho))
        {
            // yield return monserOwn.MonsterPlace(place);
            duelData.placeSelect = place[select];
        }
        else
        {
            duelData.placeSelect = place[select];
        }
        yield return null;
    }

    private int SelectMonsterMean(int gameEvent)
    { // 召唤怪兽时的表示选择
        // 由玩家选择或者AI选择
        if (gameEvent == GameEvent.normalsummon)
        {
            if (IsPlayerOwn(duelData.opWho))
                return CardMean.facedowndef;
            else
                return CardMean.facedowndef;
        }
        if (gameEvent == GameEvent.specialsummon)
        {
            if (IsPlayerOwn(duelData.opWho))
                return CardMean.faceupatk;
            else
                return CardMean.faceupatk;
        }
        return 0;
    }

    private IEnumerator DrawCard(int player, int num)
    {
        duelData.cardsJustDrawn[player].Clear();
        if (duelData.deck[player].Count == 0) yield break;
        while (num > 0)
        {
            yield return new WaitForSeconds(0.1f);
            DuelCard duelcard = duelData.deck[player][0];
            if (IsPlayerOwn(player)) handOwn.AddHandCardFromDeck(duelcard);
            else handOps.AddHandCardFromDeck(duelcard);
            duelcard.position = CardPosition.handcard;
            duelcard.index = duelData.handcard[player].Count;
            duelData.handcard[player].Add(duelcard);
            duelData.cardsJustDrawn[player].Add(duelcard.id);
            duelData.deck[player].RemoveAt(0);
            duelData.SortCard(duelData.deck[player]);
            if (IsPlayerOwn(player)) deckOwn.DeckUpdate(player);
            else deckOps.DeckUpdate(player);
            num--;
        }
    }

    private void NormalSummonFromHand(DuelCard duelcard, int place, int mean)
    {
        int player = duelcard.controller;
        if (IsPlayerOwn(player)) handOwn.RemoveHandCard(duelcard.index);
        else handOps.RemoveHandCard(duelcard.index);
        duelData.handcard[player].RemoveAt(duelcard.index);
        duelData.SortCard(duelData.handcard[player]);
        duelcard.position = CardPosition.monster;
        duelcard.index = place;
        duelcard.mean = mean;
        duelcard.meanchange = 0;
        duelcard.appearturn = duelData.turnNum;
        duelcard.battledeclare = 0;
        duelData.monster[player][place] = duelcard;
        if (IsPlayerOwn(player)) monserOwn.ShowMonsterCard(duelcard);
        else monserOps.ShowMonsterCard(duelcard);
        // 通常召唤次数+1
        duelData.normalsummon[player]++;
    }

    private void SpecialSummon(DuelCard duelcard, int place, int mean)
    {
        int player = duelcard.controller;
        if (duelcard.position == CardPosition.handcard)
        {
            if (IsPlayerOwn(player)) handOwn.RemoveHandCard(duelcard.index);
            else handOps.RemoveHandCard(duelcard.index);
            duelData.handcard[player].RemoveAt(duelcard.index);
            duelData.SortCard(duelData.handcard[player]);
        }
        if (duelcard.position == CardPosition.deck)
        {
            duelData.deck[player].Remove(duelcard);
            duelData.SortCard(duelData.deck[player]);
            if (IsPlayerOwn(player)) deckOwn.DeckUpdate(player);
            else deckOps.DeckUpdate(player);
        }
        if (duelcard.position == CardPosition.grave)
        {
            duelData.grave[player].Remove(duelcard);
            duelData.SortCard(duelData.grave[player]);
            if (IsPlayerOwn(player)) graveOwn.GraveUpdate(player);
            else graveOps.GraveUpdate(player);
        }
        duelcard.position = CardPosition.monster;
        duelcard.index = place;
        duelcard.mean = mean;
        duelcard.meanchange = 0;
        duelcard.appearturn = duelData.turnNum;
        duelcard.battledeclare = 0;
        duelData.monster[player][place] = duelcard;
        if (IsPlayerOwn(player)) monserOwn.ShowMonsterCard(duelcard);
        else monserOps.ShowMonsterCard(duelcard);
    }

    private void ChangeMean(DuelCard duelcard, int mean)
    {
        if (mean != 0) duelcard.mean = mean;
        else
        {
            if (duelcard.mean == CardMean.faceupatk)
                duelcard.mean = CardMean.faceupdef;
            else
                duelcard.mean = CardMean.faceupatk;
        }
        duelcard.meanchange++;
        if (IsPlayerOwn(duelcard.controller)) monserOwn.ShowMonsterCard(duelcard);
        else monserOps.ShowMonsterCard(duelcard);
    }

    private void CardLeave(DuelCard duelcard, int way)
    { // 卡牌被送入墓地或者被除外
        DuelCase duelcase = new DuelCase(way);
        duelcase.old.Add(duelcard.Clone());
        if (duelcard.position == CardPosition.monster)
        {
            if (IsPlayerOwn(duelcard.controller)) monserOwn.HideMonsterCard(duelcard);
            else monserOps.HideMonsterCard(duelcard);
            duelData.monster[duelcard.controller][duelcard.index] = null;
        }
        duelcard.position = CardPosition.grave;
        duelcard.index = 0;
        duelData.grave[duelcard.owner].Insert(0, duelcard);
        duelData.SortCard(duelData.grave[duelcard.owner]);
        if (IsPlayerOwn(duelcard.owner)) graveOwn.GraveUpdate(duelcard.owner);
        else graveOps.GraveUpdate(duelcard.owner);
        duelcase.card.Add(duelcard);
        duelData.duelcase.Add(duelcase);
    }

    private void BuffRefresh()
    {
        List<DuelBuff> remove = new List<DuelBuff>();
        foreach (DuelBuff buff in duelData.duelbuff)
        {
            if ((duelData.turnNum == buff.conturn && duelData.duelPhase >= buff.conphase) || 
                duelData.turnNum > buff.conturn)
            {
                remove.Add(buff);
            }
        }
        foreach (DuelBuff buff in remove)
        {
            duelData.duelbuff.Remove(buff);
        }
        foreach (DuelBuff rmbuff in remove)
        {
            // buff失效后，同类型的buff让其重新生效
            List<DuelBuff> affected = new List<DuelBuff>();
            foreach (DuelBuff buff in duelData.duelbuff)
            {
                if (buff.bufftype == rmbuff.bufftype)
                    affected.Add(buff);
            }
            if (rmbuff.bufftype == BuffType.atknew)
            { // 重置所有怪兽的攻击
                for (int p = 0; p < duelData.playerNum; p++)
                {
                    foreach (DuelCard duelcard in duelData.monster[p])
                    {
                        if (duelcard != null)
                        {
                            duelcard.atk = cardDic[duelcard.id].atk;
                        }
                    }
                }
            }
            foreach (DuelBuff buff in affected)
            {   
                BuffEffect(buff);
            }
        }
    }

    private void TurnEndReset()
    {
        int player = duelData.player;
        foreach (DuelCard duelcard in duelData.monster[player])
        {
            if (duelcard != null)
            {
                duelcard.meanchange = 0;
                duelcard.battledeclare = 0;
            }
        }
        duelData.normalsummon[player] = 0;
    }

    /* 对决斗的判断 */
    public bool IsPlayerOwn(int who)
    {
        if (who == 0 || who == 2)
            return true;
        else
            return false;
    }

    public int GetOppPlayer(int who)
    {//获取对立的玩家
        int oppPlayer = who;
        oppPlayer++;
        if (oppPlayer == duelData.playerNum) oppPlayer = 0;
        return oppPlayer;
    }

    public List<int> GetCanNormalSummon()
    {//获取手卡中可以通常召唤的怪兽
        int player = duelData.opWho;
        List<int> monster = new List<int>();
        for (int i = 0; i < duelData.handcard[player].Count; i++)
        {
            DuelCard duelcard = duelData.handcard[player][i];
            if (NormalSummonCheck(duelcard))
                monster.Add(i);
        }
        return monster;
    }

    public List<int> GetMonsterPlace()
    {//获取可放置的位置
        List<int> place = new List<int>();
        int player = duelData.opWho;
        for (int i = 0; i < duelData.areaNum; i++)
        {
            if (duelData.monster[player][i] == null)
            {
                place.Add(i);
            }
        }
        return place;
    }

    public bool MonsterPlaceCheck()
    {//检查是否有足够召唤的位置
        List<int> place = GetMonsterPlace();
        if (place.Count == 0) return false;
        return true;
    }

    public bool NormalSummonCheck(DuelCard duelcard)
    {//检查能否通常召唤
        if (duelData.normalsummon[duelcard.controller] > 0) return false;
        if (!MonsterPlaceCheck()) return false;
        if (!duelcard.type.Contains(CardType.monster)) return false;
        if (duelcard.level > 4) return false;
        return true;
    }

    public bool SpecialSummonCheck()
    {//检查能否特殊召唤
        if (!MonsterPlaceCheck()) return false;
        return true;
    }

    public bool ChangeMeanCheck(DuelCard duelcard)
    { // 检查怪兽能否变主动更表示形式
        if (duelcard.appearturn == duelData.turnNum) return false;
        if (duelcard.meanchange > 0) return false;
        if (duelcard.battledeclare > 0) return false;
        return true;
    }

    public bool BattleCheck(DuelCard duelcard)
    {
        if (duelcard.mean != CardMean.faceupatk) return false;
        if (duelcard.battledeclare > 0) return false;
        if (duelData.turnNum == 1) return false;
        return true;
    }

    public DuelBuff GetDuelBuff(DuelCard duelcard, int effect)
    { 
        foreach (DuelBuff buff in duelData.duelbuff)
        {
            if (buff.fromcard.Equals(duelcard) && buff.effect == effect)
                return buff;
        }
        return null;
    }

    public List<DuelCard> GetTargetCard(TargetCard targetcard)
    {
        List<DuelCard> targetlist = new List<DuelCard>();
        int player = duelData.opWho;
        if (targetcard.side == PlayerSide.ops)
            player = GetOppPlayer(duelData.opWho);
        targetlist.AddRange(duelData.GetAllCards(player));
        if (targetcard.side == PlayerSide.both)
            targetlist.AddRange(duelData.GetAllCards(GetOppPlayer(player)));

        List<DuelCard> positionlist = new List<DuelCard>();
        foreach (int position in targetcard.position)
        {
            foreach (DuelCard duelcard in targetlist)
            {
                if (position == duelcard.position)
                {
                    positionlist.Add(duelcard);
                }
            }
        }
        if (targetcard.position.Count != 0) targetlist = positionlist;

        List<DuelCard> tlist = new List<DuelCard>();
        foreach (KeyValuePair<int, object> kv in targetcard.target)
        {
            foreach (DuelCard duelcard in targetlist)
            {
                if (kv.Key == GameCard.name)
                {
                    if (duelcard.name.Equals(kv.Value))
                        tlist.Add(duelcard);
                }
            }
        }
        if (targetcard.target.Count != 0) targetlist = tlist;

        return targetlist;
    }

    public bool CardActivated(DuelCard duelcard, int effect)
    {
        foreach (CardEffect cardEffect in duelData.chainEffect)
        {
            if (cardEffect.duelcard.Equals(duelcard) && cardEffect.effect == effect)
                return true;
        }
        return false;
    }
    /* 对决斗的判断 */

    /* 决斗行动记录的运用 */
    public List<DuelCard> GetLastBattleCard()
    {
        List<DuelCard> duelcard = new List<DuelCard>();
        int count = duelData.record.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            if (duelData.record[i].action == PlayerAction.battle)
            {
                List<CardLocation> card = duelData.record[i].card;
                duelcard.Add(card[0].FindDuelCard(duelData));
                if (card.Count == 2)
                    duelcard.Add(card[1].FindDuelCard(duelData));
                break;
            }
        }
        return duelcard;
    }
    /* 决斗行动记录的运用 */
}
