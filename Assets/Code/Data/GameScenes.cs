using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameScenes
{
    public static void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
