using UnityEngine;


public class SoundEffectController : MonoBehaviour
{
    private AudioSource audioSource;
    private static float DEFAULT_MASTER_VOLUME = 0.5f;

    [SerializeField] private float volumeScaleWallHit    = default;
    [SerializeField] private float volumeScalePaddleHit  = default;
    [SerializeField] private float volumeScaleGoalHit    = default;
    [SerializeField] private float volumeScaleGameFinish = default;

    [SerializeField] private AudioClip wallHitSound    = default;
    [SerializeField] private AudioClip paddleHitSound  = default;
    [SerializeField] private AudioClip playerScored    = default;
    [SerializeField] private AudioClip opponentScored  = default;
    [SerializeField] private AudioClip playerWinSound  = default;
    [SerializeField] private AudioClip playerLoseSound = default;

    void Awake()
    {
        audioSource = transform.gameObject.GetComponent<AudioSource>();
        audioSource.loop        = false;
        audioSource.playOnAwake = false;
        audioSource.volume      = DEFAULT_MASTER_VOLUME;
    }

    void OnEnable()
    {
        GameEventCenter.paddleHit.AddListener(PlaySoundOnPaddleHit);
        GameEventCenter.horizontalWallHit.AddListener(PlaySoundOnWallHit);
        GameEventCenter.verticalWallHit.AddListener(PlaySoundOnWallHit);
        GameEventCenter.goalHit.AddListener(PlaySoundOnGoalHit);

        GameEventCenter.startNewGame.AddListener(SetMasterVolume);
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

        GameEventCenter.startNewGame.RemoveListener(SetMasterVolume);
        GameEventCenter.pauseGame.RemoveListener(PauseAnyActiveSoundEffects);
        GameEventCenter.resumeGame.RemoveListener(ResumeAnyActiveSoundEffects);
        GameEventCenter.winningScoreReached.RemoveListener(PlaySoundOnWinningScoreReached);
    }

    private void PauseAnyActiveSoundEffects(RecordedScore _)
    {
        audioSource.Pause();
    }
    private void ResumeAnyActiveSoundEffects(string _)
    {
        audioSource.UnPause();
    }
    private void SetMasterVolume(GameSettings gameSettings)
    {
        audioSource.volume = gameSettings.SoundVolume / 100.0f;
    }

    private void PlaySoundOnPaddleHit(string _)
    {
        audioSource.PlayOneShot(paddleHitSound, volumeScalePaddleHit);
    }
    private void PlaySoundOnWallHit(string _)
    {
        audioSource.PlayOneShot(wallHitSound, volumeScaleWallHit);
    }

    private void PlaySoundOnGoalHit(string goalName)
    {
        if (goalName.StartsWith("Left"))
        {
            audioSource.PlayOneShot(opponentScored, volumeScaleGoalHit);
        }
        else
        {
            audioSource.PlayOneShot(playerScored, volumeScaleGoalHit);
        }
    }
    private void PlaySoundOnWinningScoreReached(RecordedScore recordedScore)
    {
        if (recordedScore.IsLeftPlayerWinning())
        {
            audioSource.PlayOneShot(playerWinSound, volumeScaleGameFinish);
        }
        else
        {
            audioSource.PlayOneShot(playerLoseSound, volumeScaleGameFinish);
        }
    }
}
