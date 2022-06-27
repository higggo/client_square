using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public int userIdx;
    public GameObject pref_Character;
    public GameObject Grond;

    private void Awake()
    {
       // GlobalData.Instance.CurrentScene = GlobalData.Scene.Game;
        //GlobalData.Instance.game = this;
    }
    private void Start()
    {
    }

    public void WS_OPEN()
    {
    }
    public void WS_CLOSE()
    {
    }
    public void WS_ERROR()
    {
    }
    public void SC_GAME_TIMER(SC_Game_Timer packet)
    {
        Debug.Log($"Remain Time : {packet.sec}");
    }
    public void SC_GAME_SPECTATION(SC_Game_Spectation packet)
    {
        foreach(Character character in packet.characters)
        {
            GameObject NewCharactor = Instantiate(pref_Character, Grond.transform);
            if (packet.index == character.index)
            {
                NewCharactor.AddComponent<MyCharacter>();
                GlobalData.Instance.Camera.AddComponent<Follow>().player = NewCharactor.transform;
            }
            NewCharactor.GetComponent<CharacterInfo>().index = character.index;
            NewCharactor.transform.localPosition = new Vector3(character.position.x, character.position.y, character.position.z);

            GlobalData.Instance.Characters.Add(character.index, NewCharactor);
        }
    }
    public void SC_GAME_MOVE(SC_Game_Move packet)
    {
        GlobalData.Instance.Characters[packet.character.index].GetComponent<NavMeshAgent>().destination = new Vector3(packet.character.position.x, packet.character.position.y, packet.character.position.z);
        //GlobalData.Instance.Characters[packet.character.index].GetComponent<NavMeshAgent>().destination = new Vector3(1, 0, 1);

    }
    public void SC_GAME_OUT(SC_Game_Out packet)
    {
        Destroy(GlobalData.Instance.Characters[packet.index]);
        
        GlobalData.Instance.Characters.Remove(packet.index);
    }
    public void SC_GAME_HELLO_NEWCLIENT(SC_Game_Hello_NewClient packet)
    {
        GameObject NewCharactor = Instantiate(pref_Character, Grond.transform);
        NewCharactor.GetComponent<CharacterInfo>().index = packet.character.index;
        NewCharactor.transform.localPosition = new Vector3(packet.character.position.x, packet.character.position.y, packet.character.position.z);

        GlobalData.Instance.Characters.Add(packet.character.index, NewCharactor);
    }
}