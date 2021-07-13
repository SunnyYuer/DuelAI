using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageTip : MonoBehaviour
{
    public Text title;
    public Text content;
    public int choice;

    // Use this for initialization
    void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public IEnumerator ShowTip(string titleText, string contentText)
    {
        title.text = titleText;
        content.text = contentText;
        gameObject.SetActive(true);
        choice = -1;
        while (choice == -1)
        {
            yield return null;
        }
    }

    public void OnConfirmButtonClick()
    {
        choice = 1;
        gameObject.SetActive(false);
    }

    public void OnCancelButtonClick()
    {
        choice = 0;
        gameObject.SetActive(false);
    }
}
