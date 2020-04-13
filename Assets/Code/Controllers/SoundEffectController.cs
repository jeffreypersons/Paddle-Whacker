using UnityEngine;


public class SoundEffectController : MonoBehaviour
{
    private float volumeScale;
    private AudioSource audioSource;
    private static float DEFAULT_VOLUME = 0.50f;

    [SerializeField] private AudioClip paddleHitSound;
    [SerializeField] private AudioClip wallHitSound;

    [SerializeField] private AudioClip opponentGoalHitSound = default;
    [SerializeField] private AudioClip playerGoalHitSound   = default;

    [SerializeField] private AudioClip playerWinSound       = default;
    [SerializeField] private AudioClip playerLoseSound      = default;

    void Awake()
    {
        audioSource = transform.gameObject.GetComponent<AudioSource>();
        audioSource.loop        = false;
        audioSource.playOnAwake = false;
        audioSource.volume      = DEFAULT_VOLUME;
    }

    void OnEnable()
    {
        GameEventCenter.paddleHit.AddListener(PlaySoundOnPaddleHit);
        GameEventCenter.horizontalWallHit.AddListener(PlaySoundOnWallHit);
        GameEventCenter.verticalWallHit.AddListener(PlaySoundOnWallHit);
        GameEventCenter.goalHit.AddListener(PlaySoundOnGoalHit);

        GameEventCenter.startNewGame.AddListener(SetVolume);
        GameEventCenter.pauseGame.AddListener(PauseAnyActiveSoundEffects);
        GameEventCenter.resumeGame.AddListener(ResumeAnyActiveSoundEffects);
        GameEventCenter.winningScoreReached.AddListener(PlaySoundOnWinningScoreReached);
    }
    void OnDisable()
    {
        GameEventCenter.paddleHit.RemoveListener(PlaySoundOnPaddleHit);
        GameEventCenter.horizontalWallHit.RemoveListener(PlaySoundOnWallHit);
        GameEventCenter.verticalWallHit.RemoveListener(PlaySoundOnWallHit);
        GameEventCenter.goalHit.RemoveListener(PlaySoundOnGoalHit);

        GameEventCenter.startNewGame.RemoveListener(SetVolume);
        GameEventCenter.pauseGame.RemoveListener(PauseAnyActiveSoundEffects);
        GameEventCenter.resumeGame.RemoveListener(ResumeAnyActiveSoundEffects);
        GameEventCenter.winningScoreReached.RemoveListener(PlaySoundOnWinningScoreReached);
    }

    void PauseAnyActiveSoundEffects(RecordedScore _)
    {
        audioSource.Pause();
    }
    void ResumeAnyActiveSoundEffects(string _)
    {
        audioSource.UnPause();
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
