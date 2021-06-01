using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckOps : MonoBehaviour, IPointerClickHandler
{
    public Duel duel;
    public Image image;
    public Text deckTip;
    public Sprite UIMask;

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
        List<DuelCard> deck = duel.duelData.deck[playerOps];
        if (deck.Count > 0)
            image.sprite = Duel.spriteManager.GetCardSprite(deck[0].id, false);
        else
            image.sprite = UIMask;
        deckTip.text = "卡组" + deck.Count;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //GameObject.Find("DuelLayout(Clone)").GetComponent<DuelEvent>().DrawCard(1, PlayerSide.ops);
    }
}
