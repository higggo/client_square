using System;
using UnityEngine;
using System.Collections.Generic;

using HybridWebSocket;
using System.Text;

public enum PacketID
{
    CS_PING = 1001,
    CS_GAME_MOVE,
    CS_GAME_POSITION,
    CS_GAME_TIMER,

    SC_PING = 3001,
    SC_GAME_SPECTATION,
    SC_GAME_MOVE,
    SC_GAME_OUT,
    SC_GAME_HELLO_NEWCLIENT,
    SC_GAME_TIMER
}

[System.Serializable]
public struct Matrix
{
    public int row;
    public int col; 
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
public struct CS_Game_Timer
{
    public Head ph;
}
[System.Serializable]
public struct CS_Game_Move
{
    public Head ph;
    public Position position;
}
[System.Serializable]
public struct CS_Game_Position
{
    public Head ph;
    public Position position;
}

[System.Serializable]
public struct SC_Ping
{
    public Head ph;
}

[System.Serializable]
public struct SC_Game_Timer
{
    public Head ph;
    public int sec;
}

[System.Serializable]
public struct SC_Game_Entry
{
    public Head ph;
}

/// <summary>
/// //////////////////////////
/// </summary>
[System.Serializable]
public struct Position
{
    public float x;
    public float y;
    public float z;
}
[System.Serializable]
public struct Character
{
    public int index;
    public Position position;
}


[System.Serializable]
public struct SC_Game_Spectation
{
    public Head ph;
    public int index;
    public Character[] characters;
}
[System.Serializable]
public struct SC_Game_Move
{
    public Head ph;
    public Character character;
}


[System.Serializable]
public struct SC_Game_Out
{
    public Head ph;
    public int index;
}

[System.Serializable]
public struct SC_Game_Hello_NewClient
{
    public Head ph;
    public Character character;
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
        Debug.Log("Awake WS_Clkient");

    }
    private void Start()
    {
        WebSocketStart();

    }

    public void WebSocketStart()
    {
        ws = WebSocketFactory.CreateInstance("ws://localhost:8080");        
        
        // Add OnOpen event listener
        ws.OnOpen += () =>
        {
            if (GlobalData.Instance.CurrentScene == GlobalData.Scene.Lobby)
            {
                executeOnMainThread.Enqueue(() => GlobalData.Instance.lobby.WS_OPEN());
            }
            else if (GlobalData.Instance.CurrentScene == GlobalData.Scene.Game)
            {
                executeOnMainThread.Enqueue(() => GlobalData.Instance.game.WS_OPEN());
            }
        };

        // Add OnMessage event listener
        ws.OnMessage += (byte[] msg) =>
        {
            Debug.Log("WS received message: " + Encoding.UTF8.GetString(msg));
            string server_msg = Encoding.UTF8.GetString(msg);
            PacketID packetID = (PacketID)JsonUtility.FromJson<HeadType>(server_msg).ph.num;

            // ping
            if (packetID == PacketID.SC_PING)
            {
                CS_Ping dataform;
                dataform.ph = new Head(PacketID.CS_PING, 5);
                string cs_packet = JsonUtility.ToJson(dataform);
                Send(cs_packet);
            }
            switch (packetID)
            {
                case PacketID.SC_GAME_SPECTATION:
                    SC_Game_Spectation sc_game_spectation = JsonUtility.FromJson<SC_Game_Spectation>(server_msg);
                    executeOnMainThread.Enqueue(() => GlobalData.Instance.game.SC_GAME_SPECTATION(sc_game_spectation));
                    break;
                case PacketID.SC_GAME_MOVE:
                    SC_Game_Move sc_game_move = JsonUtility.FromJson<SC_Game_Move>(server_msg);
                    executeOnMainThread.Enqueue(() => GlobalData.Instance.game.SC_GAME_MOVE(sc_game_move));
                    break;
                case PacketID.SC_GAME_OUT:
                    SC_Game_Out sc_game_out = JsonUtility.FromJson<SC_Game_Out>(server_msg);
                    executeOnMainThread.Enqueue(() => GlobalData.Instance.game.SC_GAME_OUT(sc_game_out));
                    break;
                case PacketID.SC_GAME_HELLO_NEWCLIENT:
                    SC_Game_Hello_NewClient sc_game_hello_newclient = JsonUtility.FromJson<SC_Game_Hello_NewClient>(server_msg);
                    executeOnMainThread.Enqueue(() => GlobalData.Instance.game.SC_GAME_HELLO_NEWCLIENT(sc_game_hello_newclient));
                    break;
            }
        };

        // Add OnError event listener
        ws.OnError += (string errMsg) =>
        {
            if (GlobalData.Instance.CurrentScene == GlobalData.Scene.Lobby)
            {
                executeOnMainThread.Enqueue(() => GlobalData.Instance.lobby.WS_ERROR());
            }
            else if (GlobalData.Instance.CurrentScene == GlobalData.Scene.Game)
            {
                executeOnMainThread.Enqueue(() => GlobalData.Instance.game.WS_ERROR());
            }
        };

        // Add OnClose event listener
        ws.OnClose += (WebSocketCloseCode code) =>
        {
            if (GlobalData.Instance.CurrentScene == GlobalData.Scene.Lobby)
            {
                executeOnMainThread.Enqueue(() => GlobalData.Instance.lobby.WS_CLOSE());
            }
            else if (GlobalData.Instance.CurrentScene == GlobalData.Scene.Game)
            {
                executeOnMainThread.Enqueue(() => GlobalData.Instance.game.WS_CLOSE());
            }
        };

        // Connect to the server
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
    void OnDestroy()
    {
        ws.Close();
    }
    public void Send(string msg)
    {
        try
        {
            ws.Send(Encoding.UTF8.GetBytes(msg));
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
