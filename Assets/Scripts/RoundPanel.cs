using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundPanel : MonoBehaviour
{
    int round = 0;
    public Text txt_round;
    public void SetRound(int round)
    {
        this.round = round;
        txt_round.text = round + " 라운드";
    }
    public void Ready()
    {
        GlobalData.Instance.game.RoundStart();
    }
}
