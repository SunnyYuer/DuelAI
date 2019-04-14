using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardListDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Text resultText;
    public ScrollRect scollrect;
    private RectTransform cardlist;
    private GameObject refreshtext;
    private int cardnum;
    private int totalnum;
    private float listy;
    private float height;

    // Use this for initialization
    void Start () {
        cardlist = gameObject.GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnBeginDrag(PointerEventData eventData)
    {
        scollrect.OnBeginDrag(eventData);

        if (!MakeCard.hasRefreshtext) return;
        string[] nums = resultText.text.Split('/');
        cardnum = int.Parse(nums[0]);
        totalnum = int.Parse(nums[1]);
        refreshtext = cardlist.GetChild(cardlist.childCount - 1).gameObject;
    }

    public void OnDrag(PointerEventData eventData)
    {
        scollrect.OnDrag(eventData);

        if (!MakeCard.hasRefreshtext) return;
        listy = cardlist.anchoredPosition.y;
        height = cardlist.rect.height;
        if ((height-listy) >= 630) return;
        if (((height-listy) < 630) && ((height-listy) > (630-50)))
            refreshtext.GetComponent<Text>().text = "上拉加载更多";
        if ((height-listy) <= (630-50))
            refreshtext.GetComponent<Text>().text = "释放加载更多";
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scollrect.OnEndDrag(eventData);

        if (!MakeCard.hasRefreshtext) return;
        if ((height - listy) > (630-50)) return;
        cardnum += 1000;
        if (cardnum > totalnum) cardnum = totalnum;
        GameObject.Find("CardMakerImage(Clone)").GetComponent<MakeCard>().showcardlist(cardnum);
    }
}
