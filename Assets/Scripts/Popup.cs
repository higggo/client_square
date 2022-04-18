using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public static Popup Instance;

    public Text msg;
    public Button btn;

    public void Set(string msg, UnityAction action = null)
    {
        this.msg.text = msg;
        if(action != null) btn.onClick.AddListener(action);
        btn.onClick.AddListener(()=> { Destroy(gameObject); });
    }
}
