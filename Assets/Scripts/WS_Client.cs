using System;
using UnityEngine;
using System.Collections.Generic;

using HybridWebSocket;
using System.Text;

public enum PacketID
{
    CS_PING = 1033,
    CS_SEARCHING_ENEMY = 1002,
    CS_SEARCHING_RESULT = 1003,
    CS_SEARCHING_CANCEL = 1004,
    CS_GAME_READY = 1005,
    CS_GAME_START = 1006,
    CS_GAME_COMPUTE = 1007,
    CS_GAME_TURN = 1008,
    CS_GAME_SELECT = 1009,
    CS_GAME_RESULT = 1010,
    CS_GAME_OUT = 1011,
    CS_GAME_TIMER = 1012,
    CS_GAME_ENTRY = 1013,
    

    SC_PING = 3033,
    SC_SEARCHING_ENEMY = 3002,
    SC_SEARCHING_RESULT = 3003,
    SC_SEARCHING_CANCEL = 3004,
    SC_GAME_READY = 3005,
    SC_GAME_START = 3006,
    SC_GAME_COMPUTE = 3007,
    SC_GAME_TURN = 3008,
    SC_GAME_SELECT = 3009,
    SC_GAME_RESULT = 3010,
    SC_GAME_OUT = 3011,
    SC_GAME_TIMER = 3012,
    SC_GAME_ENTRY = 3013,
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
public struct CS_Game_Ready
{
    public Head ph;
    public bool ready;
}
[System.Serializable]
public struct CS_Game_Start
{
    public Head ph;
}

[System.Serializable]
public struct CS_Game_Compute
{
    public Head ph;
}

[System.Serializable]
public struct CS_Game_Turn
{
    public Head ph;
}
[System.Serializable]
public struct CS_Game_Select
{
    public Head ph;
    public int bar;
}

[System.Serializable]
public struct CS_Game_Result
{
    public Head ph;
}

[System.Serializable]
public struct CS_Game_Out
{
    public Head ph;
    public bool gameOut;
}

[System.Serializable]
public struct CS_Game_Timer
{
    public Head ph;
}

[System.Serializable]
public struct CS_Game_Entry
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
[System.Serializable]
public struct SC_Game_Ready
{
    public Head ph;
    public bool ready;
}
[System.Serializable]
public struct SC_Game_Start
{
    public Head ph;
    public int userIdx;
    public int match;
    public int round;
}

[System.Serializable]
public struct SC_Game_Compute
{
    public Head ph;
    public int bar;
    public int userIdx;
    public Matrix[] matrixes;
}

[System.Serializable]
public struct SC_Game_Turn
{
    public Head ph;
    public int userIdx;
}

[System.Serializable]
public struct SC_Game_Select
{
    public Head ph;
}

[System.Serializable]
public struct SC_Game_Result
{
    public Head ph;
    public int winner;
    public int winner_point;
    public int looser_point;
}

[System.Serializable]
public struct SC_Game_Out
{
    public Head ph;
    public bool gameOut;
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
    // Start is called before the first frame update
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
            else if (GlobalData.Instance.CurrentScene == GlobalData.Scene.Lobby)
            {
                switch (packetID)
                {
                    // Lobby
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
            else if (GlobalData.Instance.CurrentScene == GlobalData.Scene.Game)
            {
                Debug.Log("server msg : " + server_msg);
                switch (packetID)
                {
                    case PacketID.SC_GAME_READY:
                        SC_Game_Ready sc_game_ready = JsonUtility.FromJson<SC_Game_Ready>(server_msg);
                        executeOnMainThread.Enqueue(() => GlobalData.Instance.game.SC_GAME_READY(sc_game_ready));
                        break;
                    case PacketID.SC_GAME_START:
                        SC_Game_Start sc_game_start = JsonUtility.FromJson<SC_Game_Start>(server_msg);
                        executeOnMainThread.Enqueue(() => GlobalData.Instance.game.SC_GAME_START(sc_game_start));
                        break;
                    case PacketID.SC_GAME_COMPUTE:
                        SC_Game_Compute sc_game_compute = JsonUtility.FromJson<SC_Game_Compute>(server_msg);
                        executeOnMainThread.Enqueue(() => GlobalData.Instance.game.SC_GAME_COMPUTE(sc_game_compute));
                        break;
                    case PacketID.SC_GAME_TURN:
                        SC_Game_Turn sc_game_turn = JsonUtility.FromJson<SC_Game_Turn>(server_msg);
                        executeOnMainThread.Enqueue(() => GlobalData.Instance.game.SC_GAME_TURN(sc_game_turn));
                        break;
                    case PacketID.SC_GAME_SELECT:
                        SC_Game_Select sc_game_select = JsonUtility.FromJson<SC_Game_Select>(server_msg);
                        executeOnMainThread.Enqueue(() => GlobalData.Instance.game.SC_GAME_SELECT(sc_game_select));
                        break;
                    case PacketID.SC_GAME_RESULT:
                        SC_Game_Result sc_game_result = JsonUtility.FromJson<SC_Game_Result>(server_msg);
                        executeOnMainThread.Enqueue(() => GlobalData.Instance.game.SC_GAME_RESULT(sc_game_result));
                        break;
                    case PacketID.SC_GAME_OUT:
                        SC_Game_Out sc_game_out = JsonUtility.FromJson<SC_Game_Out>(server_msg);
                        executeOnMainThread.Enqueue(() => GlobalData.Instance.game.SC_GAME_OUT(sc_game_out));
                        break;
                    case PacketID.SC_GAME_TIMER:
                        SC_Game_Timer sc_game_timer = JsonUtility.FromJson<SC_Game_Timer>(server_msg);
                        executeOnMainThread.Enqueue(() => GlobalData.Instance.game.SC_GAME_TIMER(sc_game_timer));
                        break;
                    case PacketID.SC_GAME_ENTRY:
                        SC_Game_Entry sc_game_entry = JsonUtility.FromJson<SC_Game_Entry>(server_msg);
                        executeOnMainThread.Enqueue(() => GlobalData.Instance.game.SC_GAME_ENTRY(sc_game_entry));
                        break;
                }
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
