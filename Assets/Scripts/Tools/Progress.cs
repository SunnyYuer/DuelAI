using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour {

    private Image progressImage;
    private Text progressTitle;
    private Text progressText;
    public static string title = "更新资源";
    public static int overallpro = 1;//总进度
    public static int progress = 0;//当前进度
    public static int destroy = 0;//是否销毁进度条

    // Use this for initialization
    void Start ()
    {
        progressImage = GameObject.Find("ProgressImage").GetComponent<Image>();
        progressTitle = GameObject.Find("ProgressTitle").GetComponent<Text>();
        progressText = GameObject.Find("ProgressText").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        progressTitle.text = title;
        progressImage.fillAmount = 1f * progress / overallpro;
        progressText.text = progress + "/" + overallpro;
        if (progress >= overallpro && destroy == 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        destroy = 0;
    }
}
