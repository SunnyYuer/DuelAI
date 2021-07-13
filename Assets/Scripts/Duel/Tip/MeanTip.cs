using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeanTip : MonoBehaviour
{
    public Image atkMean;
    public Image defMean;
    public int choice;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ShowTip(Sprite faceup, Sprite facedown)
    {
        atkMean.sprite = faceup;
        defMean.sprite = facedown;
        gameObject.SetActive(true);
        choice = -1;
        while (choice == -1)
        {
            yield return null;
        }
    }

    public void OnAtkMeanClick()
    {
        choice = 0;
        gameObject.SetActive(false);
    }

    public void OnDefMeanClick()
    {
        choice = 1;
        gameObject.SetActive(false);
    }
}
