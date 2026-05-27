using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class HintFinder
{
    private const int MAX_DEPTH = 8;
    private const int MIN_LENGTH = 3;

    public static List<HexCell> FindValidWord(HexGrid grid, WordValidator validator)
    {
        if (grid == null) return null;

        var wordSet = BuildPlacementWordSet();
        if (wordSet.Count == 0) return null;

        var startCells = new List<HexCell>();
        foreach (var pair in grid.Cells)
        {
            var c = pair.Value;
            if (c == null || c.IsVacant) continue;
            if (!grid.HasAnyVacantNeighbor(c.Coord)) continue;
            startCells.Add(c);
        }

        for (int s = 0; s < startCells.Count; s++)
        {
            var startCell = startCells[s];
            var path = new List<HexCell> { startCell };
            var visited = new HashSet<HexCoord> { startCell.Coord };
            var sb = new StringBuilder();
            sb.Append(startCell.Letter);

            var found = DFS(grid, validator, wordSet, path, visited, sb);
            if (found != null) return found;
        }

        return null;
    }

    private static List<HexCell> DFS(HexGrid grid, WordValidator validator, HashSet<string> wordSet,
        List<HexCell> path, HashSet<HexCoord> visited, StringBuilder sb)
    {
        int requiredMin = ComputeRequiredLength(path);
        int effectiveMin = Mathf.Max(MIN_LENGTH, requiredMin);
        string word = sb.ToString();

        if (word.Length >= effectiveMin && wordSet.Contains(word))
        {
            if (validator == null || !validator.IsAlreadyUsed(word))
                return new List<HexCell>(path);
        }

        if (path.Count >= MAX_DEPTH) return null;

        var last = path[path.Count - 1];
        foreach (var neighbor in grid.Neighbors(last.Coord))
        {
            if (neighbor == null || neighbor.IsVacant) continue;
            if (visited.Contains(neighbor.Coord)) continue;

            visited.Add(neighbor.Coord);
            path.Add(neighbor);
            sb.Append(neighbor.Letter);

            var result = DFS(grid, validator, wordSet, path, visited, sb);
            if (result != null) return result;

            visited.Remove(neighbor.Coord);
            path.RemoveAt(path.Count - 1);
            sb.Length -= 1;
        }

        return null;
    }

    private static int ComputeRequiredLength(List<HexCell> path)
    {
        int max = 0;
        for (int i = 0; i < path.Count; i++)
        {
            int m = path[i].MinWordLength;
            if (m > max) max = m;
        }
        return max;
    }

    private static HashSet<string> BuildPlacementWordSet()
    {
        var set = new HashSet<string>();
        var pool = Dictionary.AllWords;
        if (pool == null) return set;
        for (int i = 0; i < pool.Count; i++) set.Add(pool[i]);
        return set;
    }
}
