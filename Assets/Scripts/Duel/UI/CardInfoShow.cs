using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoShow : MonoBehaviour
{
    public DuelCard duelcard;
    public Image cardImage;
    public Text cardName;
    public Text cardAtt;
    public Text cardDes;
    public Transform cardButtonLayout;
    private int buttonIndex;
    private bool showcard;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && !showcard)
        {
            if (gameObject.activeSelf)
            {
                if (!UIHelper.RaycastUICheck(gameObject))
                {
                    gameObject.SetActive(false);
                }
            }
        }
        showcard = false;
    }

    public void SetCardInfo(DuelCard card, Sprite sprite)
    {
        duelcard = card;
        cardImage.sprite = sprite;
        cardName.text = duelcard.name + " " + duelcard.id + " " + duelcard.series;
        cardAtt.text = duelcard.type;
        if (duelcard.type.Contains(CardType.monster))
            cardAtt.text += " " + duelcard.race + " " + duelcard.attribute + " 星" + duelcard.level + " " + duelcard.atk + "/" + duelcard.def;
        cardDes.text = duelcard.describe;
        ClearCardButton();
        showcard = true;
    }

    public void SetCardButton(string text)
    {
        GameObject buttonObject = cardButtonLayout.GetChild(buttonIndex).gameObject;
        buttonObject.GetComponentInChildren<Text>().text = text;
        buttonObject.SetActive(true);
        buttonIndex++;
    }

    public void ClearCardButton()
    {
        foreach (Transform cardButton in cardButtonLayout)
        {
            cardButton.gameObject.SetActive(false);
        }
        buttonIndex = 0;
    }
}
