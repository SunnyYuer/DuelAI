using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelDataManager
{
    public List<string>[] deck;
    public List<string>[] extra;
    public List<string>[] grave;
    public List<string>[] except;
    public List<string>[] handcard;
    public string[][] monster;
    public string[][] magictrap;
    public string[] field;
    public string[][] special;
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
        grave = new List<string>[duelPeopleNum];
        except = new List<string>[duelPeopleNum];
        handcard = new List<string>[duelPeopleNum];
        monster = new string[duelPeopleNum][];
        magictrap = new string[duelPeopleNum][];
        for (int i = 0; i < duelPeopleNum; i++)
        {
            deck[i] = new List<string>();
            extra[i] = new List<string>();
            grave[i] = new List<string>();
            except[i] = new List<string>();
            handcard[i] = new List<string>();
            monster[i] = new string[5];
            magictrap[i] = new string[5];
        }
        field = new string[2];
    }

    public void LoadDeckData()
    {
        for (int i = 0; i < duelPeopleNum; i++)
        {
            cardDataManager.LoadCardData(deck[i]);
            cardDataManager.LoadCardData(extra[i]);
        }
        string allcode = "";
        foreach (CardDataManager.Card card in cardDataManager.cardDic.Values)
        {
            allcode += card.code;
        }
        CodeCompiler codeCompiler = new CodeCompiler();
        //codeCompiler.CSharpCompile(allcode);
        codeCompiler.TestCSharpCode();
    }
}
