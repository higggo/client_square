using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UINavigation : MonoBehaviour
{
    public static UINavigation Instance;

    public GlobalData.Scene NextScene;
    private void Awake()
    {
        Instance = this;
    } 
    public void GoToScene(GlobalData.Scene scene)
    {
        GoToSceneAsync(scene).Forget();
    }

    private async UniTask GoToSceneAsync(GlobalData.Scene scene)
    {
        NextScene = scene;
        Scene activeScene = SceneManager.GetActiveScene();

        await SceneManager.LoadSceneAsync((int)GlobalData.Scene.Loading, LoadSceneMode.Additive);
        await SceneManager.UnloadSceneAsync(activeScene);
        await UniTask.Delay(1500);
        await SceneManager.LoadSceneAsync((int)NextScene, LoadSceneMode.Single);
    }
    public async UniTask Push()
    {
        //await UniTask.Delay(1500);
        //await SceneManager.UnloadSceneAsync((int)GlobalData.Scene.Loading);
    }
}
