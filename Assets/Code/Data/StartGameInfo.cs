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
    private int _numGoals;
    private Difficulty _difficulty;

    public int NumberOfGoals
    {
        get
        {
            return _numGoals;
        }
        private set
        {
            if (value <= 0)
            {
                Debug.LogError("Number of goals must be greater than zero");
            }
            else
            {
                _numGoals = NumberOfGoals;
            }
        }
    }
    public Difficulty DifficultyLevel
    {
        get
        {
            return _difficulty;
        }
        private set
        {
            if (!Enum.IsDefined(typeof(Difficulty), DifficultyLevel))
            {
                Debug.LogError("Given difficulty level enum " + DifficultyLevel + "is not defined");
            }
            else
            {
                _difficulty = DifficultyLevel;
            }
        }
    }

    public StartNewGameInfo(int numberOfGoals, Difficulty difficultyLevel)
    {
        _numGoals = numberOfGoals;
        _difficulty = difficultyLevel;
    }
    public override string ToString()
    {
        return $"NumberOfGoals is {NumberOfGoals}, and difficulty is {DifficultyLevel}";
    }
}
