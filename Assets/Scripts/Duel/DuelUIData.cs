using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuelUIData : MonoBehaviour
{
    public Duel duel;
    private DuelDataManager duelData;
    public GameObject cardLayoutOwn;
    public GameObject cardLayoutOps;
    public Text LPOwn;
    public Text LPOps;
    public DeckOwn deckOwn;
    public DeckOps deckOps;
    public GraveOwn graveOwn;
    public GraveOps graveOps;
    public HandCardOwn handOwn;
    public HandCardOps handOps;
    public GameObject endTurnButton;
    public GameObject battleButton;
    public Text phaseText;
    public DuelHint duelhint;
    public CardInfoShow cardinfo;
    public Tip tip;

    // Start is called before the first frame update
    void Start()
    {
        duelData = Duel.duelData;
    }

    // Update is called once per frame
    void Update()
    {
        PhaseButtonShow();
    }

    private void PhaseButtonShow()
    {
        if ((duelData.duelPhase == GamePhase.main1 || duelData.duelPhase == GamePhase.battle) &&
            duelData.eventDate.Count == 0 && !duelData.effectChain)
            battleButton.SetActive(true);
        else
            battleButton.SetActive(false);
        if ((duelData.duelPhase == GamePhase.main1 || duelData.duelPhase == GamePhase.battle ||
            duelData.duelPhase == GamePhase.main2) &&
            duelData.eventDate.Count == 0 && !duelData.effectChain)
            endTurnButton.SetActive(true);
        else
            endTurnButton.SetActive(false);
    }

    public void ChangeBattleButtonText(string text)
    {
        Text buttonText = battleButton.GetComponentInChildren<Text>();
        buttonText.text = text;
    }

    public void SetPhaseText(string text)
    {
        phaseText.text = text;
    }

    public void ShowCardLayout()
    {
        cardLayoutOwn.GetComponent<CanvasGroup>().alpha = 1;
        cardLayoutOwn.GetComponent<CanvasGroup>().interactable = true;
        cardLayoutOps.GetComponent<CanvasGroup>().alpha = 1;
        cardLayoutOps.GetComponent<CanvasGroup>().interactable = true;
    }

    public void HideCardLayout()
    {
        cardLayoutOwn.GetComponent<CanvasGroup>().alpha = 0;
        cardLayoutOwn.GetComponent<CanvasGroup>().interactable = false;
        cardLayoutOps.GetComponent<CanvasGroup>().alpha = 0;
        cardLayoutOps.GetComponent<CanvasGroup>().interactable = false;
    }

    public void SetHint(string hint)
    {
        duelhint.SetHint(hint);
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

    public void PutDeck()
    {
        for (int player = 0; player < duelData.playerNum; player++)
        {
            if (duel.IsPlayerOwn(player))
                deckOwn.DeckUpdate(player);
            else
                deckOps.DeckUpdate(player);
        }
    }

    public void DeckRemove(int player, DuelCard duelcard)
    {
        duelData.deck[player].Remove(duelcard);
        duelData.SortCard(duelData.deck[player]);
        if (duel.IsPlayerOwn(player)) deckOwn.DeckUpdate(player);
        else deckOps.DeckUpdate(player);
    }

    public void GraveInsert(int player, int where, DuelCard duelcard)
    {
        duelcard.position = CardPosition.grave;
        duelcard.index = where;
        duelData.grave[player].Insert(where, duelcard);
        duelData.SortCard(duelData.grave[player]);
        if (duel.IsPlayerOwn(player)) graveOwn.GraveUpdate(player);
        else graveOps.GraveUpdate(player);
    }

    public void GraveRemove(int player, DuelCard duelcard)
    {
        duelData.grave[player].Remove(duelcard);
        duelData.SortCard(duelData.grave[player]);
        if (duel.IsPlayerOwn(player)) graveOwn.GraveUpdate(player);
        else graveOps.GraveUpdate(player);
    }

    public void HandCardAdd(int player, DuelCard duelcard)
    {
        duelcard.position = CardPosition.handcard;
        duelcard.index = duelData.handcard[player].Count;
        duelData.handcard[player].Add(duelcard);
        if (duel.IsPlayerOwn(player)) handOwn.AddHandCard(duelcard);
        else handOps.AddHandCard(duelcard);
    }

    public void HandCardRemove(int player, DuelCard duelcard)
    {
        duelData.handcard[player].Remove(duelcard);
        duelData.SortCard(duelData.handcard[player]);
        if (duel.IsPlayerOwn(player)) handOwn.RemoveHandCard(duelcard.index);
        else handOps.RemoveHandCard(duelcard.index);
    }

    public void ShowHandCardInfoOwn(Transform cardtrans)
    {
        int index = handOwn.GetChildIndex(cardtrans);
        DuelCard duelcard = duelData.handcard[0][index];
        cardinfo.SetCardInfo(duelcard, Duel.spriteManager.GetCardSprite(duelcard.id, false));
        if (duel.NormalSummonCheck(duelcard)) cardinfo.SetCardButton("召唤");
        if (duel.SetMonsterCheck(duelcard)) cardinfo.SetCardButton("盖放");
        cardinfo.gameObject.SetActive(true);
    }

    public void ShowHandCardInfoOps(Transform cardtrans)
    {
        int index = handOps.GetChildIndex(cardtrans);
        DuelCard duelcard = duelData.handcard[1][index];
        cardinfo.SetCardInfo(duelcard, Duel.spriteManager.GetCardSprite(duelcard.id, false));
        cardinfo.gameObject.SetActive(true);
    }

    public void CardButtonOnClick(Text buttonText)
    {
        if (buttonText.text.Equals("召唤")) duel.duelEvent.NormalSummon(cardinfo.duelcard);
        if (buttonText.text.Equals("盖放")) duel.duelEvent.SetMonster(cardinfo.duelcard);
        cardinfo.gameObject.SetActive(false);
    }

    public IEnumerator WantActivate()
    {
        //由玩家选择或者AI选择
        if (duel.IsPlayerOwn(duelData.opWho))
        {
            tip.ShowTip("提示", "是否发动？");
            yield return tip.WaitForTipChoose();
            duelData.optionChoose = tip.select;
        }
        else
        {
            duelData.optionChoose = 1;
        }
        yield return null;
    }
}
