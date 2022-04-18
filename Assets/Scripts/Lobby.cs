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
    public void SC_SEARCHING_ENEMY(SC_Searching_Enemy packet)
    {
        txt_server_conn.text = $"대전 찾는 중...";
        btn_SearchEnemy.gameObject.SetActive(false);
        btn_SearchCancel.gameObject.SetActive(true);
    }

    public void SC_SEARCHING_RESULT(SC_Searching_Result packet)
    {
        txt_server_conn.text = $"곧 게임이 시작됩니다.";
        btn_SearchEnemy.gameObject.SetActive(false);
        btn_SearchCancel.gameObject.SetActive(false);

        Observable
        .Timer(TimeSpan.FromSeconds(3))
        .Subscribe(_ => {
            UINavigation.Instance.GoToScene(GlobalData.Scene.Game);
        });

    }
    public void SC_SEARCHING_CANCEL(SC_Searching_Cancel packet)
    {
        txt_server_conn.text = "서버와 연결되었습니다.";
        btn_SearchCancel.gameObject.SetActive(false);
        btn_SearchEnemy.gameObject.SetActive(true);
    }

    public void TrySearchMatch()
    {
        CS_Searching_Enemy dataform;
        dataform.ph = new Head(PacketID.CS_SEARCHING_ENEMY, 5);
        string packet = JsonUtility.ToJson(dataform);
        WS_Client.Instance.Send(packet);
    }

    public void CancelSearchMatch()
    {
        CS_Searching_Cancel dataform;
        dataform.ph = new Head(PacketID.CS_SEARCHING_CANCEL, 5);
        string packet = JsonUtility.ToJson(dataform);
        WS_Client.Instance.Send(packet);
    }
}