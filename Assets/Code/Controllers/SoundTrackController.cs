using UnityEngine;


public class SoundTrackController : MonoBehaviour
{
    private AudioSource track;
    private static float DEFAULT_VOLUME = 0.50f;

    void Awake()
    {
        track = transform.gameObject.GetComponent<AudioSource>();
        track.loop        = true;
        track.playOnAwake = false;
        track.volume      = DEFAULT_VOLUME;
    }

    void OnEnable()
    {
        GameEventCenter.startNewGame.AddListener(StartTrack);
        GameEventCenter.restartGame.AddListener(RestartTrack);
        GameEventCenter.pauseGame.AddListener(PauseTrack);
        GameEventCenter.resumeGame.AddListener(ResumeTrack);
        GameEventCenter.winningScoreReached.AddListener(EndTrack);
    }
    void OnDisable()
    {
        GameEventCenter.startNewGame.RemoveListener(StartTrack);
        GameEventCenter.restartGame.RemoveListener(RestartTrack);
        GameEventCenter.pauseGame.RemoveListener(PauseTrack);
        GameEventCenter.resumeGame.RemoveListener(ResumeTrack);
        GameEventCenter.winningScoreReached.RemoveListener(EndTrack);
    }

    private void StartTrack(GameSettings gameSettings)
    {
        track.volume = gameSettings.MusicVolume / 100.0f;
        track.Play();
    }
    private void RestartTrack(string _)
    {
        track.Stop();
        track.Play();
    }
    private void PauseTrack(RecordedScore _)
    {
        track.Pause();
    }
    private void ResumeTrack(string _)
    {
        track.UnPause();
    }
    private void EndTrack(RecordedScore _)
    {
        track.Stop();
    }
}
