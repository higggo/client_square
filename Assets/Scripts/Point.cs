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
        int num;
        string str;
        str = gameObject.transform.name;
        str = str.Substring(str.Length - 2, 2);
        int.TryParse(str, out num);
        idx = num;
    }

    public void SetPoint(bool mine)
    {
        if(mine)
        {
            text.text = "³»²¨";
        }
        else
        {
            text.text = "»ó´ë¹æ²¨";
        }
    }
}
