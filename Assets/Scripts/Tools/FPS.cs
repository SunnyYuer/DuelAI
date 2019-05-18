using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    private float currentTime = 0;
    private float lateTime = 0;
    private float framesNum = 0;
    private float fps = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        framesNum++;
        if (currentTime - lateTime >= 1.0f)
        {
            fps = framesNum / (currentTime - lateTime);
            gameObject.GetComponent<Text>().text = "FPS：" + fps;
            lateTime = currentTime;
            framesNum = 0;
        }
    }
}
