using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class DeadlockDetector
{
    public static bool HasAnyValidWord(HexGrid grid, WordValidator validator)
    {
        if (grid == null) return false;

        var startCells = new List<HexCell>();
        foreach (var pair in grid.Cells)
        {
            var cell = pair.Value;
            if (cell.IsVacant) continue;
            if (!grid.HasAnyVacantNeighbor(cell.Coord)) continue;
            startCells.Add(cell);
        }

        var allWords = Dictionary.AllWords;
        if (allWords == null || allWords.Count == 0) return false;

        var byFirstLetter = new Dictionary<char, List<string>>();
        for (int i = 0; i < allWords.Count; i++)
        {
            var w = allWords[i];
            if (w.Length == 0) continue;
            char c = w[0];
            List<string> list;
            if (!byFirstLetter.TryGetValue(c, out list))
            {
                list = new List<string>();
                byFirstLetter[c] = list;
            }
            list.Add(w);
        }

        var path = new List<HexCell>();
        var visited = new HashSet<HexCoord>();

        for (int i = 0; i < startCells.Count; i++)
        {
            var start = startCells[i];
            List<string> candidates;
            if (!byFirstLetter.TryGetValue(start.Letter, out candidates)) continue;

            int requiredMin = start.MinWordLength;

            for (int w = 0; w < candidates.Count; w++)
            {
                var word = candidates[w];
                if (word.Length < requiredMin) continue;
                if (validator != null && validator.IsAlreadyUsed(word)) continue;

                path.Clear();
                visited.Clear();
                if (CanFormFrom(start, word, 0, grid, path, visited, requiredMin))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool CanFormFrom(HexCell cell, string word, int idx, HexGrid grid, List<HexCell> path, HashSet<HexCoord> visited, int initialMinLen)
    {
        if (cell.IsVacant) return false;
        if (cell.Letter != word[idx]) return false;
        if (visited.Contains(cell.Coord)) return false;

        int needed = Mathf.Max(initialMinLen, cell.MinWordLength);
        if (word.Length < needed) return false;

        visited.Add(cell.Coord);
        path.Add(cell);

        if (idx == word.Length - 1) return true;

        foreach (var neighbor in grid.Neighbors(cell.Coord))
        {
            if (CanFormFrom(neighbor, word, idx + 1, grid, path, visited, initialMinLen)) return true;
        }

        visited.Remove(cell.Coord);
        path.RemoveAt(path.Count - 1);
        return false;
    }
}
