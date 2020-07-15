using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardListEvent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Text resultText;
    public ScrollRect scollrect;
    private RectTransform cardlist;
    private GameObject refreshtext;
    private int cardnum;
    private int totalnum;
    private float listy;
    private float height;
    private bool isgrag = false;
    public static GameObject lastclickedcard;

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

        isgrag = true;
        if (!MakeCard.hasRefreshtext) return;
        getcardnum();
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

        isgrag = false;
        if (!MakeCard.hasRefreshtext) return;
        if ((height - listy) > (630-50)) return;
        cardnum += 1000;
        if (cardnum > totalnum) cardnum = totalnum;
        GameObject.Find("CardMakerImage(Clone)").GetComponent<MakeCard>().showcardlist(cardnum);
    }

    public void getcardnum()
    {
        string[] nums = resultText.text.Split('/');
        cardnum = int.Parse(nums[0]);
        totalnum = int.Parse(nums[1]);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isgrag) return;
        Vector2 cardclickpos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cardlist, Input.mousePosition, Camera.current, out cardclickpos);
        int cardindex = (int)(-cardclickpos.y/90);
        getcardnum();
        if (cardindex > (cardnum-1)) cardindex = cardnum - 1;

        if (lastclickedcard != null)
            lastclickedcard.GetComponent<Image>().color = Color.white;
        GameObject clickedcard = cardlist.GetChild(cardindex).gameObject;
        clickedcard.GetComponent<Image>().color = Color.gray;//设置点击卡牌后的状态颜色
        lastclickedcard = clickedcard;

        SQLManager sql = new SQLManager();
        sql.ConnectSQL();

        string cardAbstract = clickedcard.GetComponentInChildren<Text>().text;
        string[] abs = cardAbstract.Split('\n');
        string id = abs[abs.Length - 1];
        SqliteDataReader reader = sql.ReadCardsAll(Main.tableName, id);
        if (reader.Read())
        {
            string name = reader.GetString(reader.GetOrdinal("name"));
            string describe = reader.GetString(reader.GetOrdinal("describe"));
            GameObject.Find("CardNameInputField").GetComponent<InputField>().text = name;
            GameObject.Find("CardIdInputField").GetComponent<InputField>().text = id;
            Dropdown dp = GameObject.Find("Dropdown").GetComponent<Dropdown>();
            dp.value = 0;
            GameObject.Find("DescribeInputField").GetComponent<InputField>().text = describe;
        }
        reader.Close();

        sql.CloseSQLConnection();
    }
}
