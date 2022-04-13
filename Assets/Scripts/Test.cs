using System;
using UnityEngine;
using WebSocketSharp;
using UniRx;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;



public class Test : MonoBehaviour
{
    WebSocket ws;

    public Queue<Action> executeOnMainThread = new Queue<Action>();

    // Start is called before the first frame update
    void Start()
    {
        ws = new WebSocket("ws://localhost:8080");
        ws.OnMessage += (sender, e) =>
        {
        };
        ws.OnOpen += (sender, e) =>
        {
        };        // Add OnError event listener
        ws.OnError += (sender, e) =>
        {
        };

        // Add OnClose event listener
        ws.OnClose += (sender, e) =>
        {
        };
        ws.Connect();
    }
    // Update is called once per frame
    void Update()
    {
        if (executeOnMainThread.Count > 0)
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
