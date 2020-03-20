using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public Button startButton;
    public Button quitButton;

    void Awake()
    {
        // we always want the main menu loaded
        DontDestroyOnLoad(mainMenu);
        DontDestroyOnLoad(this);
        startButton = startButton.GetComponent<Button>();
        quitButton  = quitButton.GetComponent<Button>();
    }

    void OnDestroy()
    {

    }
    void OnEnable()
    {
        startButton.onClick.AddListener(TriggerLoadMainMenuSceneEvent);
        quitButton.onClick.AddListener(SceneUtils.QuitGame);
    }
    void OnDisable()
    {
        startButton.onClick.RemoveListener(TriggerLoadMainMenuSceneEvent);
        quitButton.onClick.RemoveListener(SceneUtils.QuitGame);
    }

    public void TriggerLoadMainMenuSceneEvent()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("MAIN");
        GameEventCenter.startNewGame.Trigger(new StartNewGameInfo(10, StartNewGameInfo.Difficulty.Easy));
        mainMenu.SetActive(false);
    }
}
