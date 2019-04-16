using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour {

    private Image progressImage;
    private Text progressText;
    public static int progress = 0;//当前进度
    public static int overallpro = 1;//总进度

	// Use this for initialization
	void Start ()
    {
        progressImage = GameObject.Find("ProgressImage").GetComponent<Image>();
        progressText = GameObject.Find("ProgressText").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        progressImage.fillAmount = 1f * progress / overallpro;
        progressText.text = progress + "/" + overallpro;
        if (progress >= overallpro)
        {
            Destroy(gameObject);
        }
    }
}
