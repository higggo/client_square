using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour
{
    public static GlobalData Instance;
    
    public Lobby lobby;
    public Game game;
    public Scene CurrentScene = Scene.Intro;
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
}
