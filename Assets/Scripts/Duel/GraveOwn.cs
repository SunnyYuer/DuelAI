using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraveOwn : MonoBehaviour
{
    public Image image;
    public Text gravenum;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GraveUpdate(int playerOwn)
    {
        List<DuelCard> grave = Duel.duelData.grave[playerOwn];
        if (grave.Count > 0)
            image.sprite = Duel.spriteManager.GetCardSprite(grave[0].id, false);
        else
            image.sprite = Duel.UIMask;
        gravenum.text = grave.Count.ToString();
    }
}
