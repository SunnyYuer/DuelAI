using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterOwn : MonoBehaviour
{
    public Transform monsterArea;
    public static int placeSelect = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowMonsterCard(DuelCard duelcard, int position, int mean)
    {
        Sprite sprite = Duel.spriteManager.GetCardSprite(duelcard.card, false);
        Transform montrans = monsterArea.GetChild(position);
        if (mean == CardMean.faceupatk)
        {//表侧攻击表示
            montrans.rotation = Quaternion.Euler(90, 0, 0);
        }
        if (mean == CardMean.faceupdef)
        {//表侧守备表示
            montrans.rotation = Quaternion.Euler(90, 0, 90);
        }
        if (mean == CardMean.facedowndef)
        {//里侧守备表示
            montrans.rotation = Quaternion.Euler(90, 0, 90);
        }
        if (sprite == null)
            montrans.GetComponent<Renderer>().material.mainTexture = null;
        else
            montrans.GetComponent<Renderer>().material.mainTexture = sprite.texture;
        montrans.gameObject.SetActive(true);
    }

    public IEnumerator MonsterPlace(List<int> place)
    {
        /*
        foreach (int index in place)
        {
            monsterArea.GetChild(index).GetComponent<Renderer>().material = Resources.Load("Materials/place") as Material;
            monsterArea.GetChild(index).gameObject.SetActive(true);
        }
        */
        placeSelect = -1;
        while (placeSelect == -1)
        {
            yield return null;
        }
    }
}
