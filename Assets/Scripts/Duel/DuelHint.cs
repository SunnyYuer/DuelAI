using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuelHint : MonoBehaviour
{
    public Text hintText;
    private int hide;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetHint(string hint)
    {
        hintText.text = hint;
        gameObject.SetActive(true);
        hide++;
        Invoke("Hide", 1f);
    }

    private void Hide()
    {
        if (hide == 1)
        {
            gameObject.SetActive(false);
        }
        hide--;
    }
}
