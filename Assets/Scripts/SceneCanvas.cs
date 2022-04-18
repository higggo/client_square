using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCanvas : MonoBehaviour
{
    private void Awake()
    {
        Canvas myCanvas = gameObject.GetComponent<Canvas>();

        if(myCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            myCanvas.worldCamera = Camera.main;
        }
    }
}
