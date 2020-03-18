using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneUtils
{
    public static void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
