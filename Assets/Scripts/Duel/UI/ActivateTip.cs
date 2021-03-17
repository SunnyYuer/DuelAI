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
}
