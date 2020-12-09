using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckOwn : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public Text decknum;
    public Sprite UIMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeckUpdate(int playerOwn)
    {
        List<DuelCard> deck = Duel.duelData.deck[playerOwn];
        if (deck.Count > 0)
            image.sprite = Duel.spriteManager.GetCardSprite(deck[0].id, false);
        else
            image.sprite = UIMask;
        decknum.text = deck.Count.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //GameObject.Find("HandCardsLayoutOwn").GetComponent<HandCardOwn>().DrawCard();
    }
}
