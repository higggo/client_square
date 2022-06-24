using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bar : MonoBehaviour
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
