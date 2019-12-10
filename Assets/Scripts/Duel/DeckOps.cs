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

    public void DeckUpdate(int playerOps)
    {
        List<string> deck = Duel.duelData.deck[playerOps];
        if (deck.Count > 0)
            image.sprite = Duel.spriteManager.GetCardSprite(deck[0], false);
        else
            image.sprite = Duel.UIMask;
        decknum.text = deck.Count.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //GameObject.Find("HandCardsLayoutOps").GetComponent<HandCardOps>().DrawCard();
    }
}
