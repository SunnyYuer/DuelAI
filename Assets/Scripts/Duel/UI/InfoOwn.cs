using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoOwn : MonoBehaviour, IPointerClickHandler
{
    public GameObject deckLayout;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (deckLayout.activeSelf)
            {
                if (!UIHelper.RaycastUICheck(deckLayout, gameObject))
                {
                    deckLayout.SetActive(false);
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (deckLayout.activeSelf) deckLayout.SetActive(false);
        else deckLayout.SetActive(true);
    }
}
