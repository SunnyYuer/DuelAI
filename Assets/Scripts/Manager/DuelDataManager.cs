using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelDataManager
{
    public List<string>[] deck;
    public List<string>[] extra;
    public List<string>[] handcard;
    public CardDataManager cardDataManager;
    public int turnNum;
    public int whoTurn;
    public int duelPhase;

    public DuelDataManager()
    {
        InitialDeck();
        cardDataManager = new CardDataManager();
        turnNum = 0;
    }

    public void InitialDeck()
    {
        deck = new List<string>[4];
        extra = new List<string>[4];
        handcard = new List<string>[4];
        for (int i = 0; i < 4; i++)
        {
            deck[i] = new List<string>();
            extra[i] = new List<string>();
            handcard[i] = new List<string>();
        }
    }

    public void LoadDeckData()
    {
        for (int i = 0; i < 4; i++)
        {
            cardDataManager.LoadCardData(deck[i]);
            cardDataManager.LoadCardData(extra[i]);
        }
    }
}
