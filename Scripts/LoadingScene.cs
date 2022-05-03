using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public GameObject loadingScene;
    // Start is called before the first frame update
    void Start()
    {
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(1);
        float loadProgress = loadingOperation.progress;
        if (loadingOperation.isDone)
        {
            loadingScene.SetActive(false);
        }
    }

}
