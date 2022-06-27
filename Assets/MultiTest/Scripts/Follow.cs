using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform player;
    public Vector3 dist = new Vector3(0, 5f, -5);

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + dist;
    }
}
