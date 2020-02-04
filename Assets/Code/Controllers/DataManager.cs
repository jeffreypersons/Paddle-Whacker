using UnityEngine;

// singleton class for managing data that needs to be persisted across scenes
public class DataManager : MonoBehaviour {
    public static DataManager instance { get; set; }

    public int WINNING_SCORE;
    [HideInInspector] public int player1Score { get; set; }
    [HideInInspector] public int player2Score { get; set; }

    void Awake () {
        InitializeAsPersistentSingleton();
    }

    void Start() {
        ResetAll();
    }

    public void ResetAll()
    {
        player1Score = 0;
        player2Score = 0;
    }

    private void InitializeAsPersistentSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
