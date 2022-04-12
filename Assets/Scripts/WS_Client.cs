using System;
using UnityEngine;
using WebSocketSharp;
using UniRx;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public enum PacketID
{
    CS_PING = 1033,
    CS_SEARCHING_ENEMY = 1002,
    CS_SEARCHING_RESULT = 1003,
    CS_SEARCHING_CANCEL = 1004,

    SC_PING = 3033,
    SC_SEARCHING_ENEMY = 3002,
    SC_SEARCHING_RESULT = 3003,
    SC_SEARCHING_CANCEL = 3004,
}


//
// Packet Struct //
[System.Serializable]
public struct Head
{
    
    public PacketID num;      // packet number
    public int size;

    public Head(PacketID num, int size) : this()
    {
        this.num = num;
        this.size = size;
    }
}
[System.Serializable]
public struct HeadType
{
    public Head ph;
}
[System.Serializable]
public struct DataForm
{
    public Head ph;
    public string msg;
}
[System.Serializable]
public struct CS_Ping
{
    public Head ph;
}
[System.Serializable]
public struct CS_Searching_Enemy
{
    public Head ph;
}
[System.Serializable]
public struct CS_Searching_Cancel
{
    public Head ph;
}

[System.Serializable]
public struct SC_Ping
{
    public Head ph;
}

[System.Serializable]
public struct SC_Searching_Enemy
{
    public Head ph;
}

[System.Serializable]
public struct SC_Searching_Result
{
    public Head ph;
    public int result;
}
[System.Serializable]
public struct SC_Searching_Cancel
{
    public Head ph;
}


//


//
// Main Class


public class WS_Client : MonoBehaviour
{
    public static WS_Client Instance;
    WebSocket ws;

    public Queue<Action> executeOnMainThread = new Queue<Action>();

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        ws = new WebSocket("ws://localhost:8080");
        ws.OnMessage += (sender, e) =>
        {
            string server_msg = e.Data;

            if (GlobalData.Instance.CurrentScene == GlobalData.Scene.Lobby)
            {
                switch ((PacketID)JsonUtility.FromJson<HeadType>(server_msg).ph.num)
                {
                    // Lobby
                    case PacketID.SC_PING:
                        //
                        SC_Ping sc_ping = JsonUtility.FromJson<SC_Ping>(e.Data);
                        executeOnMainThread.Enqueue(() => SC_PING(sc_ping));
                        break;
                    case PacketID.SC_SEARCHING_ENEMY:
                        SC_Searching_Enemy sc_se = JsonUtility.FromJson<SC_Searching_Enemy>(server_msg);
                        executeOnMainThread.Enqueue(() => GlobalData.Instance.lobby.SC_SEARCHING_ENEMY(sc_se));
                        break;
                    case PacketID.SC_SEARCHING_RESULT:
                        SC_Searching_Result sc_sr = JsonUtility.FromJson<SC_Searching_Result>(server_msg);
                        executeOnMainThread.Enqueue(() => GlobalData.Instance.lobby.SC_SEARCHING_RESULT(sc_sr));
                        break;
                    case PacketID.SC_SEARCHING_CANCEL:
                        SC_Searching_Cancel sc_sc = JsonUtility.FromJson<SC_Searching_Cancel>(server_msg);
                        executeOnMainThread.Enqueue(() => GlobalData.Instance.lobby.SC_SEARCHING_CANCEL(sc_sc));
                        break;
                    default:
                        break;
                }
            }
                
        };
        ws.OnOpen += (sender, e) =>
        {
            if (GlobalData.Instance.CurrentScene == GlobalData.Scene.Lobby)
            {
                executeOnMainThread.Enqueue(() => GlobalData.Instance.lobby.WS_OPEN());
            }

        };        // Add OnError event listener
        ws.OnError += (sender, e) =>
        {
            if (GlobalData.Instance.CurrentScene == GlobalData.Scene.Lobby)
            {
                executeOnMainThread.Enqueue(() => GlobalData.Instance.lobby.WS_ERROR());
            }
        };
  
        // Add OnClose event listener
        ws.OnClose += (sender, e) =>
        {
            if (GlobalData.Instance.CurrentScene == GlobalData.Scene.Lobby)
            {
                executeOnMainThread.Enqueue(() => GlobalData.Instance.lobby.WS_CLOSE());
            }
        };
        ws.Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if(executeOnMainThread.Count > 0)
        {
            executeOnMainThread.Dequeue().Invoke();
        }
        

    }
    private void OnDestroy()
    {
        ws.Close();
    }
    public void Send(string msg)
    {
        try
        {
            ws.Send(msg);
        }
        catch (System.Exception)
        {
            ws.Close();
            //txt_server_conn.text = "서버 연결에 실패했습니다.";
        }
    }

    public void SC_PING(SC_Ping packet)
    {
        
        Debug.Log("Receive Ping : ping.ph.num" + packet.ph.num);
    }
}
