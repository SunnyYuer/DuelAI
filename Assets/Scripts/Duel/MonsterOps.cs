using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterOps : MonoBehaviour
{
    public Transform monsterArea;
    public static int placeSelect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMonsterCard(int index, int position)
    {
        List<DuelCard> handcard = Duel.duelData.handcard[Duel.duelData.opWho];
        Sprite sprite = Duel.spriteManager.GetCardSprite(handcard[index].card, false);
        if(sprite == null)
            monsterArea.GetChild(position).GetComponent<Renderer>().material.mainTexture = null;
        else
            monsterArea.GetChild(position).GetComponent<Renderer>().material.mainTexture = sprite.texture;
        monsterArea.GetChild(position).gameObject.SetActive(true);
    }
}
