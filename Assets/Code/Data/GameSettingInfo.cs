using UnityEngine;


public class GameSettingsInfo
{
    public int   NumberOfGoals   { get; private set; }
    public float DifficultyLevel { get; private set; }
    public float SoundVolume     { get; private set; }
    public float MusicVolume     { get; private set; }

    public override string ToString() =>
        $"NumberOfGoals is {NumberOfGoals}, and difficulty is {DifficultyLevel}%, " +
        $"SoundVolume is {SoundVolume}%, and MusicVolume is {MusicVolume}%";

    public GameSettingsInfo(int numberOfGoals, int difficultyPercent, int soundVolumePercent, int musicVolumePercent)
    {
        if (ValidPositiveInteger(numberOfGoals)    &&
            ValidatePercentage(difficultyPercent)  &&
            ValidatePercentage(soundVolumePercent) &&
            ValidatePercentage(musicVolumePercent))
        {
            NumberOfGoals   = numberOfGoals;
            DifficultyLevel = MathUtils.PercentToRatio(difficultyPercent);
            SoundVolume     = MathUtils.PercentToRatio(soundVolumePercent);
            MusicVolume     = MathUtils.PercentToRatio(musicVolumePercent);
        }
    }


    private bool ValidPositiveInteger(int value)
    {
        if (value < 0)
        {
            Debug.LogError($"`{nameof(value)}` must be an integer greater than 0, recieved {value} instead");
            return false;
        }
        return true;
    }

    private bool ValidatePercentage(int value)
    {
        if (!MathUtils.IsWithinRange(value, 0, 100))
        {
            Debug.LogError($"`{nameof(value)}` must be given as an integer percentage between 0 and 100, recieved {value} instead");
            return false;
        }
        return true;
    }
}
