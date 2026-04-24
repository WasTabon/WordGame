using UnityEngine;

public struct HexCoord
{
    public int q;
    public int r;

    public HexCoord(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    public static HexCoord Zero => new HexCoord(0, 0);

    public HexCoord[] Neighbors()
    {
        return new[]
        {
            new HexCoord(q + 1, r),
            new HexCoord(q + 1, r - 1),
            new HexCoord(q, r - 1),
            new HexCoord(q - 1, r),
            new HexCoord(q - 1, r + 1),
            new HexCoord(q, r + 1)
        };
    }

    public Vector2 ToPixel(float size)
    {
        float x = size * Mathf.Sqrt(3f) * (q + r * 0.5f);
        float y = -size * 1.5f * r;
        return new Vector2(x, y);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is HexCoord)) return false;
        var o = (HexCoord)obj;
        return q == o.q && r == o.r;
    }

    public override int GetHashCode()
    {
        return (q * 73856093) ^ (r * 19349663);
    }

    public override string ToString()
    {
        return "(" + q + "," + r + ")";
    }

    public static bool operator ==(HexCoord a, HexCoord b) { return a.q == b.q && a.r == b.r; }
    public static bool operator !=(HexCoord a, HexCoord b) { return a.q != b.q || a.r != b.r; }
}
