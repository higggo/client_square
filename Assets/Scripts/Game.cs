using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public int userIdx;
    public Transform Parent_Bar;
    public Transform Parent_Point;
    Dictionary<int, Point> all_points;
    Dictionary<int, Bar> all_bars;
    Dictionary<int, Bar> active_bars;
    Dictionary<int, Bar> static_bars;

    private void Awake()
    {
        GlobalData.Instance.CurrentScene = GlobalData.Scene.Game;
        GlobalData.Instance.game = this;
    }
    private void Start()
    {
        Bar[] BarComponents = Parent_Bar.GetComponentsInChildren<Bar>();
        for (int i=0; i < BarComponents.Length; i++)
        {
            all_bars.Add(BarComponents[i].idx, BarComponents[i]);
        }

        Point[] PointComponents = Parent_Point.GetComponentsInChildren<Point>();
        for (int i=0; i < PointComponents.Length; i++)
        {
            all_points.Add(PointComponents[i].idx, PointComponents[i]);
        }
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
        static_bars.Clear();
        active_bars.Clear();
        active_bars = all_bars;

        all_points.Clear();
        for(int i=0; i<all_points.Count; i++)
        {
            all_points[i].text.text = "";
        }

    }
    public void SC_GAME_START(SC_Game_Start packet)
    {
        userIdx = packet.userIdx;
    }
    public void SC_GAME_COMPUTE(SC_Game_Compute packet)
    {
        active_bars[packet.bar].Select();
        active_bars.Remove(packet.bar);
        static_bars.Add(packet.bar, active_bars[packet.bar]);

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
            // 내 턴
            for (int i = 0; i < active_bars.Count; i++)
            {
                active_bars[i].AnimPlay(true);
            }
        }
        else
        {
            // 상대 턴
        }
    }
    public void SC_GAME_SELECT(SC_Game_Select packet)
    {

    }
    public void SC_GAME_RESULT(SC_Game_Result packet)
    {

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
}
