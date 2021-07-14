using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardClick : MonoBehaviour, IPointerClickHandler
{
    private DuelUIManager uiManage;

    // Start is called before the first frame update
    void Start()
    {
        uiManage = GameObject.Find("DuelLayout(Clone)").GetComponent<DuelUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (transform.parent.name.Equals("HandCardsLayoutOwn"))
        {
            uiManage.ShowHandCardInfoOwn(transform);
        }
        if (transform.parent.name.Equals("HandCardsLayoutOps"))
        {
            uiManage.ShowHandCardInfoOps(transform);
        }
    }
}
