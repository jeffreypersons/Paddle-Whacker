using UnityEngine;


public class GameSettings
{
    public int NumberOfGoals   { get; private set; }
    public int DifficultyLevel { get; private set; }
    public int SoundVolume     { get; private set; }
    public int MusicVolume     { get; private set; }
    public override string ToString()
    {
        return $"NumberOfGoals is {NumberOfGoals}, and difficulty is {DifficultyLevel}%, " +
               $"SoundVolume is {SoundVolume}%, and MusicVolume is {MusicVolume}%";
    }

    public GameSettings(int numberOfGoals, int difficultyLevel, int soundVolume, int musicVolume)
    {
        if (ValidPositiveInteger(numberOfGoals) &&
            ValidatePercentage(difficultyLevel) &&
            ValidatePercentage(soundVolume)     &&
            ValidatePercentage(musicVolume))
        {
            NumberOfGoals   = numberOfGoals;
            DifficultyLevel = difficultyLevel;
            SoundVolume     = soundVolume;
            MusicVolume     = musicVolume;
        }
    }

    private bool ValidPositiveInteger(int value)
    {
        if (value < 0)
        {
            Debug.LogError($"`{nameof(value)}` must be greater than 0");
            return false;
        }
        return true;
    }
    private bool ValidatePercentage(int value)
    {
        if (value < 0 || value > 100)
        {
            Debug.LogError($"`{nameof(value)}` must be between 0 and 100%");
            return false;
        }
        return true;
    }
}
