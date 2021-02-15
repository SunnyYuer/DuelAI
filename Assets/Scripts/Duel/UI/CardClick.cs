using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardClick : MonoBehaviour, IPointerClickHandler
{
    private DuelUIData uiData;

    // Start is called before the first frame update
    void Start()
    {
        uiData = GameObject.Find("DuelLayout(Clone)").GetComponent<DuelUIData>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (transform.parent.name.Equals("HandCardsLayoutOwn"))
        {
            uiData.ShowHandCardInfoOwn(transform);
        }
        if (transform.parent.name.Equals("HandCardsLayoutOps"))
        {
            uiData.ShowHandCardInfoOps(transform);
        }
    }
}
