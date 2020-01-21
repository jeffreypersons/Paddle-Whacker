using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // closes unity game application (or exits session if running in editor)
    // note: any unsaved game data is lost
    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
