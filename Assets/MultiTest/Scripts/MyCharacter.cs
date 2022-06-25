using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class MyCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Observable.Interval(System.TimeSpan.FromSeconds(3))
            .Subscribe(_ => {
                CS_Game_Position dataform;
                dataform.ph = new Head(PacketID.CS_GAME_POSITION, 5);
                dataform.position.x = transform.position.x;
                dataform.position.y = transform.position.y;
                dataform.position.z = transform.position.z;
                WS_Client.Instance.Send(JsonUtility.ToJson(dataform));
            })
            .AddTo(this);

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                //agent.destination = hit.point;

                CS_Game_Move dataform;
                dataform.ph = new Head(PacketID.CS_GAME_MOVE, 5);
                dataform.position.x = hit.point.x;
                dataform.position.y = hit.point.y;
                dataform.position.z = hit.point.z;
                WS_Client.Instance.Send(JsonUtility.ToJson(dataform));
            }
        }
    }
}
