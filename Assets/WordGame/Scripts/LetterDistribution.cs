using UnityEngine;

public static class LetterDistribution
{
    private static readonly int[] Weights = new int[]
    {
        9, 2, 2, 4, 12, 2, 3, 2, 9, 1, 1, 4, 2, 6, 8, 2, 1, 6, 4, 6, 4, 2, 2, 1, 2, 1
    };

    private static readonly int TotalWeight;

    static LetterDistribution()
    {
        int sum = 0;
        for (int i = 0; i < Weights.Length; i++) sum += Weights[i];
        TotalWeight = sum;
    }

    public static char GetRandom()
    {
        int r = Random.Range(0, TotalWeight);
        int acc = 0;
        for (int i = 0; i < Weights.Length; i++)
        {
            acc += Weights[i];
            if (r < acc) return (char)('A' + i);
        }
        return 'E';
    }
}
