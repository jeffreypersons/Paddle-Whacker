using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public void SwitchTo(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ReloadGameScene()
    {
        DataManager.instance.ResetAll();
        SceneManager.LoadScene("Game");
    }
}
