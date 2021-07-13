using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelTip : MonoBehaviour
{
    public MessageTip messageTip;
    public MeanTip meanTip;
    public int choice;
    public int selects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ShowMessageTip(string title, string content)
    {
        yield return messageTip.ShowTip(title, content);
        choice = messageTip.choice;
    }

    public IEnumerator ShowMeanTip(Sprite faceup, Sprite facedown = null)
    {
        if (facedown == null)
            yield return meanTip.ShowTip(faceup, faceup);
        else
            yield return meanTip.ShowTip(faceup, facedown);
        choice = meanTip.choice;
    }
}
