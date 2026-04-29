using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int CurrentScore { get; private set; }

    public event Action<int, int> OnScoreChanged;

    public int AddWord(string word, bool numberCellBonus)
    {
        if (string.IsNullOrEmpty(word)) return 0;
        int len = word.Length;
        int baseScore = len * (len + 5);
        int delta = numberCellBonus ? baseScore * 2 : baseScore;
        CurrentScore += delta;
        if (OnScoreChanged != null) OnScoreChanged.Invoke(CurrentScore, delta);
        return delta;
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        if (OnScoreChanged != null) OnScoreChanged.Invoke(0, 0);
    }
}
