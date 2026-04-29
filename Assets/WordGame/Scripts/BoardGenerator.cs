using System.Collections.Generic;
using UnityEngine;

public class BoardGenResult
{
    public Dictionary<HexCoord, char> letters;
    public List<string> placedWords;
    public List<List<HexCoord>> placedPaths;
    public int uncoveredCount;
    public int solvableChainLength;
}

public static class BoardGenerator
{
    private static readonly char[] FallbackNoise = { 'F', 'J', 'K', 'Q', 'X', 'Z' };

    private const int MAX_ATTEMPTS = 50;
    private const int MAX_GREEDY_FAILURES = 60;
    private const int TARGET_CHAIN = 6;
    private const int MAX_GAP_FILL_PASSES = 4;

    public static BoardGenResult Generate(int gridRadius, HexCoord vacantCoord)
    {
        var indexMap = BuildIndexMap(gridRadius);

        BoardGenResult best = null;
        int bestScore = int.MinValue;

        for (int attempt = 0; attempt < MAX_ATTEMPTS; attempt++)
        {
            var r = GenerateOnce(gridRadius, vacantCoord);
            int totalCells = CountAllCells(gridRadius) - 1;
            r.uncoveredCount = totalCells - r.letters.Count;

            int target = Mathf.Min(r.placedWords.Count, TARGET_CHAIN);
            int chain = LongestSolvableChain(r, indexMap, vacantCoord, target);
            r.solvableChainLength = chain;

            bool fullyCovered = r.uncoveredCount == 0;
            bool solvable = chain >= target && target > 0;

            if (fullyCovered && solvable)
            {
                Debug.Log("[BoardGenerator] Perfect board (cover=100%, chain=" + chain + ", words=" + r.placedWords.Count + ") after " + (attempt + 1) + " attempts. Words: " + string.Join(", ", r.placedWords));
                return r;
            }

            int score = -r.uncoveredCount * 100 + chain * 10 + r.placedWords.Count;
            if (score > bestScore) { bestScore = score; best = r; }
        }

        if (best == null) best = GenerateOnce(gridRadius, vacantCoord);
        FillUncoveredWithNoise(best, gridRadius, vacantCoord);

        Debug.LogWarning("[BoardGenerator] Could not find perfect board in " + MAX_ATTEMPTS + " attempts. Best: cover=" + (CountAllCells(gridRadius) - 1 - best.uncoveredCount) + "/" + (CountAllCells(gridRadius) - 1) + ", chain=" + best.solvableChainLength + ", words=" + best.placedWords.Count + ". Filled gaps with noise.");
        return best;
    }

    private static BoardGenResult GenerateOnce(int gridRadius, HexCoord vacantCoord)
    {
        var allCoords = ComputeAllCoords(gridRadius, vacantCoord);

        var allWords = Dictionary.AllWords;
        if (allWords == null || allWords.Count == 0)
        {
            Debug.LogWarning("BoardGenerator: dictionary empty.");
            return GenerateNoiseOnly(allCoords);
        }

        var placedLetters = new Dictionary<HexCoord, char>();
        var placedWords = new List<string>();
        var placedPaths = new List<List<HexCoord>>();
        var usedWords = new HashSet<string>();

        var centerNeighbors = new List<HexCoord>();
        foreach (var n in vacantCoord.Neighbors())
        {
            if (IsInHexShape(n, gridRadius)) centerNeighbors.Add(n);
        }

        var anchorPool = new List<string>();
        for (int i = 0; i < allWords.Count; i++)
        {
            int len = allWords[i].Length;
            if (len >= 4 && len <= 6) anchorPool.Add(allWords[i]);
        }
        Shuffle(anchorPool);

        for (int i = 0; i < anchorPool.Count; i++)
        {
            var path = new List<HexCoord>();
            if (TryPlace(anchorPool[i], placedLetters, vacantCoord, gridRadius, centerNeighbors, null, path))
            {
                ApplyPath(anchorPool[i], path, placedLetters);
                placedWords.Add(anchorPool[i]);
                placedPaths.Add(path);
                usedWords.Add(anchorPool[i]);
                break;
            }
        }

        var greedyPool = new List<string>(allWords);
        Shuffle(greedyPool);
        greedyPool.Sort((a, b) => b.Length.CompareTo(a.Length));

        int failures = 0;
        for (int i = 0; i < greedyPool.Count; i++)
        {
            if (failures > MAX_GREEDY_FAILURES) break;
            var word = greedyPool[i];
            if (usedWords.Contains(word)) continue;

            var path = new List<HexCoord>();
            if (TryPlace(word, placedLetters, vacantCoord, gridRadius, allCoords, null, path))
            {
                ApplyPath(word, path, placedLetters);
                placedWords.Add(word);
                placedPaths.Add(path);
                usedWords.Add(word);
                failures = 0;
            }
            else failures++;
        }

        var shortPool = new List<string>();
        for (int i = 0; i < allWords.Count; i++)
        {
            int len = allWords[i].Length;
            if (len == 3 || len == 4) shortPool.Add(allWords[i]);
        }
        Shuffle(shortPool);
        shortPool.Sort((a, b) => a.Length.CompareTo(b.Length));

        for (int pass = 0; pass < MAX_GAP_FILL_PASSES; pass++)
        {
            var uncovered = new List<HexCoord>();
            for (int i = 0; i < allCoords.Count; i++)
            {
                if (!placedLetters.ContainsKey(allCoords[i])) uncovered.Add(allCoords[i]);
            }
            if (uncovered.Count == 0) break;
            Shuffle(uncovered);

            int filledThisPass = 0;
            for (int u = 0; u < uncovered.Count; u++)
            {
                if (placedLetters.ContainsKey(uncovered[u])) continue;
                if (TryFillGap(uncovered[u], placedLetters, vacantCoord, gridRadius, allCoords, shortPool, usedWords, placedWords, placedPaths))
                    filledThisPass++;
            }
            if (filledThisPass == 0) break;
        }

        return new BoardGenResult
        {
            letters = placedLetters,
            placedWords = placedWords,
            placedPaths = placedPaths
        };
    }

    private static bool TryFillGap(HexCoord targetCell, Dictionary<HexCoord, char> placed, HexCoord vacant, int radius, List<HexCoord> allCoords, List<string> shortWords, HashSet<string> usedWords, List<string> placedWords, List<List<HexCoord>> placedPaths)
    {
        var shuffled = new List<string>(shortWords);
        Shuffle(shuffled);

        for (int i = 0; i < shuffled.Count; i++)
        {
            var word = shuffled[i];
            if (usedWords.Contains(word)) continue;

            var path = new List<HexCoord>();
            if (TryPlace(word, placed, vacant, radius, allCoords, targetCell, path))
            {
                ApplyPath(word, path, placed);
                placedWords.Add(word);
                placedPaths.Add(path);
                usedWords.Add(word);
                return true;
            }
        }
        return false;
    }

    private static bool TryPlace(string word, Dictionary<HexCoord, char> existing, HexCoord vacant, int radius, IList<HexCoord> startCoords, HexCoord? mustInclude, List<HexCoord> outPath)
    {
        var startOrder = new List<HexCoord>(startCoords);
        Shuffle(startOrder);

        for (int i = 0; i < startOrder.Count; i++)
        {
            var start = startOrder[i];
            char ec;
            if (existing.TryGetValue(start, out ec) && ec != word[0]) continue;

            var visited = new HashSet<HexCoord>();
            var path = new List<HexCoord>();
            bool seenTarget = mustInclude.HasValue && start == mustInclude.Value;

            if (PlaceRec(word, 0, start, existing, vacant, radius, visited, path, mustInclude, seenTarget))
            {
                outPath.Clear();
                outPath.AddRange(path);
                return true;
            }
        }
        return false;
    }

    private static bool PlaceRec(string word, int idx, HexCoord pos, Dictionary<HexCoord, char> existing, HexCoord vacant, int radius, HashSet<HexCoord> visited, List<HexCoord> path, HexCoord? mustInclude, bool seenTarget)
    {
        if (visited.Contains(pos)) return false;
        if (pos == vacant) return false;
        if (!IsInHexShape(pos, radius)) return false;

        char ec;
        if (existing.TryGetValue(pos, out ec) && ec != word[idx]) return false;

        bool nowSeen = seenTarget || (mustInclude.HasValue && pos == mustInclude.Value);

        visited.Add(pos);
        path.Add(pos);

        if (idx == word.Length - 1)
        {
            if (mustInclude.HasValue && !nowSeen)
            {
                visited.Remove(pos);
                path.RemoveAt(path.Count - 1);
                return false;
            }
            return true;
        }

        var neighbors = new List<HexCoord>(pos.Neighbors());
        Shuffle(neighbors);

        for (int i = 0; i < neighbors.Count; i++)
        {
            if (PlaceRec(word, idx + 1, neighbors[i], existing, vacant, radius, visited, path, mustInclude, nowSeen)) return true;
        }

        visited.Remove(pos);
        path.RemoveAt(path.Count - 1);
        return false;
    }

    private static int LongestSolvableChain(BoardGenResult board, Dictionary<HexCoord, int> indexMap, HexCoord vacantCoord, int target)
    {
        if (board.placedPaths == null || board.placedPaths.Count == 0) return 0;

        var pathMasks = new long[board.placedPaths.Count];
        var startNeighborMasks = new long[board.placedPaths.Count];
        for (int i = 0; i < board.placedPaths.Count; i++)
        {
            pathMasks[i] = PathMask(board.placedPaths[i], indexMap);
            startNeighborMasks[i] = NeighborMask(board.placedPaths[i][0], indexMap);
        }

        long startState = 1L << indexMap[vacantCoord];
        var visited = new HashSet<long>();
        int best = 0;
        DFS(startState, 0, target, pathMasks, startNeighborMasks, visited, ref best);
        return best;
    }

    private static void DFS(long state, int depth, int target, long[] pathMasks, long[] startNeighborMasks, HashSet<long> visited, ref int best)
    {
        if (depth > best) best = depth;
        if (depth >= target) return;
        if (visited.Contains(state)) return;
        visited.Add(state);

        for (int i = 0; i < pathMasks.Length; i++)
        {
            long pmask = pathMasks[i];
            if ((pmask & state) != 0) continue;
            if ((startNeighborMasks[i] & state) == 0) continue;

            long newState = state | pmask;
            DFS(newState, depth + 1, target, pathMasks, startNeighborMasks, visited, ref best);
            if (best >= target) return;
        }
    }

    private static long PathMask(List<HexCoord> path, Dictionary<HexCoord, int> indexMap)
    {
        long mask = 0;
        for (int i = 0; i < path.Count; i++)
        {
            int idx;
            if (indexMap.TryGetValue(path[i], out idx)) mask |= (1L << idx);
        }
        return mask;
    }

    private static long NeighborMask(HexCoord c, Dictionary<HexCoord, int> indexMap)
    {
        long mask = 0;
        foreach (var n in c.Neighbors())
        {
            int idx;
            if (indexMap.TryGetValue(n, out idx)) mask |= (1L << idx);
        }
        return mask;
    }

    private static Dictionary<HexCoord, int> BuildIndexMap(int radius)
    {
        var map = new Dictionary<HexCoord, int>();
        int idx = 0;
        for (int q = -radius; q <= radius; q++)
        {
            int rMin = Mathf.Max(-radius, -q - radius);
            int rMax = Mathf.Min(radius, -q + radius);
            for (int r = rMin; r <= rMax; r++)
            {
                map[new HexCoord(q, r)] = idx++;
            }
        }
        return map;
    }

    private static List<HexCoord> ComputeAllCoords(int radius, HexCoord exclude)
    {
        var list = new List<HexCoord>();
        for (int q = -radius; q <= radius; q++)
        {
            int rMin = Mathf.Max(-radius, -q - radius);
            int rMax = Mathf.Min(radius, -q + radius);
            for (int r = rMin; r <= rMax; r++)
            {
                var c = new HexCoord(q, r);
                if (c != exclude) list.Add(c);
            }
        }
        return list;
    }

    private static int CountAllCells(int radius)
    {
        return 3 * radius * (radius + 1) + 1;
    }

    private static void ApplyPath(string word, List<HexCoord> path, Dictionary<HexCoord, char> letters)
    {
        for (int i = 0; i < word.Length; i++) letters[path[i]] = word[i];
    }

    private static bool IsInHexShape(HexCoord c, int radius)
    {
        int aq = Mathf.Abs(c.q);
        int ar = Mathf.Abs(c.r);
        int aqr = Mathf.Abs(c.q + c.r);
        int max = aq;
        if (ar > max) max = ar;
        if (aqr > max) max = aqr;
        return max <= radius;
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

    private static void FillUncoveredWithNoise(BoardGenResult r, int radius, HexCoord vacant)
    {
        var allCoords = ComputeAllCoords(radius, vacant);
        for (int i = 0; i < allCoords.Count; i++)
        {
            if (!r.letters.ContainsKey(allCoords[i]))
                r.letters[allCoords[i]] = FallbackNoise[Random.Range(0, FallbackNoise.Length)];
        }
        r.uncoveredCount = 0;
    }

    private static BoardGenResult GenerateNoiseOnly(List<HexCoord> allCoords)
    {
        var letters = new Dictionary<HexCoord, char>();
        for (int i = 0; i < allCoords.Count; i++)
            letters[allCoords[i]] = FallbackNoise[Random.Range(0, FallbackNoise.Length)];
        return new BoardGenResult
        {
            letters = letters,
            placedWords = new List<string>(),
            placedPaths = new List<List<HexCoord>>(),
            uncoveredCount = 0,
            solvableChainLength = 0
        };
    }
}
