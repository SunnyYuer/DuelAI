﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCardOwn : MonoBehaviour
{
    public RectTransform handcardlist;
    public GameObject card;
    private float listwidth;
    private float listheight;
    private float cardwidth;

    // Start is called before the first frame update
    void Start()
    {
        listwidth = handcardlist.rect.width;
        listheight = handcardlist.rect.height;
        cardwidth = card.GetComponent<RectTransform>().rect.width;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GetChildIndex(Transform cardtrans)
    {
        int index;
        for (index = 0; index < handcardlist.childCount; index++)
        {
            if (cardtrans == handcardlist.GetChild(index))
                break;
        }
        return index;
    }

    public void AddHandCard(DuelCard duelcard)
    {
        GameObject handcard = Instantiate(card, handcardlist);
        handcard.GetComponent<Image>().sprite = Duel.spriteManager.GetCardSprite(duelcard.id, false);
        StartCoroutine(ChangeHandCardPosition(false));
    }

    public void RemoveHandCard(int index)
    {
        Destroy(handcardlist.GetChild(index).gameObject);
        StartCoroutine(ChangeHandCardPosition(true));
    }

    public IEnumerator ChangeHandCardPosition(bool wait)
    {
        if (wait) yield return new WaitForSeconds(0.1f);
        Vector3 vector = new Vector3(0, listheight / 2, 0);
        int cardnum = handcardlist.childCount;
        if (cardnum <= 6)
        {
            for (int i = 0; i < cardnum; i++)
            {
                vector.x = (-cardnum + 1 + 2 * i) * ((cardwidth / 2f) + 5);
                handcardlist.GetChild(i).localPosition = vector;
            }
        }
        else
        {
            float dis = (listwidth - cardwidth) / (cardnum - 1);
            vector.x = -(listwidth - cardwidth) / 2f - dis;
            for (int i = 0; i < cardnum; i++)
            {
                vector.x += dis;
                handcardlist.GetChild(i).localPosition = vector;
            }
        }
    }

    public void SetOutline(int index)
    {
        handcardlist.GetChild(index).GetChild(0).gameObject.SetActive(true);
    }

    public void CutOutline(int index)
    {
        handcardlist.GetChild(index).GetChild(0).gameObject.SetActive(false);
    }
}
