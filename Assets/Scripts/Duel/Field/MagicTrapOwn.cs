using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicTrapOwn : MonoBehaviour
{
    public Transform magictrapArea;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetChildCard(int index)
    {
        return magictrapArea.GetChild(index).GetChild(0);
    }

    public Transform GetSelectParticle(int index)
    {
        return magictrapArea.GetChild(index).GetChild(1);
    }

    public void SetCover()
    {
        Sprite sprite = Duel.spriteManager.GetTextureSprite("cover");
        for (int index = 0; index < magictrapArea.childCount; index++)
        {
            if (sprite == null)
                GetChildCard(index).GetComponent<Renderer>().materials[2].mainTexture = null;
            else
                GetChildCard(index).GetComponent<Renderer>().materials[2].mainTexture = sprite.texture;
        }
    }

    public void ReSetAll()
    {
        for (int index = 0; index < magictrapArea.childCount; index++)
        {
            HideMagicTrapCard(index);
            HideSelectParticle(index);
        }
    }

    public IEnumerator ShowMagicTrapCard(DuelCard duelcard)
    {
        Transform mgttrans = GetChildCard(duelcard.index);
        Sprite sprite = Duel.spriteManager.GetCardSprite(duelcard.id, false);
        if (duelcard.mean == CardMean.faceupmgt)
        {//表侧表示
            mgttrans.localPosition = new Vector3(duelcard.index, 0.43f, -0.43f);
            mgttrans.rotation = Quaternion.Euler(0, 0, 180);
        }
        if (duelcard.mean == CardMean.facedownmgt)
        {//里侧表示
            mgttrans.rotation = Quaternion.Euler(90, 180, 0);
        }
        if (sprite == null)
            mgttrans.GetComponent<Renderer>().material.mainTexture = null;
        else
            mgttrans.GetComponent<Renderer>().material.mainTexture = sprite.texture;
        mgttrans.gameObject.SetActive(true);
        yield return null;
    }

    public IEnumerator ShowCoverCard(DuelCard duelcard)
    {
        Transform mgttrans = GetChildCard(duelcard.index);
        float deltaTime = 0;
        Vector3 position = mgttrans.position;
        position.z -= 0.43f;
        while (deltaTime < 1f)
        {
            if ((deltaTime + Time.deltaTime) < 1f)
                mgttrans.RotateAround(position, new Vector3(1, 0, 0), -90f * Time.deltaTime);
            else
                mgttrans.RotateAround(position, new Vector3(1, 0, 0), -90f * (1f - deltaTime));
            yield return null;
            deltaTime += Time.deltaTime;
        }
    }

    public void HideMagicTrapCard(int index)
    {
        Transform mgttrans = GetChildCard(index);
        mgttrans.gameObject.SetActive(false);
        mgttrans.localPosition = new Vector3(index, 0, 0);
        mgttrans.rotation = Quaternion.Euler(270, 0, 0);
    }

    public void ShowSelectParticle(List<int> place)
    {
        foreach (int index in place)
        {
            GetSelectParticle(index).gameObject.SetActive(true);
        }
    }

    public void HideSelectParticle(int index)
    {
        Transform particle = GetSelectParticle(index);
        particle.gameObject.SetActive(false);
    }
}
