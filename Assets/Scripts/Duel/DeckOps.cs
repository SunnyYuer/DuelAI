using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckOps : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public Text decknum;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeckUpdate()
    {
        if (Duel.duelData.deck[1].Count > 0)
            image.sprite = Duel.spriteManager.GetCardSprite(Duel.duelData.deck[1][0], false);
        else
            image.sprite = Duel.UIMask;
        decknum.text = Duel.duelData.deck[1].Count.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //GameObject.Find("HandCardsLayoutOps").GetComponent<HandCardOps>().DrawCard();
    }
}
