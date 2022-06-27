using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalData : MonoBehaviour
{
    public static GlobalData Instance;
    
    public Lobby lobby;
    public Game game;
    public Scene CurrentScene = Scene.Intro;
    public Camera mainCamera;
    public GameObject PopupObject;
    public Transform UI_Popup;

    public Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();

    public GameObject Camera;
    private void Awake()
    {
        Instance = this;
    }
    public enum Scene
    {
        Intro = 0,
        Lobby,
        Loading,
        Game
    }

    public void Popup(string msg, UnityAction action = null)
    {
        GameObject popup = Instantiate(PopupObject, UI_Popup);
        popup.GetComponent<Popup>().Set(msg, action);
    }
}
