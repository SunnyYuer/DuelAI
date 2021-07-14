using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelFieldManager : MonoBehaviour
{
    public Duel duel;
    private DuelDataManager duelData;
    private MonsterOwn monserOwn;
    private MonsterOps monserOps;
    private MagicTrapOwn magictrapOwn;
    private MagicTrapOps magictrapOps;

    private void Awake()
    {
        monserOwn = GameObject.Find("MonsterAreaOwn").GetComponent<MonsterOwn>();
        monserOps = GameObject.Find("MonsterAreaOps").GetComponent<MonsterOps>();
        magictrapOwn = GameObject.Find("MagicTrapAreaOwn").GetComponent<MagicTrapOwn>();
        magictrapOps = GameObject.Find("MagicTrapAreaOps").GetComponent<MagicTrapOps>();
    }

    // Start is called before the first frame update
    void Start()
    {
        duelData = duel.duelData;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCover()
    {
        monserOwn.SetCover();
        monserOps.SetCover();
        magictrapOwn.SetCover();
        magictrapOps.SetCover();
    }

    public void ReSetAll()
    {
        monserOwn.ReSetAll();
        monserOps.ReSetAll();
        magictrapOwn.ReSetAll();
        magictrapOps.ReSetAll();
    }

    public void MonsterShow(DuelCard duelcard)
    {
        if (duel.IsPlayerOwn(duelcard.controller)) monserOwn.ShowMonsterCard(duelcard);
        else monserOps.ShowMonsterCard(duelcard);
    }

    public void MonsterRemove(DuelCard duelcard)
    {
        if (duel.IsPlayerOwn(duelcard.controller)) monserOwn.HideMonsterCard(duelcard.index);
        else monserOps.HideMonsterCard(duelcard.index);
        duelData.monster[duelcard.controller][duelcard.index] = null;
    }

    public IEnumerator WaitMonsterPlace(List<int> place)
    {
        monserOwn.ShowSelectParticle(place);
        duelData.placeSelect = -1;
        while (duelData.placeSelect == -1)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 clickposition = FieldHelper.GetClickInWorldPosition();
                duelData.placeSelect = monserOwn.GetClickArea(place, clickposition);
            }
            yield return null;
        }
        monserOwn.SelectPlace(place, duelData.placeSelect);
    }

    public void HideMonsterPlaceParticle(int index)
    {
        monserOwn.HideSelectParticle(index);
    }

    public IEnumerator MagicTrapShow(DuelCard duelcard, bool activateCover)
    {
        if (activateCover)
        {
            if (duel.IsPlayerOwn(duelcard.controller)) yield return magictrapOwn.ShowCoverCard(duelcard);
            else yield return magictrapOps.ShowCoverCard(duelcard);
        }
        else
        {
            if (duel.IsPlayerOwn(duelcard.controller)) yield return magictrapOwn.ShowMagicTrapCard(duelcard);
            else yield return magictrapOps.ShowMagicTrapCard(duelcard);
        }
    }

    public void MagicTrapRemove(DuelCard duelcard)
    {
        if (duel.IsPlayerOwn(duelcard.controller)) magictrapOwn.HideMagicTrapCard(duelcard.index);
        else magictrapOps.HideMagicTrapCard(duelcard.index);
        duelData.magictrap[duelcard.controller][duelcard.index] = null;
    }
}
