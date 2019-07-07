using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        LoadSetting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSetting()
    {
        int setting = PlayerPrefs.GetInt("shadow", 1);
        if (setting == 1) toggle.isOn = true;
        else toggle.isOn = false;
    }

    public void OnShadowToggle(bool onoff)
    {
        if (onoff && PlayerPrefs.GetInt("shadow", 1)==0)
        {
            GameObject.Find("Light").GetComponent<Light>().shadows = LightShadows.Soft;
            PlayerPrefs.SetInt("shadow", 1);
        }
        if(!onoff && PlayerPrefs.GetInt("shadow", 1)==1)
        {
            GameObject.Find("Light").GetComponent<Light>().shadows = LightShadows.None;
            PlayerPrefs.SetInt("shadow", 0);
        }
    }

    public void OnCloseClick()
    {
        Destroy(gameObject);
    }
}
