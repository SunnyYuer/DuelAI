using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoShow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCardInfo(DuelCard duelcard, Sprite sprite)
    {
        transform.Find("CardImage").GetComponent<Image>().sprite = sprite;
        transform.Find("CardNameText").GetComponent<Text>().text = duelcard.name + " " + duelcard.id + " " + duelcard.series;
        transform.Find("CardAttText").GetComponent<Text>().text = duelcard.type;
        if (duelcard.type.Contains(CardType.monster))
            transform.Find("CardAttText").GetComponent<Text>().text += " " + duelcard.race + " " + duelcard.attribute + " 星" + duelcard.level + " " + duelcard.atk + "/" + duelcard.def;
        transform.Find("CardDesText").GetComponent<Text>().text = duelcard.describe;
    }
}
