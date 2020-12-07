using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuelUI : MonoBehaviour
{
    public Duel duel;
    private DuelDataManager duelData;
    public Text LPOwn;
    public Text LPOps;

    // Start is called before the first frame update
    void Start()
    {
        duelData = Duel.duelData;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LPUpdate(int player, int change)
    {
        player %= 2;
        duelData.LP[player] += change;
        if (player == 0)
            LPOwn.text = "LP  " + duelData.LP[player];
        else
            LPOps.text = "LP  " + duelData.LP[player];
    }
}
