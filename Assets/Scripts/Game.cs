using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public int userIdx;
    public Transform Parent_Bar;
    public Transform Parent_Point;
    Dictionary<int, Point> all_points;
    Dictionary<int, Bar> all_bars;
    Dictionary<int, Bar> active_bars;
    Dictionary<int, Bar> static_bars;

    public RoundPanel Round_Panel;
    public Button btn_ready;
    public Button btn_cancel;

    public Text txt_winner;
    public Text txt_score;

    int myPoint = 0;
    int otherPoint = 0;

    private void Awake()
    {
        GlobalData.Instance.CurrentScene = GlobalData.Scene.Game;
        GlobalData.Instance.game = this;

        all_points = new Dictionary<int, Point>();
        all_bars = new Dictionary<int, Bar>();
        active_bars = new Dictionary<int, Bar>();
        static_bars = new Dictionary<int, Bar>();
    }
    private void Start()
    {
        //
        btn_ready.gameObject.SetActive(false);

        //
        Bar[] BarComponents = Parent_Bar.GetComponentsInChildren<Bar>();
        for (int i=0; i < BarComponents.Length; i++)
        {
            int num;
            string str;
            str = BarComponents[i].transform.name;
            int.TryParse(str, out num);

            BarComponents[i].idx = num;
            Debug.Log($"BarComponents[{i}].idx : {BarComponents[i].idx}");
            all_bars.Add(BarComponents[i].idx, BarComponents[i]);
        }

        Point[] PointComponents = Parent_Point.GetComponentsInChildren<Point>();
        for (int i=0; i < PointComponents.Length; i++)
        {
            
            int num;
            string str;
            str = PointComponents[i].transform.name;
            int.TryParse(str, out num);
            PointComponents[i].idx = num;
            all_points.Add(PointComponents[i].idx, PointComponents[i]);
        }

        CS_Game_Entry dataform;
        dataform.ph = new Head(PacketID.CS_GAME_ENTRY, 5);
        WS_Client.Instance.Send(JsonUtility.ToJson(dataform));
    }

    public void WS_OPEN()
    {
    }
    public void WS_CLOSE()
    {
        GlobalData.Instance.Popup("서버와 연결이 끊겼습니다.");
    }
    public void WS_ERROR()
    {
        GlobalData.Instance.Popup("서버와 연결이 끊겼습니다.");
    }
    public void SC_GAME_READY(SC_Game_Ready packet)
    {
        Debug.Log("ready"+packet);
        if(packet.ready == true)
        {
            btn_cancel.gameObject.SetActive(true);
            btn_ready.gameObject.SetActive(false);
        }
        else
        {
            btn_cancel.gameObject.SetActive(false);
            btn_ready.gameObject.SetActive(true);
        }

    }
    public void SC_GAME_START(SC_Game_Start packet)
    {
        userIdx = packet.userIdx;

        btn_cancel.gameObject.SetActive(false);
        btn_ready.gameObject.SetActive(false);
        
        Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => { 
            // Bar Init
            static_bars.Clear();
            foreach (var key in all_points.Keys)
            {
                all_points[key].text.text = "";
            }
            foreach (var key in all_bars.Keys)
            {
                all_bars[key].active = true;
                all_bars[key].AnimActive(true);
                all_bars[key].AnimPlay(false);
                all_bars[key].anim.Play("bar_active", -1, 0f);
                active_bars.Add(key, all_bars[key]);
            }

            // Round Start
            Round_Panel.SetRound(packet.round);
            Round_Panel.gameObject.SetActive(true);
            txt_winner.gameObject.SetActive(false);
        });
        
    }
    public void SC_GAME_COMPUTE(SC_Game_Compute packet)
    {
        if(active_bars[packet.bar].Select())
        {
            active_bars[packet.bar].AnimActive(false);
            static_bars.Add(packet.bar, active_bars[packet.bar]);
            static_bars[packet.bar].AnimPlay(true);
            active_bars.Remove(packet.bar);

            for (int i = 0; i < packet.matrixes.Length; i++)
            {
                int idx = packet.matrixes[i].row * 10 + packet.matrixes[i].col;
                if (packet.userIdx == userIdx)
                {
                    all_points[idx].SetPoint(true);
                }
                else
                {
                    all_points[idx].SetPoint(false);
                }
            }
        }

        CS_Game_Compute dataform;
        dataform.ph = new Head(PacketID.CS_GAME_COMPUTE, 5);
        WS_Client.Instance.Send(JsonUtility.ToJson(dataform));
    }
    public void SC_GAME_TURN(SC_Game_Turn packet)
    {
        if (userIdx == packet.userIdx)
        {
            // ³» ÅÏ
            foreach(var key in active_bars.Keys)
            {
                active_bars[key].AnimPlay(true);
            }
        }
        else
        {
            // »ó´ë ÅÏ
            foreach(var key in active_bars.Keys)
            {
                active_bars[key].AnimPlay(false);
            }
        }
    }
    public void SC_GAME_SELECT(SC_Game_Select packet)
    {

    }
    public void SC_GAME_RESULT(SC_Game_Result packet)
    {
        static_bars.Clear();
        active_bars.Clear();
        if(userIdx == packet.winner)
        {
            txt_winner.text = $"{packet.winner_point} : {packet.looser_point} 승리";
            myPoint++;
        }
        else
        {
            txt_winner.text = $"{packet.looser_point} : {packet.winner_point} 패배";
            otherPoint++;
        }
        txt_winner.gameObject.SetActive(true);
        txt_score.text = otherPoint.ToString() + " : " + myPoint.ToString();

        // Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => { 
        //     txt_winner.gameObject.SetActive(false);
        //     btn_ready.gameObject.SetActive(true);
        // });
        CS_Game_Result dataform;
        dataform.ph = new Head(PacketID.CS_GAME_RESULT, 5);
        WS_Client.Instance.Send(JsonUtility.ToJson(dataform));

    }
    public void SC_GAME_OUT(SC_Game_Out packet)
    {

    }
    public void SC_GAME_TIMER(SC_Game_Timer packet)
    {
        Debug.Log($"Remain Time : {packet.sec}");
    }
    public void SC_GAME_ENTRY(SC_Game_Entry packet)
    {
        CS_Game_Result dataform;
        dataform.ph = new Head(PacketID.CS_GAME_ENTRY, 5);
        WS_Client.Instance.Send(JsonUtility.ToJson(dataform));
    }
    public void SC_GAME_NEW_MATCH(SC_Game_NewMatch packet)
    {
        btn_ready.gameObject.SetActive(true);

        CS_Game_Result dataform;
        dataform.ph = new Head(PacketID.CS_GAME_NEW_MATCH, 5);
        WS_Client.Instance.Send(JsonUtility.ToJson(dataform));
    }
    public void Ready()
    {
        CS_Game_Ready dataform;
        dataform.ph = new Head(PacketID.CS_GAME_READY, 5);
        dataform.ready = true;
        WS_Client.Instance.Send(JsonUtility.ToJson(dataform));
    }
    public void Cancel()
    {
        CS_Game_Ready dataform;
        dataform.ph = new Head(PacketID.CS_GAME_READY, 5);
        dataform.ready = false;
        WS_Client.Instance.Send(JsonUtility.ToJson(dataform));
    }
    public void RoundStart()
    {
        CS_Game_Start dataform;
        dataform.ph = new Head(PacketID.CS_GAME_START, 5);
        WS_Client.Instance.Send(JsonUtility.ToJson(dataform));

        Round_Panel.gameObject.SetActive(false);
    }
}