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

    public void ShowMonsterCard(int index, int position)
    {
        List<string> handcard = Duel.duelData.handcard[Duel.duelData.opWhoOwn];
        Sprite sprite = Duel.spriteManager.GetCardSprite(handcard[index], false);
        if(sprite == null)
            monsterArea.GetChild(position).GetComponent<Renderer>().material.mainTexture = null;
        else
            monsterArea.GetChild(position).GetComponent<Renderer>().material.mainTexture = sprite.texture;
        monsterArea.GetChild(position).gameObject.SetActive(true);
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
