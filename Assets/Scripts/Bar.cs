using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bar : MonoBehaviour, IPointerClickHandler
{
    public bool active = true;
    public Animator anim;
    public bool animActive = false;
    public int idx;

    private void Start()
    {
        anim = GetComponent<Animator>();
        

        this.anim.SetBool("active", false);

        int num;
        string str;
        str = gameObject.transform.name;
        //str = str.Substring(str.Length - 2, 2);
        int.TryParse(str, out num);

        idx = num;
        Debug.Log($"{gameObject.transform.name}bar idx : {idx}");
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(active)
        {
            CS_Game_Select dataform;
            dataform.ph = new Head(PacketID.CS_GAME_SELECT, 5);
            dataform.bar = idx;
            string packet = JsonUtility.ToJson(dataform);
            WS_Client.Instance.Send(packet);
        }
    }
    void SetActive(bool active)
    {
        this.active = active;
    }
    public bool Select()
    {
        if (!active) return false;
        else
        {
            SetActive(false);
            return true;
        }
    }
    public void AnimActive(bool anim)
    {
        this.anim.SetBool("active", anim);
        animActive = anim;
    }
    public void AnimPlay(bool play)
    {
        this.anim.enabled = play;
    }
}
