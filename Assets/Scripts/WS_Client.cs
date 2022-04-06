using System;
using UnityEngine;
using WebSocketSharp;
using UniRx;
using UnityEngine.UI;
enum PacketID
{
    CS_PING = 1033,
    CS_SEARCHING_ENEMY = 1002,
    SC_PING = 3033,
}


//
// Packet Struct //
[System.Serializable]
public struct Head
{
    
    public int num;      // packet number
    public int size;

    public Head(int num, int size) : this()
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

//


//
// Main Class


public class WS_Client : MonoBehaviour
{
    WebSocket ws;

    public GameObject txt_server_conn;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            ws = new WebSocket("ws://localhost:8080");
            ws.OnMessage += (sender, e) =>
            {
                switch ((PacketID)JsonUtility.FromJson<HeadType>(e.Data).ph.num)
                {
                    case PacketID.SC_PING:
                        //
                        Debug.Log("Receive Ping : " + e.Data);
                        SC_Ping ping = JsonUtility.FromJson<SC_Ping>(e.Data);
                        break;
                    default:
                        break;
                }
            };
            ws.OnOpen += (sender, e) =>
            {
                txt_server_conn.GetComponent<Text>().text = "서버와 연결되었습니다.";

                Observable.Interval(TimeSpan.FromMilliseconds(3000f))
                    .Subscribe(_ => {
                        CS_Ping dataform;
                        dataform.ph = new Head((int)PacketID.CS_PING, 5);
                        string cs_packet = JsonUtility.ToJson(dataform);
                        Debug.Log("Send PING : " + cs_packet);
                        Send(cs_packet);
                    });
            };        // Add OnError event listener
            ws.OnError += (sender, e) =>
            {
                txt_server_conn.GetComponent<Text>().text = "서버 연결에 실패했습니다.";
            };
  
            // Add OnClose event listener
            ws.OnClose += (sender, e) =>
            {
                txt_server_conn.GetComponent<Text>().text = "서버 연결에 실패했습니다.";
            };
            ws.Connect();

        }
        catch (System.Exception)
        {
            Debug.Log("Socket error !");
            txt_server_conn.GetComponent<Text>().text = "서버 연결에 실패했습니다.";
            throw;
        } 
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ws == null)
        {
            return;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            DataForm dataform;
            dataform.ph = new Head(1, 5);
            dataform.msg = "asdfsdf";
            string cs_packet = JsonUtility.ToJson(dataform);
            Send(cs_packet);
        }
        
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
            txt_server_conn.GetComponent<Text>().text = "서버 연결에 실패했습니다.";
        }
    }
}
