using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public class SplashText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Observable.Timer(TimeSpan.FromSeconds(3f))
        // .Subscribe(_=>{
        //     ExitSplash();
        //     });
    }

    public void ExitSplash()
    {
        this.gameObject.SetActive(false);
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);

    }
}
