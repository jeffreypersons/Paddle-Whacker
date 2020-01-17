using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public void SwitchTo(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
