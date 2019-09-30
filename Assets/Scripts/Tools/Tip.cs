using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Tip : MonoBehaviour {

    public static string title = "提示";
    public static string content;
    public static int select;//玩家的选择

    // Use this for initialization
    void Start () {
        select = -1;
        GameObject.Find("MessageTitle").GetComponent<Text>().text = title;
        GameObject.Find("MessageContent").GetComponent<Text>().text = content;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnConfirmButtonClick()
    {
        select = 1;
        Destroy(gameObject);
    }

    public void OnCancelButtonClick()
    {
        select = 0;
        Destroy(gameObject);
    }

    public IEnumerator WaitForSelect()
    {
        Debug.Log("11111111");
        while (select == -1)
        {
            yield return 0;
        }
    }
}
