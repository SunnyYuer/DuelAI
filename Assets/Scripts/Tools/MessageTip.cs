using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageTip : MonoBehaviour
{
    public Text title;
    public Text content;
    public int select; // 玩家的选择

    // Use this for initialization
    void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void ShowTip(string titleText, string contentText)
    {
        title.text = titleText;
        content.text = contentText;
        gameObject.SetActive(true);
    }

    public void OnConfirmButtonClick()
    {
        select = 1;
        gameObject.SetActive(false);
    }

    public void OnCancelButtonClick()
    {
        select = 0;
        gameObject.SetActive(false);
    }

    public IEnumerator WaitForTipChoose()
    {
        select = -1;
        while (select == -1)
        {
            yield return null;
        }
    }
}
