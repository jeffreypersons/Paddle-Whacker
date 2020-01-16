using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public void NextScene(string name)
    {
        Debug.Log(name);
        SceneManager.LoadScene(name);
    }
}
