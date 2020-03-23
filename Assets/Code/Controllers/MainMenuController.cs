using UnityEngine;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public Button startButton;
    public Button quitButton;

    public StartNewGameInfo newGameInfo;

    void Awake()
    {
        newGameInfo = new StartNewGameInfo(3, StartNewGameInfo.Difficulty.Easy);
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
        SceneUtils.LoadScene("Game", () =>
        {
            GameEventCenter.startNewGame.Trigger(newGameInfo);
        });
    }
}
