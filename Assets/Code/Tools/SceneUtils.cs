using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class SceneUtils
{
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    public static void LoadScene(string sceneName, Action onSceneLoaded)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => { onSceneLoaded.Invoke(); };
    }
    public static void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
