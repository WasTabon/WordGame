using UnityEngine;

public static class EscapeWinDetector
{
    public static bool HasReachedEdge(HexGrid grid)
    {
        if (grid == null) return false;
        int r = grid.gridRadius;
        foreach (var pair in grid.Cells)
        {
            var coord = pair.Key;
            var cell = pair.Value;
            if (!cell.IsVacant) continue;
            if (IsEdge(coord, r)) return true;
        }
        return false;
    }

    private static bool IsEdge(HexCoord c, int radius)
    {
        int aq = Mathf.Abs(c.q);
        int ar = Mathf.Abs(c.r);
        int aqr = Mathf.Abs(c.q + c.r);
        int max = aq;
        if (ar > max) max = ar;
        if (aqr > max) max = aqr;
        return max == radius;
    }
}
