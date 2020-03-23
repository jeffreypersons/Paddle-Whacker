using System;
using UnityEngine;


public class StartNewGameInfo
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    private int _numberOfGoals;
    public int NumberOfGoals
    {
        get
        {
            return _numberOfGoals;
        }
        private set
        {
            if (value <= 0)
            {
                Debug.LogError("Number of goals must be greater than zero");
            }
            else
            {
                _numberOfGoals = value;
            }
        }
    }

    private Difficulty _difficultyLevel;
    public Difficulty DifficultyLevel
    {
        get
        {
            return _difficultyLevel;
        }
        private set
        {
            if (!Enum.IsDefined(typeof(Difficulty), value))
            {
                Debug.LogError("Given difficulty level enum " + value + "is not defined");
            }
            else
            {
                _difficultyLevel = value;
            }
        }
    }

    public StartNewGameInfo(int numberOfGoals, Difficulty difficultyLevel)
    {
        NumberOfGoals = numberOfGoals;
        DifficultyLevel = difficultyLevel;
    }
    public override string ToString()
    {
        return $"NumberOfGoals is {NumberOfGoals}, and difficulty is {DifficultyLevel}";
    }
}
