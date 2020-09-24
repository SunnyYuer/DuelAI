using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainInterface : MonoBehaviour
{
    public GameObject Duel;
    public GameObject DeckEditor;
    public GameObject CardMaker;
    public GameObject Setting;

    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    public void OnStartGameButtonClick()
    {
        Instantiate(Duel, GameObject.Find("Canvas").transform);
        Destroy(gameObject);
    }

    public void OnEditDeckButtonClick()
    {
        Instantiate(DeckEditor, GameObject.Find("Canvas").transform);
        GameObject.Find("FPSText(Clone)").transform.SetAsLastSibling();
    }

    public void OnDIYButtonClick()
    {
        Instantiate(CardMaker, GameObject.Find("Canvas").transform);
        GameObject.Find("FPSText(Clone)").transform.SetAsLastSibling();
    }

    public void OnSettingClick()
    {
        Instantiate(Setting, GameObject.Find("Canvas").transform);
    }

    public void OnQuitGameClick()
    {
        Application.Quit();
    }
}
