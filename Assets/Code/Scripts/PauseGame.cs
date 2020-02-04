using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    private bool isPaused;

    void Start()
    {
        isPaused = false;
    }
    public void Pause()
    {
        Time.timeScale = 0;
        isPaused = true;
    }
    public void Resume()
    {
        Time.timeScale = 1;
        isPaused = false;
    }
    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
}
