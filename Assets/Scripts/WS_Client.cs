using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

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
public struct DataForm
{
    public Head ph;
    public string msg;
}

public class WS_Client : MonoBehaviour
{
    WebSocket ws;
    // Start is called before the first frame update
    void Start()
    {
        ws = new WebSocket("ws://localhost:8080");
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message received from " + ((WebSocket)sender).Url + ", Data : " + e.Data);
        };
        ws.Connect();

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
            ws.Send(cs_packet);
        }
        
    }
}
