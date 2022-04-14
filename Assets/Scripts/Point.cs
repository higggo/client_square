using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Point : MonoBehaviour
{
    public Text text;
    public int idx;
    private void Awake()
    {
        text = gameObject.GetComponentInChildren<Text>();
    }

    public void SetPoint(bool mine)
    {
        if(mine)
        {
            text.text = "내꺼";
        }
        else
        {
            text.text = "상대꺼";
        }
    }
}
