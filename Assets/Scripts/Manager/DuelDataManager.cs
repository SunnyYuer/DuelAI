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
    public int duelPeopleNum;
    public int opWhoOwn;//友方谁在操作
    public int opWhoOps;//敌方谁在操作

    public DuelDataManager(int peopleNum)
    {
        duelPeopleNum = peopleNum;
        InitialDeck();
        cardDataManager = new CardDataManager();
        turnNum = 0;
        opWhoOwn = 0;//0或2
        opWhoOps = 1;//1或3
    }

    public void InitialDeck()
    {
        deck = new List<string>[duelPeopleNum];
        extra = new List<string>[duelPeopleNum];
        handcard = new List<string>[duelPeopleNum];
        for (int i = 0; i < duelPeopleNum; i++)
        {
            deck[i] = new List<string>();
            extra[i] = new List<string>();
            handcard[i] = new List<string>();
        }
    }

    public void LoadDeckData()
    {
        for (int i = 0; i < duelPeopleNum; i++)
        {
            cardDataManager.LoadCardData(deck[i]);
            cardDataManager.LoadCardData(extra[i]);
        }
    }
}
