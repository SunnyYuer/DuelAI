using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Duel : MonoBehaviour
{
    public GameObject mainLayout;
    public GameObject deckOwn;
    public GameObject deckOps;
    public GameObject handOwn;
    public GameObject handOps;
    public static List<string> owndeck;
    public static List<string> ownextra;
    public static List<string> opsdeck;
    public static List<string> opsextra;
    public static Sprite UIMask;
    public static CardSpriteManager spriteManager;
    public GameObject endTurnButton;
    public Text phaseText;
    public GameObject mainPhaseButton;
    public static int whoTurn;
    public static int duelPhase;
    public int turnNum;

    // Start is called before the first frame update
    void Start()
    {
        owndeck = new List<string>();
        ownextra = new List<string>();
        opsdeck = new List<string>();
        opsextra = new List<string>();
        spriteManager = new CardSpriteManager();
        UIMask = GameObject.Find("DeckImageOwn").GetComponent<Image>().sprite;//保存UIMask
        //读取卡组
        ReadDeckFile();
        //放置卡组
        deckOwn.GetComponent<DeckOwn>().DeckUpdate();
        deckOps.GetComponent<DeckOps>().DeckUpdate();
        //初始化回合和阶段
        turnNum = 0;
        whoTurn = 0;
        ChangePhase(0);
        //各自起手5张卡
        StartCoroutine(DrawCardOwn(5));
        StartCoroutine(DrawCardOps(5));
        //决斗开始
    }

    // Update is called once per frame
    void Update()
    {
        
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
            owndeck.Add(strs[i]);
            opsdeck.Add(strs[i]);
            i++;
        }
        i++;
        while (!strs[i].Equals("!side"))
        {
            int rmindex = strs[i].IndexOf('#');
            if (rmindex >= 0) strs[i] = strs[i].Remove(rmindex);
            ownextra.Add(strs[i]);
            opsextra.Add(strs[i]);
            i++;
        }
    }

    public void ChangePhase(int phase)
    {
        if (phase >= 7)
        {
            phase = 0;
            whoTurn = 1 - whoTurn;
        }
        duelPhase = phase;
        PhaseButtonShow();
        if (duelPhase == 0)
        {
            if (whoTurn == 0) phaseText.text = "我的回合";
            else phaseText.text = "对方回合";
            StartCoroutine(PhaseWait());
        }
        if (duelPhase == 1)
        {
            phaseText.text = "抽卡阶段";
            if(whoTurn == 0) StartCoroutine(DrawCardOwn(1));
            else StartCoroutine(DrawCardOps(1));
            StartCoroutine(PhaseWait());
        }
        if (duelPhase == 2)
        {
            phaseText.text = "准备阶段";
            StartCoroutine(PhaseWait());
        }
        if (duelPhase == 3)
        {
            phaseText.text = "主一阶段";
        }
        if (duelPhase == 4)
        {
            phaseText.text = "战斗阶段";
        }
        if (duelPhase == 5)
        {
            phaseText.text = "主二阶段";
        }
        if (duelPhase == 6)
        {
            phaseText.text = "结束阶段";
            StartCoroutine(PhaseWait());
        }
    }

    IEnumerator PhaseWait()
    {
        yield return new WaitForSeconds(1);
        ChangePhase(++duelPhase);
    }

    public void PhaseButtonShow()
    {
        if (duelPhase >= 3 && duelPhase <= 4) mainPhaseButton.SetActive(true);
        else mainPhaseButton.SetActive(false);
        if (duelPhase >= 3 && duelPhase <= 5) endTurnButton.SetActive(true);
        else endTurnButton.SetActive(false);
    }

    public void OnEndTurnButtonClick()
    {
        ChangePhase(6);
    }

    public void OnMainPhaseButtonClick()
    {
        Text buttonText = mainPhaseButton.GetComponentInChildren<Text>();
        if (duelPhase == 4)
        {
            buttonText.text = "开始战斗";
            ChangePhase(5);
        }
        if (duelPhase == 3)
        {
            buttonText.text = "结束战斗";
            ChangePhase(4);
        }
    }

    IEnumerator DrawCardOwn(int num)
    {
        yield return 0;
        while (num > 0)
        {
            handOwn.GetComponent<HandCardOwn>().DrawCard();
            yield return new WaitForSeconds(0.1f);
            num--;
        }
    }

    IEnumerator DrawCardOps(int num)
    {
        yield return 0;
        while (num > 0)
        {
            handOps.GetComponent<HandCardOps>().DrawCard();
            yield return new WaitForSeconds(0.1f);
            num--;
        }
    }

    public void OnQuitClick()
    {
        Destroy(gameObject);
        Instantiate(mainLayout, GameObject.Find("Canvas").transform);
    }
}
