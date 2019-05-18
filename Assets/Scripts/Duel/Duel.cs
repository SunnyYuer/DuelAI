using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duel : MonoBehaviour
{
    public GameObject mainLayout;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnQuitClick()
    {
        Destroy(gameObject);
        Instantiate(mainLayout, GameObject.Find("Canvas").transform);
    }
}
