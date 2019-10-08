using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterOwn : MonoBehaviour
{
    public Transform monsterArea;

    private int placeSelect = -2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (placeSelect == -1)
        {
            StartCoroutine(Wait());
            placeSelect = 2;
        }
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

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(5);
    }

    public int MonsterPlace(List<int> place)
    {
        foreach (int index in place)
        {
            monsterArea.GetChild(index).GetComponent<Renderer>().material = Resources.Load("Materials/place") as Material;
            monsterArea.GetChild(index).gameObject.SetActive(true);
        }
        placeSelect = -1;
        while (placeSelect == -1)
        {
            Thread.Sleep(20);
        }
        return placeSelect;
    }
}
