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

    public void LPUpdate(int player, int change)
    {
        player %= 2;
        duelData.LP[player] += change;
        LPOwn.text = "LP  " + duelData.LP[player];
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
                StartCoroutine(EndPhase());
                break;
            case GamePhase.draw:
                phaseText.text = "抽卡阶段";
                duelEvent.DrawCard(0, 1);
                yield return WaitGameEvent();
                yield return EffectChain();
                StartCoroutine(EndPhase());
                break;
            case GamePhase.standby:
                phaseText.text = "准备阶段";
                StartCoroutine(EndPhase());
                break;
            case GamePhase.main1:
                phaseText.text = "主一阶段";
                ChangeBattleButtonText();
                yield return DuelAI();
                break;
            case GamePhase.battle:
                phaseText.text = "战斗阶段";
                ChangeBattleButtonText();
                yield return DuelAI();
                break;
            case GamePhase.main2:
                phaseText.text = "主二阶段";
                yield return DuelAI();
                break;
            case GamePhase.end:
                phaseText.text = "结束阶段";
                TurnEndReset();
                StartCoroutine(EndPhase());
                break;
            default:
                break;
        }
    }

    private IEnumerator EndPhase()
    {
        yield return new WaitForSeconds(1);
        duelData.duelPhase += 10;
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

    private void OnEndTurnButtonClick()
    {
        StartCoroutine(DuelPhase(GamePhase.end));
    }

    private void ChangeBattleButtonText()
    {
        Text buttonText = battleButton.GetComponentInChildren<Text>();
        if (duelData.duelPhase == GamePhase.main1) buttonText.text = "开始战斗";
        if (duelData.duelPhase == GamePhase.battle) buttonText.text = "结束战斗";
    }

    private void OnBattleButtonClick()
    {
        if (duelData.duelPhase == GamePhase.battle)
            StartCoroutine(DuelPhase(GamePhase.main2));
        if (duelData.duelPhase == GamePhase.main1)
            StartCoroutine(DuelPhase(GamePhase.battle));
    }

    private IEnumerator Game()
    {
        int intdata1;
        int intdata2;
        DuelCard duelcarddata;
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
                    {//自己抽卡
                        yield return DrawCard(player, intdata2);
                    }
                    if (intdata1 == 1)
                    {//对方抽卡
                        yield return DrawCard(GetOppPlayer(player), intdata2);
                    }
                    if (intdata1 == 2)
                    {//双方同时抽卡
                     //同时行动时，先处理回合玩家
                        player = duelData.player;
                        yield return DrawCard(player, intdata2);
                        yield return DrawCard(GetOppPlayer(player), intdata2);
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
                    if (duelcarddata.position == CardPosition.handcard)
                    {
                        SpecialSummonFromHand(duelcarddata, duelData.placeSelect, intdata1);
                    }
                    break;
                case GameEvent.changemean:
                    duelcarddata = eData.data["monstercard"] as DuelCard;
                    intdata1 = (int)eData.data["monstermean"];
                    ChangeMean(duelcarddata, intdata1);
                    break;
                case GameEvent.battle:
                    duelcarddata = eData.data["atkmonster"] as DuelCard;
                    yield return Battle(duelcarddata);
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

    private IEnumerator PayCost(CardEffect cardEffect)
    {
        duelEvent.SetThisCard(cardEffect.duelcard);
        luaCode.Run(luaCode.CostFunStr(cardEffect));
        yield return WaitGameEvent();
    }

    private IEnumerator ActivateEffect(CardEffect cardEffect)
    {
        duelEvent.SetThisCard(cardEffect.duelcard);
        luaCode.Run(luaCode.EffectFunStr(cardEffect));
        yield return WaitGameEvent();
    }

    private IEnumerator EffectChain()
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

    private void ScanEffect()
    {
        int player = duelData.opWho;
        Debug.Log("扫描效果 player=" + player);
        int i;
        for (i = 0; i < duelData.handcard[player].Count; i++)
        {
            duelEvent.SetThisCard(duelData.handcard[player][i]);
            luaCode.Run("c" + duelData.handcard[player][i].id);
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

    private IEnumerator DuelAI()
    {
        while (!duelAI.done)
        {
            duelAI.Run();
            yield return WaitGameEvent();
        }
        duelAI.done = false;
    }

    private IEnumerator DrawCard(int player, int num)
    {
        duelData.cardsJustDrawn[player].Clear();
        while (num > 0)
        {
            yield return new WaitForSeconds(0.1f);
            if (IsPlayerOwn(player)) handOwn.AddHandCardFromDeck(player);
            else handOps.AddHandCardFromDeck(player);
            DuelCard duelcard = new DuelCard
            {
                owner = player,
                controller = player,
                position = CardPosition.handcard,
                index = duelData.handcard[player].Count
            };
            duelcard.SetCard(cardDic[duelData.deck[player][0]]);
            duelData.handcard[player].Add(duelcard);
            duelData.cardsJustDrawn[player].Add(duelData.deck[player][0]);
            duelData.deck[player].RemoveAt(0);
            if (IsPlayerOwn(player)) deckOwn.DeckUpdate(player);
            else deckOps.DeckUpdate(player);
            num--;
        }
    }

    private IEnumerator SelectMonsterPlace()
    {//选择怪兽放置
        List<int> place = GetMonsterPlace();
        //由玩家选择或者AI选择
        int select = 0;
        if (IsPlayerOwn(duelData.opWho))
        {
            //yield return monserOwn.MonsterPlace(place);
            duelData.placeSelect = place[select];
        }
        else
        {
            duelData.placeSelect = place[select];
        }
        yield return null;
    }

    private int SelectMonsterMean(int gameEvent)
    {//召唤怪兽时的表示选择
        //由玩家选择或者AI选择
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
                return CardMean.faceupdef;
        }
        return 0;
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

    private void SpecialSummonFromHand(DuelCard duelcard, int place, int mean)
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

    private IEnumerator Battle(DuelCard atkmonster)
    {
        int atkplayer = atkmonster.controller; // 攻击方
        int antiplayer = GetOppPlayer(atkplayer); // 被攻击方
        // 战斗步骤
        duelData.duelPhase++;
        int target = duelAI.GetAttackTarget();
        atkmonster.battledeclare++;
        DuelCard antimonster = null;
        if (target != -1) antimonster = duelData.monster[antiplayer][target];
        DuelRecord record = new DuelRecord(PlayerAction.battle);
        record.AddCard(atkmonster);
        record.AddCard(antimonster);
        duelData.record.Add(record);
        // 伤害步骤开始时
        duelData.duelPhase++;
        // 伤害计算前
        duelData.duelPhase++;
        // 伤害计算时
        duelData.duelPhase++;
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
        // 伤害计算后
        duelData.duelPhase++;
        // 伤害步骤终了时
        duelData.duelPhase++;
        if (destroycard == 1)
        {
            DestroyCard(atkmonster, 0);
        }
        if (destroycard == 2)
        {
            DestroyCard(antimonster, 0);
        }
        if (destroycard == 3)
        {
            DestroyCard(atkmonster, 0);
            DestroyCard(antimonster, 0);
        }
        duelData.duelPhase = GamePhase.battle;
        yield return null;
    }

    private void DestroyCard(DuelCard duelcard, int way)
    {
        if (IsPlayerOwn(duelcard.controller)) monserOwn.HideMonsterCard(duelcard);
        else monserOps.HideMonsterCard(duelcard);
        duelData.monster[duelcard.controller][duelcard.index] = null;
        duelData.grave[duelcard.owner].Insert(0, duelcard.id);
        if (IsPlayerOwn(duelcard.owner)) graveOwn.GraveUpdate(duelcard.owner);
        else graveOps.GraveUpdate(duelcard.owner);
    }

    private void TurnEndReset()
    {
        int player = duelData.player;
        for (int i = 0; i < duelData.areaNum; i++)
        {
            DuelCard duelcard = duelData.monster[player][i];
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
    /* 对决斗的判断 */
}
