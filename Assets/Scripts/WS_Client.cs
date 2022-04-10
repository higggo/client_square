using System;
using UnityEngine;
using WebSocketSharp;
using UniRx;
using UnityEngine.UI;
using System.Collections.Generic;
public enum PacketID
{
    CS_PING = 1033,
    CS_SEARCHING_ENEMY = 1002,

    SC_PING = 3033,
    SC_SEARCHING_RESULT = 3003
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
public struct SC_Ping
{
    public Head ph;
}

[System.Serializable]
public struct SC_Searching_Result
{
    public Head ph;
    public int result;
}

//


//
// Main Class


public class WS_Client : MonoBehaviour
{
    WebSocket ws;

    public Text txt_server_conn;
    public Queue<Action> executeOnMainThread = new Queue<Action>();

    // Start is called before the first frame update
    void Start()
    {
            ws = new WebSocket("ws://localhost:8080");
            ws.OnMessage += (sender, e) =>
            {
                string server_msg = e.Data;
                switch ((PacketID)JsonUtility.FromJson<HeadType>(server_msg).ph.num)
                {
                    case PacketID.SC_PING:
                        //
                        SC_Ping sc_ping = JsonUtility.FromJson<SC_Ping>(e.Data);
                        executeOnMainThread.Enqueue(()=>SC_PING(sc_ping));
                        break;
                    case PacketID.SC_SEARCHING_RESULT:
                    {
                        SC_Searching_Result sc_sr = JsonUtility.FromJson<SC_Searching_Result>(server_msg);
                        executeOnMainThread.Enqueue(()=>SC_SEARCHING_RESULT(sc_sr));
                    }
                        break;
                    default:
                        break;
                }
            };
            ws.OnOpen += (sender, e) =>
            {
                txt_server_conn.text = "서버와 연결되었습니다.";
                Observable.Interval(TimeSpan.FromMilliseconds(3000f))
                    .Subscribe(_ => {
                        CS_Ping dataform;
                        dataform.ph = new Head(PacketID.CS_PING, 5);
                        string cs_packet = JsonUtility.ToJson(dataform);
                        Send(cs_packet);
                    });
            };        // Add OnError event listener
            ws.OnError += (sender, e) =>
            {
                txt_server_conn.text = "서버 연결에 실패했습니다.";
            };
  
            // Add OnClose event listener
            ws.OnClose += (sender, e) =>
            {
                txt_server_conn.text = "서버 연결에 실패했습니다.";
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
    void Send(string msg)
    {
        try
        {
            ws.Send(msg);
        }
        catch (System.Exception)
        {
            ws.Close();
            txt_server_conn.text = "서버 연결에 실패했습니다.";
        }
    }

    public void TrySearchMatch()
    {
        CS_Searching_Enemy dataform;
        dataform.ph = new Head(PacketID.CS_SEARCHING_ENEMY, 5);
        string packet = JsonUtility.ToJson(dataform);
        Send(packet);
    }

    public void SC_PING(SC_Ping packet)
    {
        
        Debug.Log("Receive Ping : ping.ph.num" + packet.ph.num);
    }

    public void SC_SEARCHING_RESULT(SC_Searching_Result packet) 
    {
        txt_server_conn.text = $"곧 게임이 시작됩니다. result : {packet.result}";
    }
}
