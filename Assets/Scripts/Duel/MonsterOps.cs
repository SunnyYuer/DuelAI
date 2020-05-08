using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterOps : MonoBehaviour
{
    public Transform monsterArea;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMonsterCard(DuelCard duelcard)
    {
        Transform montrans = monsterArea.GetChild(duelcard.index);
        Sprite sprite = Duel.spriteManager.GetCardSprite(duelcard.card, false);
        if (duelcard.mean == CardMean.faceupatk)
        {//表侧攻击表示
            montrans.rotation = Quaternion.Euler(90, 180, 0);
        }
        if (duelcard.mean == CardMean.faceupdef)
        {//表侧守备表示
            montrans.rotation = Quaternion.Euler(90, 180, 90);
        }
        if (duelcard.mean == CardMean.facedowndef)
        {//里侧守备表示
            montrans.rotation = Quaternion.Euler(90, 180, 90);
            sprite = Duel.spriteManager.GetTextureSprite("cover");
        }
        if (sprite == null)
            montrans.GetComponent<Renderer>().material.mainTexture = null;
        else
            montrans.GetComponent<Renderer>().material.mainTexture = sprite.texture;
        montrans.gameObject.SetActive(true);
    }

    public void HideMonsterCard(DuelCard duelcard)
    {
        Transform montrans = monsterArea.GetChild(duelcard.index);
        montrans.gameObject.SetActive(false);
    }
}
