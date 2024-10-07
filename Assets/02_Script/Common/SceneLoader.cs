using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Title,
    Lobby,
    InGame,
}

public class SceneLoader : Singleton<SceneLoader>
{
    public void LoadScene(SceneType sceneType)
    {
        SceneManager.LoadScene(sceneType.ToString());
    }

    public AsyncOperation LoadSceneAsync(SceneType sceneType)
    {
        return SceneManager.LoadSceneAsync(sceneType.ToString());
    }

}
