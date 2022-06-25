using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
public class CharacterInfo : MonoBehaviour
{
    public int index = 0;

    NavMeshAgent agent;
    public Animator Anim;
    public bool bRun;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        this.ObserveEveryValueChanged(x => bRun)
            .Subscribe(_ => {
                if (_)
                {
                    Anim.ResetTrigger("Wait");
                    Anim.SetTrigger("Run");
                    Debug.Log("Run");
                }
                else
                {
                    Anim.ResetTrigger("Run");
                    Anim.SetTrigger("Wait");
                    Debug.Log("Wait");
                }
            });
    }

    private void Update()
    {

        bRun = agent.velocity.magnitude > 0.0001f;
    }

}
