using System.Collections.Generic;
using UnityEngine;

public static class NumberCellPlacer
{
    public static Dictionary<HexCoord, int> Place(List<List<HexCoord>> placedPaths, int desiredCount, int minNumber)
    {
        var cellMinLen = new Dictionary<HexCoord, int>();

        for (int i = 0; i < placedPaths.Count; i++)
        {
            var path = placedPaths[i];
            if (path == null || path.Count == 0) continue;
            var start = path[0];
            int len = path.Count;

            int currentMin;
            if (!cellMinLen.TryGetValue(start, out currentMin) || len < currentMin)
                cellMinLen[start] = len;
        }

        var candidates = new List<HexCoord>();
        foreach (var pair in cellMinLen)
        {
            if (pair.Value >= minNumber) candidates.Add(pair.Key);
        }

        Shuffle(candidates);

        var result = new Dictionary<HexCoord, int>();
        int count = Mathf.Min(desiredCount, candidates.Count);
        for (int i = 0; i < count; i++)
        {
            result[candidates[i]] = cellMinLen[candidates[i]];
        }
        return result;
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
}
