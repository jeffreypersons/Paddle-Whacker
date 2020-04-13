using UnityEngine;


public class SoundEffectsController : MonoBehaviour
{
    private float volumeScale;
    private AudioSource audioSource;

    [SerializeField] private AudioClip paddleHitSound;
    [SerializeField] private AudioClip wallHitSound;

    [SerializeField] private AudioClip opponentGoalHitSound = default;
    [SerializeField] private AudioClip playerGoalHitSound   = default;

    [SerializeField] private AudioClip playerWinSound       = default;
    [SerializeField] private AudioClip playerLoseSound      = default;

    void Awake()
    {
        audioSource = transform.gameObject.GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        GameEventCenter.startNewGame.AddListener(SetVolume);

        GameEventCenter.paddleHit.AddListener(PlaySoundOnPaddleHit);
        GameEventCenter.horizontalWallHit.AddListener(PlaySoundOnWallHit);
        GameEventCenter.verticalWallHit.AddListener(PlaySoundOnWallHit);

        GameEventCenter.goalHit.AddListener(PlaySoundOnGoalHit);
        GameEventCenter.winningScoreReached.AddListener(PlaySoundOnWinningScoreReached);
    }
    void OnDisable()
    {
        GameEventCenter.startNewGame.RemoveListener(SetVolume);

        GameEventCenter.paddleHit.RemoveListener(PlaySoundOnPaddleHit);
        GameEventCenter.horizontalWallHit.RemoveListener(PlaySoundOnWallHit);
        GameEventCenter.verticalWallHit.RemoveListener(PlaySoundOnWallHit);

        GameEventCenter.goalHit.RemoveListener(PlaySoundOnGoalHit);
        GameEventCenter.winningScoreReached.RemoveListener(PlaySoundOnWinningScoreReached);
    }

    void SetVolume(GameSettings gameSettings)
    {
        volumeScale = gameSettings.SoundVolume / 100.0f;
    }
    private void PlaySoundOnPaddleHit(string _)
    {
        audioSource.PlayOneShot(paddleHitSound, volumeScale);
    }
    private void PlaySoundOnWallHit(string _)
    {
        audioSource.PlayOneShot(wallHitSound, volumeScale);
    }
    private void PlaySoundOnGoalHit(string _)
    {
        // ??
    }

    private void PlaySoundOnWinningScoreReached(RecordedScore recordedScore)
    {
        // ??
    }
}
