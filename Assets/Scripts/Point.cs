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
            text.text = "����";
        }
        else
        {
            text.text = "���沨";
        }
    }
}
