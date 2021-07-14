using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Duel : MonoBehaviour
{
    public GameObject mainLayout;
    public DuelUIManager uiManage;
    public DuelFieldManager fieldManage;
    public DuelEvent duelEvent;
    private Camera mainCamera;
    public static SpriteManager spriteManager;
    public DuelDataManager duelData;
    private LuaCode luaCode;
    private DuelAI duelAI;

    void Awake()
    {
        duelData = new DuelDataManager(2);
        luaCode = new LuaCode();
        spriteManager = new SpriteManager();
        duelAI = new DuelAI(this, duelEvent);
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        fieldManage.SetCover();
        //加载卡组数据
        ReadDeckFile();
        luaCode.SetCode(duelData.cardData.allcode);
        luaCode.SetTestCard();
        ReadCardEffect();
        //放置卡组
        uiManage.PutDeck();
        //初始化先攻，阶段，生命值
        uiManage.SetHint("决斗");
        duelData.player = 0;
        duelData.opWho = 0;
        duelData.duelPhase = 0;
        uiManage.LPUpdate(0, 8000);
        uiManage.LPUpdate(1, 8000);
        yield return new WaitForSeconds(1);
        //各自起手5张卡
        MainView();
        yield return DrawCard(0, 5);
        yield return DrawCard(1, 5);
        duelData.duelcase.Clear();
        //决斗开始
        StartCoroutine(DuelPhase(duelData.duelPhase));
        StartCoroutine(Game());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnQuitClick()
    {
        InitialView();
        luaCode.Close();
        StopAllCoroutines();
        duelData = null;
        fieldManage.ReSetAll();
        Destroy(gameObject);
        Instantiate(mainLayout, GameObject.Find("Canvas").transform);
    }

    public void ReadDeckFile()
    {
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
                if (!duelData.cardData.ContainsCard(card)) continue;
                DuelCard duelcard = new DuelCard
                {
                    cardeffect = new List<CardEffect>(),
                    owner = player,
                    controller = player,
                    position = CardPosition.deck,
                    index = duelData.deck[player].Count,
                    mean = CardMean.faceup,
                    infopublic = false,
                };
                duelcard.SetCard(duelData.cardData.GetCard(card));
                duelData.deck[player].Add(duelcard);
            }
            foreach (string card in extra)
            {
                if (!duelData.cardData.ContainsCard(card)) continue;
                DuelCard duelcard = new DuelCard
                {
                    cardeffect = new List<CardEffect>(),
                    owner = player,
                    controller = player,
                    position = CardPosition.extra,
                    index = duelData.extra[player].Count,
                    mean = CardMean.faceup,
                    infopublic = false,
                };
                duelcard.SetCard(duelData.cardData.GetCard(card));
                duelData.extra[player].Add(duelcard);
            }
        }
    }

    private void LuaFucRun(DuelCard duelcard, string affix, int effect)
    {
        if (duelcard.code.Equals("") && !luaCode.testcard.Contains(duelcard.id)) return;
        luaCode.Run("c" + duelcard.id + affix + (effect == 0 ? "" : effect.ToString()));
    }

    private T LuaFucRun<T>(DuelCard duelcard, string affix, int effect)
    {
        T retvalue = default;
        if (duelcard.code.Equals("") && !luaCode.testcard.Contains(duelcard.id)) return retvalue;
        retvalue = luaCode.Run<T>("c" + duelcard.id + affix + (effect == 0 ? "" : effect.ToString()));
        return retvalue;
    }

    private void ReadCardEffect()
    {
        for (int i = 0; i < duelData.playerNum; i++)
        {
            foreach (DuelCard duelcard in duelData.deck[i])
            {
                duelEvent.SetThisCard(duelcard);
                LuaFucRun(duelcard, "", 0);
            }
            foreach (DuelCard duelcard in duelData.extra[i])
            {
                duelEvent.SetThisCard(duelcard);
                LuaFucRun(duelcard, "", 0);
            }
            AddPubLimit(-1, i, LimitType.specialsummonself, 1);
        }
    }

    private void InitialView()
    {
        mainCamera.transform.position = new Vector3(8f, 2.5f, 3f);
        mainCamera.transform.eulerAngles = new Vector3(19f, -90f, 0f);
    }

    private void MainView()
    {
        mainCamera.transform.position = new Vector3(3f, 2.5f, -1f);
        mainCamera.transform.eulerAngles = new Vector3(35f, 0f, 0f);
        uiManage.ShowCardLayout();
    }

    private void ObserveView(int player)
    {
        if (IsPlayerOwn(player))
        {
            mainCamera.transform.position = new Vector3(3f, 2.5f, 4f);
            mainCamera.transform.eulerAngles = new Vector3(35f, 180f, 0f);
        }
        else
        {
            mainCamera.transform.position = new Vector3(3f, 2.5f, 2f);
            mainCamera.transform.eulerAngles = new Vector3(35f, 0f, 0f);
        }
        // 隐藏手卡布局
        uiManage.HideCardLayout();
        /*
        // 特写视角
        Vector3 camPosition = new Vector3
        {
            x = obsPosition.x,
            y = 1f,
            z = obsPosition.z + (obsPosition.z < 3f ? 1.5f : -1.5f),
        };
        Vector3 dvalue = obsPosition - camPosition;
        Vector3 camAngle = new Vector3
        {
            y = dvalue.z > 0f ? 0f : 180f,
            x = Mathf.Atan2(Mathf.Abs(dvalue.y), Mathf.Abs(dvalue.z)) * Mathf.Rad2Deg * (dvalue.y < 0f ? 1f : -1f),
        };
        mainCamera.transform.eulerAngles = camAngle;
        mainCamera.transform.position = camPosition;
        */
    }

    private IEnumerator DuelPhase(int phase)
    {
        duelData.duelPhase = phase;
        switch (duelData.duelPhase)
        {
            case GamePhase.turnstart:
                duelData.turnNum++;
                if (IsPlayerOwn(duelData.player)) uiManage.SetPhaseText("我的回合");
                else uiManage.SetPhaseText("对方回合");
                Debug.Log("玩家" + duelData.player);
                yield return new WaitForSeconds(1);
                StartCoroutine(EndPhase());
                break;
            case GamePhase.draw:
                uiManage.SetPhaseText("抽卡阶段");
                Debug.Log("抽卡阶段");
                yield return new WaitForSeconds(1);
                duelEvent.DrawCard(1);
                yield return WaitEventChain();
                StartCoroutine(EndPhase());
                break;
            case GamePhase.standby:
                uiManage.SetPhaseText("准备阶段");
                Debug.Log("准备阶段");
                yield return new WaitForSeconds(1);
                StartCoroutine(EndPhase());
                break;
            case GamePhase.main1:
                uiManage.SetPhaseText("主一阶段");
                uiManage.ChangeBattleButtonText("开始战斗");
                Debug.Log("主一阶段");
                yield return new WaitForSeconds(1);
                if (!IsPlayerOwn(duelData.player)) yield return DuelAI();
                break;
            case GamePhase.battle:
                uiManage.SetPhaseText("战斗阶段");
                uiManage.ChangeBattleButtonText("结束战斗");
                Debug.Log("战斗阶段");
                yield return new WaitForSeconds(1);
                yield return EffectChain();
                if(!IsPlayerOwn(duelData.player)) yield return DuelAI();
                break;
            case GamePhase.main2:
                uiManage.SetPhaseText("主二阶段");
                Debug.Log("主二阶段");
                yield return new WaitForSeconds(1);
                if (!IsPlayerOwn(duelData.player)) yield return DuelAI();
                break;
            case GamePhase.end:
                uiManage.SetPhaseText("结束阶段");
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
            if (duelData.duelPhase == GamePhase.end && duelData.handcard[duelData.player].Count > 6)
            {
                TargetCard targetcard = new TargetCard();
                targetcard.SetPosition(CardPosition.handcard);
                duelEvent.SelectCard(targetcard, duelData.handcard[duelData.player].Count - 6, GameEvent.discard);
                yield return WaitEventChain();
            }
            BuffRefresh();
        }
        if (phase == 0)
            duelData.duelPhase += 10;
        else
            duelData.duelPhase = phase;
        if (duelData.duelPhase > GamePhase.end)
        {
            TurnEndReset();
            duelData.duelPhase = GamePhase.turnstart;
            ChangeNextPlayer();
        }
        StartCoroutine(DuelPhase(duelData.duelPhase));
    }

    public void OnEndTurnButtonClick()
    {
        StartCoroutine(EndPhase(GamePhase.end));
    }

    public void OnBattleButtonClick()
    {
        if (duelData.duelPhase == GamePhase.battle)
        {
            duelData.duelPhase = GamePhase.battleEnd;
            StartCoroutine(EndPhase(GamePhase.main2));
        }
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
        yield return ConEffectApply();
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
            uiManage.LPUpdate(antiplayer, -atk);
        }
        else
        { // 攻击选定的目标
            if (antimonster.mean == CardMean.faceupatk)
            { // 对方的怪兽处于攻击表示
                int atk1 = atkmonster.atk;
                int atk2 = antimonster.atk;
                if (atk1 > atk2)
                {
                    uiManage.LPUpdate(antiplayer, -(atk1 - atk2));
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
                    uiManage.LPUpdate(atkplayer, -(atk2 - atk1));
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
                    uiManage.LPUpdate(atkplayer, -(def2 - atk1));
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
            yield return CardLeave(atkmonster, GameEvent.battledestroy);
        }
        if (destroycard == 2)
        {
            yield return CardLeave(antimonster, GameEvent.battledestroy);
        }
        if (destroycard == 3)
        {
            yield return CardLeave(atkmonster, GameEvent.battledestroy);
            yield return CardLeave(antimonster, GameEvent.battledestroy);
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
                    intdata1 = (int)eData.data["drawnum"];
                    MainView();
                    yield return DrawCard(player, intdata1);
                    break;
                case GameEvent.discard:
                    cardlistdata = eData.data["discardlist"] as List<DuelCard>;
                    MainView();
                    yield return CardLeave(cardlistdata, GameEvent.discard);
                    break;
                case GameEvent.selectcard:
                    cardlistdata = eData.data["targetlist"] as List<DuelCard>;
                    intdata1 = (int)eData.data["num"];
                    intdata2 = (int)eData.data["gameEvent"];
                    MainView();
                    yield return SelectCard(cardlistdata, intdata1);
                    yield return new WaitForSeconds(1);
                    if (intdata2 == GameEvent.discard)
                    {
                        duelEvent.DisCard(cardlistdata);
                    }
                    if (intdata2 == GameEvent.specialsummon)
                    {
                        duelEvent.SpecialSummon(cardlistdata);
                    }
                    break;
                case GameEvent.showcard:
                    duelcarddata = eData.data["showcard"] as DuelCard;
                    ShowCard(duelcarddata);
                    break;
                case GameEvent.normalsummon:
                    duelcarddata = eData.data["handcard"] as DuelCard;
                    yield return SelectMonsterPlace();
                    // 默认表侧攻击表示，如果不允许盖放，则多出选项表侧守备表示
                    duelData.meanChoose = CardMean.faceupatk;
                    if (!duelData.setmonster[player%2])
                        yield return SelectMonsterMean(eData.gameEvent, duelcarddata);
                    ObserveView(player);
                    NormalSummon(duelcarddata, duelData.placeSelect, duelData.meanChoose);
                    yield return new WaitForSeconds(1);
                    break;
                case GameEvent.specialsummon:
                    duelcarddata = eData.data["monstercard"] as DuelCard;
                    yield return SelectMonsterPlace();
                    yield return SelectMonsterMean(eData.gameEvent, duelcarddata);
                    ObserveView(player);
                    SpecialSummon(duelcarddata, duelData.placeSelect, duelData.meanChoose);
                    yield return new WaitForSeconds(1);
                    break;
                case GameEvent.setmonster:
                    duelcarddata = eData.data["monstercard"] as DuelCard;
                    yield return SelectMonsterPlace();
                    ObserveView(player);
                    NormalSummon(duelcarddata, duelData.placeSelect, CardMean.facedowndef);
                    yield return new WaitForSeconds(1);
                    break;
                case GameEvent.setmagictrap:
                    duelcarddata = eData.data["magictrapcard"] as DuelCard;
                    yield return SelectMagicTrapPlace();
                    ObserveView(player);
                    yield return UseMagicTrap(duelcarddata, duelData.placeSelect, CardMean.facedownmgt, GameEvent.setmagictrap);
                    yield return new WaitForSeconds(1);
                    break;
                case GameEvent.changemean:
                    duelcarddata = eData.data["monstercard"] as DuelCard;
                    intdata1 = (int)eData.data["monstermean"];
                    ObserveView(player);
                    ChangeMean(duelcarddata, intdata1);
                    yield return new WaitForSeconds(1);
                    break;
                case GameEvent.afterthat:
                    AllTimePointPass();
                    break;
                case GameEvent.effectdestroy:
                    duelcarddata = eData.data["card"] as DuelCard;
                    ObserveView(duelcarddata.controller);
                    yield return CardLeave(duelcarddata, GameEvent.effectdestroy);
                    break;
                case GameEvent.activateinvalid:
                    duelcarddata = duelData.chainEffect[1].duelcard;
                    ObserveView(duelcarddata.controller);
                    duelcarddata.validtype = GameEvent.activateinvalid;
                    Debug.Log("玩家" + duelcarddata.controller + " " + duelcarddata.name+" 被无效");
                    break;
                default:
                    break;
            }
            duelData.eventDate.RemoveAt(0);
            if (!IsInEventChain())
            {
                duelData.opWho = duelData.player;
                StartCoroutine(EffectChain());
            }
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
        while (IsInEventChain())
        {
            yield return null;
        }
    }

    private IEnumerator ActivateEffect(CardEffect cardEffect)
    {
        DuelCard duelcard = cardEffect.duelcard;
        ObserveView(duelcard.controller); // 发动卡时，角色出示卡牌，给角色特写
        ActivateTimePointPass();
        int way = GameEvent.activateeffect;
        if (!duelcard.type.Contains(CardType.monster))
        {
            if (duelcard.position == CardPosition.handcard)
            {
                yield return SelectMagicTrapPlace();
                yield return UseMagicTrap(duelcard, duelData.placeSelect, CardMean.faceupmgt, GameEvent.activatecard);
                way = GameEvent.activatecard;
            }
            if (duelcard.position == CardPosition.magictrap && duelcard.mean == CardMean.facedownmgt)
            {
                yield return ActivateCoverCard(duelcard);
                way = GameEvent.activatecard;
            }
        }
        if (cardEffect.cost) yield return PayCost(cardEffect);
        Debug.Log("玩家" + duelcard.controller + " 卡牌 " + duelcard.name + " 的效果" + cardEffect.effect + " 发动");
        duelData.eventText = uiManage.activateTip.ActivateText(cardEffect);
        DuelCase duelcase = new DuelCase(way);
        duelcase.type = "activate";
        duelcase.card.Add(duelcard);
        duelData.duelcase.Add(duelcase);
        if (cardEffect.limit.Count > 0) LimitCount(duelcard, cardEffect.effect, cardEffect.limit);
        duelData.chainEffect.Insert(0, cardEffect);
        yield return new WaitForSeconds(1);
    }

    private IEnumerator PayCost(CardEffect cardEffect)
    {
        DuelCard duelcard = cardEffect.duelcard;
        Debug.Log("玩家" + duelcard.controller + " 卡牌 " + duelcard.name + " 的效果" + cardEffect.effect + " 支付代价");
        duelEvent.SetThisCard(duelcard);
        LuaFucRun(duelcard, "cost", cardEffect.effect);
        yield return WaitGameEvent();
        CostTimePointPass();
    }

    private IEnumerator EffectApply(CardEffect cardEffect)
    {
        DuelCard duelcard = cardEffect.duelcard;
        Debug.Log("玩家" + duelcard.controller + " 卡牌 " + duelcard.name + " 的效果" + cardEffect.effect + " 生效");
        duelEvent.SetThisCard(duelcard);
        LuaFucRun(duelcard, "effect", cardEffect.effect);
        yield return WaitGameEvent();
        SetLastCaseType("effect");
    }

    private void BuffApply(CardEffect cardEffect)
    {
        DuelCard duelcard = cardEffect.duelcard;
        duelEvent.SetThisCard(duelcard);
        LuaFucRun(duelcard, "effect", cardEffect.effect);
    }

    private IEnumerator ConEffectApply()
    { // 永续效果立即生效
        for (int player = 0; player < duelData.playerNum; player++)
        {
            ScanEffect(player, EffectType.continuous);
        }
        foreach (CardEffect cardEffect in duelData.immediateEffect)
        {
            yield return EffectApply(cardEffect);
        }
        duelData.immediateEffect.Clear();
    }

    private IEnumerator MagicTrapLeave()
    { // 连锁完魔法陷阱送去墓地
        for (int player = 0; player < duelData.playerNum; player++)
        {
            foreach (DuelCard duelcard in duelData.magictrap[player])
            {
                if (duelcard == null) continue;
                if (!duelcard.type.Contains(CardType.monster))
                {
                    if (duelcard.validtype == GameEvent.activateinvalid)
                    {
                        yield return CardLeave(duelcard, GameEvent.activateinvalid);
                        continue;
                    }
                    if (!duelcard.type.Contains(MagicType.continuous) &&
                        !duelcard.type.Contains(MagicType.equip) &&
                        !duelcard.type.Contains(MagicType.field) &&
                        duelcard.mean == CardMean.faceupmgt)
                    {
                        yield return CardLeave(duelcard, GameEvent.activateover);
                        continue;
                    }
                }
            }
        }
    }

    public IEnumerator CardActivate(CardEffect activateEffect)
    {
        duelData.effectChain = true;
        duelData.activatableEffect.Clear();
        yield return ActivateEffect(activateEffect);
        duelData.opWho = GetOppPlayer(duelData.opWho);
        yield return EffectChain();
    }

    private IEnumerator EffectChain()
    {
        duelData.effectChain = true;
        yield return null;
        bool newchain = true;
        while (newchain)
        {
            ClearPassCase();
            if (duelData.chainEffect.Count == 0 &&
                duelData.duelPhase != GamePhase.battle && duelData.duelPhase != GamePhase.battleEnd)
            { // 战斗阶段开始和战斗阶段结束时，发动的诱发效果不会组成连锁，而是1个1个的另开连锁发动
                yield return TriggerChain();
            }
            int noactivate = 0;
            while (noactivate < 2)
            { // 双方都不发动才不继续扫描
                bool chain = false;
                Debug.Log("扫描效果 玩家" + duelData.opWho);
                ScanEffect(duelData.opWho, EffectType.activate);
                uiManage.SetCardOutline();
                if (duelData.activatableEffect.Count > 0)
                {
                    // 由玩家选择或者AI选择
                    if (IsPlayerOwn(duelData.opWho))
                    {
                        MainView();
                        yield return uiManage.WaitChooseActivate();
                    }
                    else duelData.optionChoose = 0;
                    CardEffect activateEffect = null;
                    if (duelData.optionChoose >= 0)
                    {
                        activateEffect = duelData.activatableEffect[duelData.optionChoose];
                        chain = true;
                    }
                    uiManage.CutCardOutline();
                    if (chain)
                    {
                        yield return ActivateEffect(activateEffect);
                    }
                }
                if (chain) noactivate = 0;
                else noactivate++;
                duelData.opWho = GetOppPlayer(duelData.opWho);
            }
            if (duelData.chainEffect.Count == 0) newchain = false;
            while (duelData.chainEffect.Count > 0)
            {
                if (duelData.chainEffect[0].duelcard.validtype != GameEvent.activateinvalid)
                {
                    AllTimePointPass();
                    duelData.opWho = duelData.chainEffect[0].duelcard.controller;
                    yield return EffectApply(duelData.chainEffect[0]);
                }
                duelData.chainEffect.RemoveAt(0);
            }
            ChainLimitReset();
            yield return MagicTrapLeave();
            duelData.opWho = duelData.player;
        }
        duelData.duelcase.Clear();
        duelData.effectChain = false;
        // 自己的操作和事件完成后，要回到主视角
        if (IsPlayerOwn(duelData.opWho) && duelAI.done) MainView();
    }

    private IEnumerator TriggerChain()
    {
        for (int order = 0; order < 6; order++)
        { // 同一时点发动多个诱发类效果
            List<CardEffect> effectList = ScanTriggerEffect(order);
            if (effectList.Count > 0)
            {
                Debug.Log("顺序" + (order + 1) + " 诱发类效果");
                duelData.opWho = effectList[0].duelcard.controller;
                if (effectList[0].effectType == EffectType.musttrigger || effectList[0].effectType == EffectType.musttriggerinstant)
                {
                    while (effectList.Count > 0)
                    {
                        yield return ActivateEffect(effectList[0]);
                        effectList.RemoveAt(0);
                    }
                }
                else
                {
                    while (effectList.Count > 0)
                    {
                        yield return uiManage.WantActivate();
                        if (duelData.optionChoose == 1)
                        {
                            yield return ActivateEffect(effectList[0]);
                        }
                        effectList.RemoveAt(0);
                    }
                }
            }
        }
        duelData.activatableEffect.Clear();
        if (duelData.chainEffect.Count > 0)
            duelData.opWho = GetOppPlayer(duelData.opWho);
        else
            duelData.opWho = duelData.player;
    }

    public int ScanEffect(int player, int effectType)
    {
        duelData.opWho = player;
        duelEvent.precheck = true;
        TargetCard targetcard = new TargetCard();
        targetcard.SetPosition(CardPosition.handcard);
        targetcard.SetPosition(CardPosition.monster);
        targetcard.SetPosition(CardPosition.magictrap);
        targetcard.SetPosition(CardPosition.grave);
        List<DuelCard> cardList = GetTargetCard(targetcard);
        foreach (DuelCard duelcard in cardList)
        {
            duelEvent.SetThisCard(duelcard);
            foreach (CardEffect cEffect in duelcard.cardeffect)
            {
                if (effectType == EffectType.activate && cEffect.effectType > EffectType.activate) continue;
                if (effectType == EffectType.continuous && cEffect.effectType != EffectType.continuous) continue;
                if (!ActivateCheck(duelcard, cEffect)) continue;
                duelEvent.SetThisEffect(cEffect.effect);
                if (!cEffect.condition || (cEffect.condition && LuaFucRun<bool>(duelcard, "condition", cEffect.effect)))
                {
                    if (cEffect.cost) LuaFucRun(duelcard, "cost", cEffect.effect);
                    LuaFucRun(duelcard, "effect", cEffect.effect);
                    duelEvent.SetDuelEffect();
                }
            }
        }
        duelEvent.precheck = false;
        return duelData.activatableEffect.Count;
    }

    public List<CardEffect> ScanTriggerEffect(int order)
    {
        int player;
        if (order == 0)
        {
            for (player = 0; player < duelData.playerNum; player++)
            {
                ScanEffect(player, EffectType.activate);
            }
        }
        List<CardEffect> effectList = new List<CardEffect>();
        if (order % 2 == 0) player = duelData.player;
        else player = GetOppPlayer(duelData.player);
        foreach (CardEffect cardEffect in duelData.activatableEffect)
        { 
            if ((order == 0 || order == 1) && cardEffect.duelcard.controller == player && cardEffect.effectType == EffectType.musttrigger &&
                cardEffect.speed == 1)
            { // 回合玩家的1速必发的诱发类效果  非回合玩家的1速必发的诱发类效果
                effectList.Add(cardEffect);
            }
            if ((order == 2 || order == 3) && cardEffect.duelcard.controller == player && cardEffect.effectType == EffectType.cantrigger &&
                cardEffect.speed == 1)
            { // 回合玩家的公开情报的1速选发的诱发类效果  非回合玩家的公开情报的1速选发的诱发类效果
                effectList.Add(cardEffect);
            }
            if ((order == 4 || order == 5) && cardEffect.duelcard.controller == player && cardEffect.effectType == EffectType.musttriggerinstant &&
                cardEffect.speed == 2)
            { // 回合玩家的2速必发的诱发即时类效果  非回合玩家的2速必发的诱发即时类效果
                effectList.Add(cardEffect);
            }
        }
        return effectList;
    }

    private IEnumerator DuelAI()
    {
        duelAI.done = false;
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
        MainView();
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
            MainView();
            yield return fieldManage.WaitMonsterPlace(place);
        }
        else
        {
            duelData.placeSelect = place[select];
        }
        yield return null;
    }

    private IEnumerator SelectMagicTrapPlace()
    { // 选择魔法陷阱放置
        List<int> place = GetMagicTrapPlace();
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

    private IEnumerator SelectMonsterMean(int gameEvent, DuelCard duelcard)
    { // 召唤怪兽时的表示选择
        // 由玩家选择或者AI选择
        int[] means = new int[2] { CardMean.faceupatk, CardMean.faceupdef };
        if (gameEvent == GameEvent.normalsummon)
        {
            if (IsPlayerOwn(duelData.opWho))
            {
                yield return uiManage.WaitMeanChoose(duelcard, false);
                fieldManage.HideMonsterPlaceParticle(duelData.placeSelect);
                duelData.meanChoose = means[duelData.meanChoose];
            }
            else
                duelData.meanChoose = means[0];
        }
        if (gameEvent == GameEvent.specialsummon)
        {
            if (IsPlayerOwn(duelData.opWho))
            {
                yield return uiManage.WaitMeanChoose(duelcard, false);
                fieldManage.HideMonsterPlaceParticle(duelData.placeSelect);
                duelData.meanChoose = means[duelData.meanChoose];
            }
            else
                duelData.meanChoose = means[0];
        }
    }

    private IEnumerator DrawCard(int player, int num)
    {
        if (duelData.deck[player].Count == 0) yield break;
        duelData.eventText = uiManage.activateTip.DrawCardText(player, num);
        DuelCase duelcase = new DuelCase(GameEvent.drawcard);
        while (num > 0)
        {
            yield return new WaitForSeconds(0.1f);
            DuelCard duelcard = duelData.deck[player][0];
            uiManage.DeckRemove(player, duelcard);
            uiManage.HandCardAdd(player, duelcard);
            duelcase.card.Add(duelcard);
            num--;
        }
        duelData.duelcase.Add(duelcase);
        if (!duelData.effectChain) SetLastCaseType("event");
    }

    private void ShowCard(DuelCard duelcard)
    {
        DuelCase duelcase = new DuelCase(GameEvent.showcard);
        Debug.Log("给对方观看卡牌  " + duelcard.name);
        duelcase.card.Add(duelcard);
        duelData.duelcase.Add(duelcase);
    }

    private void NormalSummon(DuelCard duelcard, int place, int mean)
    {
        int player = duelcard.controller;
        if (mean == CardMean.faceupatk)
            Debug.Log("玩家" + player + " 通常召唤 " + duelcard.name);
        if (mean == CardMean.facedowndef)
            Debug.Log("玩家" + player + " 盖放 " + duelcard.name);
        uiManage.HandCardRemove(player, duelcard);
        duelcard.position = CardPosition.monster;
        duelcard.index = place;
        duelcard.mean = mean;
        duelcard.meanchange = 0;
        if (mean != CardMean.facedowndef) duelcard.infopublic = true;
        duelcard.appearturn = duelData.turnNum;
        duelcard.battledeclare = 0;
        duelData.monster[player][place] = duelcard;
        fieldManage.MonsterShow(duelcard);
        // 通常召唤次数+1
        duelData.normalsummon[player]++;
    }

    private void SpecialSummon(DuelCard duelcard, int place, int mean)
    {
        int player = duelcard.controller;
        Debug.Log("玩家" + player + " 特殊召唤 " + duelcard.name);
        if (duelcard.position == CardPosition.handcard)
        {
            uiManage.HandCardRemove(player, duelcard);
        }
        if (duelcard.position == CardPosition.deck)
        {
            uiManage.DeckRemove(player, duelcard);
        }
        if (duelcard.position == CardPosition.grave)
        {
            uiManage.GraveRemove(player, duelcard);
        }
        duelcard.position = CardPosition.monster;
        duelcard.index = place;
        duelcard.mean = mean;
        duelcard.meanchange = 0;
        duelcard.infopublic = true;
        duelcard.appearturn = duelData.turnNum;
        duelcard.battledeclare = 0;
        duelData.monster[player][place] = duelcard;
        fieldManage.MonsterShow(duelcard);
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
        fieldManage.MonsterShow(duelcard);
    }

    private IEnumerator UseMagicTrap(DuelCard duelcard, int place, int mean, int way)
    {
        int player = duelcard.controller;
        if (mean == CardMean.facedownmgt)
            Debug.Log("玩家" + player + " 盖放 " + duelcard.name);
        DuelCase duelcase = new DuelCase(way);
        duelcase.old.Add(duelcard.Clone());
        if (duelcard.position == CardPosition.handcard)
        {
            uiManage.HandCardRemove(player, duelcard);
        }
        duelcard.position = CardPosition.magictrap;
        duelcard.index = place;
        duelcard.mean = mean;
        duelcard.appearturn = duelData.turnNum;
        duelData.magictrap[player][place] = duelcard;
        yield return fieldManage.MagicTrapShow(duelcard, false);
        duelcase.card.Add(duelcard);
        if (way != GameEvent.activatecard)
            duelData.duelcase.Add(duelcase);
    }

    private IEnumerator ActivateCoverCard(DuelCard duelcard)
    {
        duelcard.mean = CardMean.faceupmgt;
        yield return fieldManage.MagicTrapShow(duelcard, true);
    }

    private IEnumerator CardLeave(DuelCard duelcard, int way)
    { // 卡牌被送入墓地或者被除外
        List<DuelCard> cardlist = new List<DuelCard>
        {
            duelcard
        };
        yield return CardLeave(cardlist, way);
    }

    private IEnumerator CardLeave(List<DuelCard> cardlist, int way)
    { // 卡牌被送入墓地或者被除外
        DuelCase duelcase = new DuelCase(way);
        foreach (DuelCard duelcard in cardlist)
        {
            Debug.Log("玩家" + duelcard.controller + " " + duelcard.name + " 送入墓地");
            duelcase.old.Add(duelcard.Clone());
            if (duelcard.position == CardPosition.handcard)
            {
                uiManage.HandCardRemove(duelcard.controller, duelcard);
            }
            if (duelcard.position == CardPosition.monster)
            {
                fieldManage.MonsterRemove(duelcard);
            }
            if (duelcard.position == CardPosition.magictrap)
            {
                fieldManage.MagicTrapRemove(duelcard);
            }
            duelcard.infopublic = true;
            uiManage.GraveInsert(duelcard.owner, 0, duelcard);
            duelcase.card.Add(duelcard);
            yield return new WaitForSeconds(0.1f);
        }
        duelData.duelcase.Add(duelcase);
        if (!duelData.effectChain) SetLastCaseType("event");
    }

    private void BuffRefresh()
    {
        List<CardEffect> remove = new List<CardEffect>();
        foreach (CardEffect buff in duelData.buffeffect)
        {
            if ((duelData.turnNum == buff.contime.toturn && duelData.duelPhase >= buff.contime.phase) ||
                duelData.turnNum > buff.contime.toturn)
            {
                remove.Add(buff);
            }
        }
        foreach (CardEffect buff in remove)
        {
            duelData.buffeffect.Remove(buff);
        }
        TargetCard targetcard = new TargetCard();
        targetcard.SetSide(PlayerSide.both);
        targetcard.SetPosition(CardPosition.handcard);
        targetcard.SetPosition(CardPosition.monster);
        targetcard.SetPosition(CardPosition.magictrap);
        List<DuelCard> cardList = GetTargetCard(targetcard);
        foreach (DuelCard duelcard in cardList)
        { // 重置双方场上和手卡的信息
            duelcard.ResetCard(duelData.cardData.GetCard(duelcard.id));
        }
        foreach (CardEffect buff in duelData.buffeffect)
        { // 让buff重新生效
            // 后期加入动作后要注意不要有动作
            BuffApply(buff);
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
        TurnLimitReset();
    }

    #region 对决斗的判断
    public bool IsPlayerOwn(int who)
    {
        if (who == 0 || who == 2)
            return true;
        else
            return false;
    }

    public int GetOppPlayer(int who)
    { // 获取对立的玩家
        int oppPlayer = who;
        oppPlayer++;
        if (oppPlayer == duelData.playerNum) oppPlayer = 0;
        return oppPlayer;
    }

    public void ChangeNextPlayer()
    {
        duelData.player++;
        if (duelData.player == duelData.playerNum) duelData.player = 0;
        duelData.opWho = duelData.player;
    }

    public List<int> GetPlayerOrder()
    { // 从回合玩家开始获取玩家顺序
        List<int> order = new List<int>();
        int player = duelData.player;
        for (int i = 0; i < duelData.playerNum; i++)
        {
            order.Add(player);
            player = GetOppPlayer(player);
        }
        return order;
    }

    public List<int> GetSidePlayer(int side)
    { // 获取对应方的玩家
        List<int> players = new List<int>();
        int player = duelData.opWho;
        if (side == PlayerSide.own)
            players.Add(player);
        if (side == PlayerSide.ops)
        { // 双人对战时获取对方需要有选择对方的哪一方
            players.Add(GetOppPlayer(player));
        }
        if (side == PlayerSide.both)
        { // 双方同时处理时，回合玩家先处理
            players = GetPlayerOrder();
        }
        return players;
    }

    public bool IsInEventChain()
    { // 判断是否处于事件或者效果连锁之中
        if (duelData.eventDate.Count > 0) return true;
        if (duelData.effectChain) return true;
        return false;
    }

    public bool IsMainFree()
    { // 判断是否是玩家主要阶段的自由操作时间
        if (duelData.duelPhase != GamePhase.main1 && duelData.duelPhase != GamePhase.main2) return false;
        if (IsInEventChain()) return false;
        return true;
    }

    public bool IsBattleTime()
    { // 判断是否是玩家战斗阶段的选择怪兽战斗时间
        if (duelData.duelPhase != GamePhase.battle) return false;
        if (IsInEventChain()) return false;
        return true;
    }

    public int GetCardSpeed(CardEffect cardEffect)
    {
        int speed = 0;
        DuelCard duelcard = cardEffect.duelcard;
        if (duelcard.type.Contains(CardType.monster))
        {
            if (cardEffect.effectType == EffectType.cantriggerinstant || cardEffect.effectType == EffectType.musttriggerinstant)
            { // 怪兽的诱发即时效果为2速
                speed = 2;
            }
            else
            { // 其他怪兽效果为1速
                speed = 1;
            }
        }
        if (duelcard.type.Contains(CardType.magic))
        {
            if (duelcard.type.Contains(MagicType.quick))
            { // 速攻魔法为2速
                speed = 2;
            }
            else
            { // 其他魔法为1速
                speed = 1;
            }
        }
        if (duelcard.type.Contains(CardType.trap))
        {
            if (duelcard.type.Contains(TrapType.counter))
            { // 反击陷阱为3速
                speed = 3;
            }
            else
            { // 其他陷阱为2速
                speed = 2;
            }
        }
        return speed;
    }

    public List<int> GetMonsterPlace()
    { // 获取怪兽可放置的位置
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
    { // 检查是否有足够召唤怪兽的位置
        List<int> place = GetMonsterPlace();
        if (place.Count == 0) return false;
        return true;
    }

    public bool NormalSummonCheck(DuelCard duelcard)
    { // 检查能否通常召唤
        if (!duelcard.type.Contains(CardType.monster)) return false;
        if (duelData.normalsummon[duelcard.controller] > 0) return false;
        if (!MonsterPlaceCheck()) return false;
        if (duelcard.level > 4) return false;
        return true;
    }

    public bool SpecialSummonCheck()
    { // 检查能否特殊召唤
        if (!MonsterPlaceCheck()) return false;
        return true;
    }

    public List<int> GetCanNormalSummon()
    { // 获取手卡中可以通常召唤的怪兽
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

    public bool SetMonsterCheck(DuelCard duelcard)
    { // 检查能否盖放怪兽
        if (!duelcard.type.Contains(CardType.monster)) return false;
        if (!duelData.setmonster[duelcard.controller]) return false;
        if (duelData.normalsummon[duelcard.controller] > 0) return false;
        if (!MonsterPlaceCheck()) return false;
        if (duelcard.level > 4) return false;
        return true;
    }

    public bool ChangeMeanCheck(DuelCard duelcard)
    { // 检查怪兽能否主动变更表示形式
        if (duelcard.appearturn == duelData.turnNum) return false;
        if (duelcard.meanchange > 0) return false;
        if (duelcard.battledeclare > 0) return false;
        return true;
    }

    public List<int> GetMagicTrapPlace()
    { // 获取魔法陷阱可放置的位置
        List<int> place = new List<int>();
        int player = duelData.opWho;
        for (int i = 0; i < duelData.areaNum; i++)
        {
            if (duelData.magictrap[player][i] == null)
            {
                place.Add(i);
            }
        }
        return place;
    }

    public bool MagicTrapPlaceCheck()
    { // 检查是否有足够放置魔法陷阱的位置
        List<int> place = GetMagicTrapPlace();
        if (place.Count == 0) return false;
        return true;
    }

    public bool UseMagicTrapCheck(DuelCard duelcard)
    { // 检查能否从手卡使用魔法陷阱
        if (!MagicTrapPlaceCheck()) return false;
        if (!duelcard.type.Contains(CardType.magic) && !duelcard.type.Contains(CardType.trap) || duelcard.type.Contains(MagicType.field)) return false;
        return true;
    }

    public bool ActivateTrapCheck(DuelCard duelcard)
    { // 检查能否发动陷阱
        if (duelcard.appearturn == duelData.turnNum) return false;
        return true;
    }

    public bool BattleCheck(DuelCard duelcard)
    { // 检查怪兽能否攻击
        if (duelcard.mean != CardMean.faceupatk) return false;
        if (duelcard.battledeclare > 0) return false;
        if (duelData.turnNum == 1) return false;
        return true;
    }

    public bool CardActivated(DuelCard duelcard, int effect)
    { // 卡牌的效果是否已经发动
        foreach (CardEffect cardEffect in duelData.chainEffect)
        {
            if (cardEffect.duelcard.Equals(duelcard) && cardEffect.effect == effect)
                return true;
        }
        return false;
    }

    public bool BuffApplied(DuelCard duelcard, int effect)
    { // buff是否已经生效
        foreach (CardEffect buff in duelData.buffeffect)
        {
            if (buff.duelcard.Equals(duelcard) && buff.effect == effect)
                return true;
        }
        return false;
    }

    public bool ActivateCheck(DuelCard duelcard, CardEffect cardEffect)
    {  // 检查能否发动
        // 检查场上是否有足够的位置
        if (cardEffect.effectType == EffectType.startup)
        {
            if (duelcard.type.Contains(CardType.magic) && !duelcard.type.Contains(MagicType.field))
            { // 魔法卡必须有能发动的位置
                if (duelcard.position == CardPosition.handcard)
                {
                    if (!MagicTrapPlaceCheck()) return false;
                }
            }
            // 连锁期间，启动效果不能发动
            if (duelData.effectChain) return false;
        }
        // 检查效果是否已经发动
        if (cardEffect.effectType < EffectType.activate)
        {
            if (CardActivated(duelcard, cardEffect.effect)) return false;
        }
        if (cardEffect.effectType == EffectType.continuous)
        {
            if (BuffApplied(duelcard, cardEffect.effect)) return false;
        }
        // 检查发动位置是否正确
        if (!cardEffect.position)
        {
            if (cardEffect.effectType == EffectType.startup)
            {
                if (duelcard.type.Contains(CardType.monster))
                { // 怪兽卡必须在场上
                    if (duelcard.position < CardPosition.area) return false;
                }
            }
            if (cardEffect.effectType < EffectType.activate)
            {
                if (duelcard.type.Contains(CardType.magic))
                { // 魔法卡必须在场上或者手卡
                    if (duelcard.position < CardPosition.handcard) return false;
                }
                if (duelcard.type.Contains(CardType.trap))
                { // 陷阱卡必须在场上，且过了1回合
                    if (duelcard.position < CardPosition.area) return false;
                    if (!ActivateTrapCheck(duelcard)) return false;
                }
            }
            if (cardEffect.effectType == EffectType.continuous)
            { // 必须在场上表侧表示存在
                if (duelcard.position < CardPosition.area) return false;
                if (duelcard.mean > CardMean.faceup) return false;
            }
        }
        return true;
    }

    public bool ChainCheck(CardEffect cardEffect)
    {
        DuelCard duelcard = cardEffect.duelcard;
        if (duelcard.type.Contains(CardType.monster) &&
                (cardEffect.effectType == EffectType.cantrigger || cardEffect.effectType == EffectType.musttrigger))
        { // 从手卡发动的怪兽的诱发效果，尽管咒文速度是1，实际处理时当作2速
            if (duelcard.position == CardPosition.handcard && !duelcard.infopublic) cardEffect.speed = 2;
            else cardEffect.speed = 1;
        }
        // 检查效果速度
        if (cardEffect.effectType < EffectType.activate && duelData.chainEffect.Count != 0)
        {
            if (duelData.chainEffect[0].speed == 1 && cardEffect.speed == 1) return false;
            if (duelData.chainEffect[0].speed >= 2 && cardEffect.speed < duelData.chainEffect[0].speed) return false;
        }
        return true;
    }
    #endregion 对决斗的判断

    #region 目标卡
    public List<DuelCard> GetTargetCard(TargetCard targetcard)
    {
        List<DuelCard> targetlist = new List<DuelCard>();
        int player = targetcard.side == PlayerSide.own ? duelData.opWho : GetOppPlayer(duelData.opWho);
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
        foreach (KeyValuePair<int, List<object>> kv in targetcard.target)
        {
            if (kv.Key == GameCard.name)
            {
                List<string> values = ConvertUtil.ToList<string>(kv.Value);
                foreach (DuelCard duelcard in targetlist)
                {
                    if (values.Contains(duelcard.name)) tlist.Add(duelcard);
                }
            }
            if (kv.Key == GameCard.type)
            {
                List<string> values = ConvertUtil.ToList<string>(kv.Value);
                foreach (DuelCard duelcard in targetlist)
                {
                    foreach (string value in values)
                    {
                        if (duelcard.type.Contains(value))
                        {
                            tlist.Add(duelcard);
                            break;
                        }
                    }
                }
            }
        }
        if (targetcard.target.Count != 0) targetlist = tlist;

        return targetlist;
    }
    #endregion 目标卡

    #region 时点
    private void ShowTimePoint()
    {
        foreach (DuelCase duelcase in duelData.duelcase)
        {
            Debug.Log("事件" + duelcase.gameEvent + " " + duelcase.type + " 时点" + duelcase.timepoint);
        }
        Debug.Log("");
    }

    private void SetLastCaseType(string type)
    {
        List<DuelCase> duelcase = duelData.duelcase;
        for (int i = duelcase.Count - 1; i >= 0; i--)
        {
            if (duelcase[i].type == null)
            {
                duelcase[i].type = type;
            }
            else break;
        }
    }

    private void ActivateTimePointPass()
    { // 上一个发动时点过时
        List<DuelCase> duelcase = duelData.duelcase;
        for (int i = duelcase.Count - 1; i >= 0; i--)
        {
            if (duelcase[i].gameEvent == GameEvent.activateeffect || duelcase[i].gameEvent == GameEvent.activatecard)
            {
                duelcase[i].timepoint = 1;
                break;
            }
        }
    }

    private void CostTimePointPass()
    {
        List<DuelCase> duelcase = duelData.duelcase;
        for (int i = duelcase.Count - 1; i >= 0; i--)
        {
            if (duelcase[i].type == null)
            {
                duelcase[i].type = "cost";
                duelcase[i].timepoint = 1;
            }
            else break;
        }
    }

    private void AllTimePointPass()
    {
        foreach (DuelCase duelcase in duelData.duelcase)
        {
            duelcase.timepoint = 1;
        }
    }

    private void ClearPassCase()
    { // 清理过期的场合
        DuelCase duelcase = new DuelCase(GameEvent.newchain);
        duelcase.type = "chain";
        duelData.duelcase.Add(duelcase);
        List<DuelCase> caseList = duelData.duelcase;
        int i = 0;
        for (; i < caseList.Count; i++)
        {
            if (caseList[i].gameEvent == GameEvent.newchain)
            {
                if (caseList[i].timepoint == 1) break;
                else return;
            }
        }
        duelData.duelcase.RemoveRange(0, i + 1);
    }
    #endregion 时点

    #region 发动限制
    public void AddLimit(int range, DuelCard duelcard, int effect, int limitType, int max)
    {
        ActivateLimit limit;
        limit = FindLimit(duelcard, effect, limitType);
        if (limit != null) return;
        limit = new ActivateLimit
        {
            range = range,
            duelcard = duelcard,
            effect = effect,
            type = limitType,
            max = max,
            count = 0,
        };
        int player = duelcard.controller;
        duelData.activatelimit[player].Add(limit);
    }

    public void AddPubLimit(int range, int player, int limitType, int max)
    { // 不限卡的限制
        ActivateLimit limit;
        limit = FindPubLimit(player, limitType);
        if (limit != null) return;
        limit = new ActivateLimit
        {
            range = range,
            type = limitType,
            max = max,
            count = 0,
        };
        duelData.activatelimit[player].Add(limit);
    }

    public void AddUniLimit(int range, DuelCard duelcard, List<int> effects, int limitType)
    { // 多效果联合限制
        ActivateLimit limit;
        limit = FindUniLimit(duelcard, limitType);
        if (limit != null) return;
        limit = new ActivateLimit
        {
            range = range,
            duelcard = duelcard,
            effects = effects,
            type = limitType,
        };
        int player = duelcard.controller;
        duelData.activatelimit[player].Add(limit);
    }

    private ActivateLimit FindLimit(DuelCard duelcard, int effect, int limitType)
    {
        int player = duelcard.controller;
        foreach (ActivateLimit limit in duelData.activatelimit[player])
        {
            if (limit.type != limitType) continue;
            if (limit.range == 0 && limit.duelcard.Equals(duelcard))
            { // 这张卡
                if (limit.effect == effect) return limit;
            }
            if (limit.range == GameCard.name && limit.duelcard.name.Equals(duelcard.name))
            { // 这个卡名
                if (limit.effect == effect) return limit;
            }
            if (limit.range < 0)
            { // 不限卡
                if (limit.type == LimitType.specialsummonself)
                {
                    return limit;
                }
            }
        }
        return null;
    }

    private ActivateLimit FindPubLimit(int player, int limitType)
    {
        foreach (ActivateLimit limit in duelData.activatelimit[player])
        {
            if (limit.type != limitType) continue;
            if (limit.range < 0)
            { // 不限卡
                if (limit.type == LimitType.specialsummonself)
                {
                    return limit;
                }
            }
        }
        return null;
    }

    private ActivateLimit FindUniLimit(DuelCard duelcard, int limitType)
    {
        int player = duelcard.controller;
        foreach (ActivateLimit limit in duelData.activatelimit[player])
        {
            if (limit.type != limitType) continue;
            if (limit.range == 0 && limit.duelcard.Equals(duelcard))
            { // 这张卡
                return limit;
            }
            if (limit.range == GameCard.name && limit.duelcard.name.Equals(duelcard.name))
            { // 这个卡名
                return limit;
            }
        }
        return null;
    }

    private void LimitCount(DuelCard duelcard, int effect, List<EffectLimit> eLimits)
    {
        foreach (EffectLimit eLimit in eLimits)
        {
            ActivateLimit limit = FindLimit(duelcard, effect, eLimit.type);
            if (limit != null) limit.count++;
        }
    }

    public bool LimitCheck(DuelCard duelcard, int effect, int limitType)
    {
        ActivateLimit limit = FindLimit(duelcard, effect, limitType);
        if (limit != null)
        {
            if (limit.count == limit.max) return false;
        }
        return true;
    }

    private void ChainLimitReset()
    {
        for (int player = 0; player < duelData.playerNum; player++)
        {
            foreach (ActivateLimit limit in duelData.activatelimit[player])
            {
                if (limit.type == LimitType.specialsummonself && limit.count != 0)
                {
                    limit.count = 0;
                }
            }
        }
    }

    private void TurnLimitReset()
    {
        for (int player = 0; player < duelData.playerNum; player++)
        {
            foreach (ActivateLimit limit in duelData.activatelimit[player])
            {
                if (limit.type == LimitType.turnactivate && limit.count != 0)
                {
                    limit.count = 0;
                }
            }
        }
    }
    #endregion 发动限制

    #region 决斗行动记录
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
    #endregion 决斗行动记录
}
