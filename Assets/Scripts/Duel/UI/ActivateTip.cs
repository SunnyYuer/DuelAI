using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateTip : MonoBehaviour
{
    public Text activateTip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActivateTip(string tip)
    {
        activateTip.text = tip;
        if (tip.Length < 15) activateTip.alignment = TextAnchor.MiddleCenter;
        else activateTip.alignment = TextAnchor.MiddleLeft;
    }

    public string WhoText(int who)
    {
        if (who == 0 || who == 2)
            return "我方";
        else
            return "对方";
    }

    public string DrawCardText(int who, int num)
    {
        return WhoText(who) + "抽了" + num + "张卡";
    }

    public string ActivateText(CardEffect cardEffect)
    {
        DuelCard duelcard = cardEffect.duelcard;
        return WhoText(duelcard.controller) + "发动「" + duelcard.name + "」的效果" + cardEffect.effect;
    }
}
