using System.Collections;
using System.Collections.Generic;
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

    public Button btn_ready;
    public Button btn_cancel;

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


        active_bars = all_bars;
    }

    public void WS_OPEN()
    {
    }
    public void WS_CLOSE()
    {
    }
    public void WS_ERROR()
    {
    }
    public void SC_GAME_READY(SC_Game_Ready packet)
    {
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


        CS_Game_Start dataform;
        dataform.ph = new Head(PacketID.CS_GAME_START, 5);
        WS_Client.Instance.Send(JsonUtility.ToJson(dataform));
    }
    public void SC_GAME_COMPUTE(SC_Game_Compute packet)
    {
        active_bars[packet.bar].Select();
        static_bars.Add(packet.bar, active_bars[packet.bar]);
        active_bars.Remove(packet.bar);

        for(int i=0; i<packet.matrixes.Length; i++)
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
        active_bars = all_bars;

        all_points.Clear();
        foreach(var key in all_points.Keys)
        {
            all_points[key].text.text = "";
        }
    }
    public void SC_GAME_OUT(SC_Game_Out packet)
    {

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
}