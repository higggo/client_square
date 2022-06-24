using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Lobby : MonoBehaviour
{
    public Button btn_SearchEnemy;
    public Button btn_SearchCancel;

    public Text txt_server_conn;

    private void Awake()
    {
        GlobalData.Instance.CurrentScene = GlobalData.Scene.Lobby;
        GlobalData.Instance.lobby = this;
        Debug.Log("Awake WS_Clkient");
    }
    private void Start()
    {
        WS_Client.Instance.WebSocketStart();
        Debug.Log("Start WS_Clkient");
    }
    public void WS_OPEN()
    {
        txt_server_conn.text = "서버와 연결되었습니다.";
        //Observable.Interval(TimeSpan.FromMilliseconds(3000f))
        //    .Subscribe(_ => {
        //        CS_Ping dataform;
        //        dataform.ph = new Head(PacketID.CS_PING, 5);
        //        string cs_packet = JsonUtility.ToJson(dataform);
        //        WS_Client.Instance.Send(cs_packet);
        //    });
    }
    public void WS_CLOSE()
    {
        GlobalData.Instance.Popup("서버와 연결이 끊겼습니다.");
        txt_server_conn.text = "서버와 연결이 끊겼습니다.";
    }
    public void WS_ERROR()
    {
        GlobalData.Instance.Popup("서버와 연결이 끊겼습니다.");
        txt_server_conn.text = "서버와 연결이 끊겼습니다.";
    }
}