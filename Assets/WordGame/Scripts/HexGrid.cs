using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HexGrid : MonoBehaviour
{
    public RectTransform container;
    public Sprite cellSprite;
    public float cellSize = 75f;
    public int gridRadius = 3;

    private static readonly Color LetterColor = Color.white;
    private static readonly Color NumberColor = new Color(0.91f, 0.65f, 0.27f, 1f);

    private Dictionary<HexCoord, HexCell> cells = new Dictionary<HexCoord, HexCell>();

    public IReadOnlyDictionary<HexCoord, HexCell> Cells { get { return cells; } }

    public void Build()
    {
        Build(null);
    }

    public void Build(Dictionary<HexCoord, char> presetLetters)
    {
        Debug.Assert(container != null, "HexGrid: container not assigned!");
        Debug.Assert(cellSprite != null, "HexGrid: cellSprite not assigned!");

        Clear();

        foreach (var coord in HexShape(gridRadius))
        {
            char letter;
            if (presetLetters != null && presetLetters.TryGetValue(coord, out letter))
            {
                var cell = CreateCell(coord, letter);
                cells[coord] = cell;
            }
            else
            {
                var cell = CreateCell(coord, LetterDistribution.GetRandom());
                cells[coord] = cell;
            }
        }
    }

    public void Clear()
    {
        if (container != null)
        {
            for (int i = container.childCount - 1; i >= 0; i--)
            {
                var child = container.GetChild(i).gameObject;
                if (Application.isPlaying) Destroy(child);
                else DestroyImmediate(child);
            }
        }
        cells.Clear();
    }

    public HexCell GetCell(HexCoord c)
    {
        HexCell cell;
        cells.TryGetValue(c, out cell);
        return cell;
    }

    public IEnumerable<HexCell> Neighbors(HexCoord c)
    {
        foreach (var n in c.Neighbors())
        {
            HexCell cell;
            if (cells.TryGetValue(n, out cell)) yield return cell;
        }
    }

    public bool HasAnyVacantNeighbor(HexCoord c)
    {
        foreach (var cell in Neighbors(c))
        {
            if (cell.IsVacant) return true;
        }
        return false;
    }

    public void MakeCellsVacant(IEnumerable<HexCoord> coords)
    {
        foreach (var c in coords)
        {
            HexCell cell;
            if (cells.TryGetValue(c, out cell)) cell.SetVacant(true);
        }
    }

    private HexCell CreateCell(HexCoord coord, char letter)
    {
        var go = new GameObject("Cell_" + coord.q + "_" + coord.r, typeof(RectTransform));
        go.transform.SetParent(container, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(cellSize * Mathf.Sqrt(3f), cellSize * 2f);
        rt.anchoredPosition = coord.ToPixel(cellSize);

        var img = go.AddComponent<Image>();
        img.sprite = cellSprite;
        img.raycastTarget = true;

        var letterGO = new GameObject("Letter", typeof(RectTransform));
        letterGO.transform.SetParent(go.transform, false);
        var letterRT = letterGO.GetComponent<RectTransform>();
        letterRT.anchorMin = Vector2.zero;
        letterRT.anchorMax = Vector2.one;
        letterRT.offsetMin = Vector2.zero;
        letterRT.offsetMax = Vector2.zero;
        var letterTMP = letterGO.AddComponent<TextMeshProUGUI>();
        letterTMP.fontSize = 70;
        letterTMP.fontStyle = FontStyles.Bold;
        letterTMP.alignment = TextAlignmentOptions.Center;
        letterTMP.color = LetterColor;
        letterTMP.raycastTarget = false;

        var numberGO = new GameObject("Number", typeof(RectTransform));
        numberGO.transform.SetParent(go.transform, false);
        var numberRT = numberGO.GetComponent<RectTransform>();
        numberRT.anchorMin = new Vector2(0f, 1f);
        numberRT.anchorMax = new Vector2(0f, 1f);
        numberRT.pivot = new Vector2(0f, 1f);
        numberRT.sizeDelta = new Vector2(44, 44);
        numberRT.anchoredPosition = new Vector2(10, -14);
        var numberTMP = numberGO.AddComponent<TextMeshProUGUI>();
        numberTMP.fontSize = 34;
        numberTMP.fontStyle = FontStyles.Bold;
        numberTMP.alignment = TextAlignmentOptions.Center;
        numberTMP.color = NumberColor;
        numberTMP.raycastTarget = false;
        numberGO.SetActive(false);

        var cell = go.AddComponent<HexCell>();
        cell.hexImage = img;
        cell.letterText = letterTMP;
        cell.numberText = numberTMP;
        cell.Setup(coord, letter);

        return cell;
    }

    private static IEnumerable<HexCoord> HexShape(int radius)
    {
        for (int q = -radius; q <= radius; q++)
        {
            int rMin = Mathf.Max(-radius, -q - radius);
            int rMax = Mathf.Min(radius, -q + radius);
            for (int r = rMin; r <= rMax; r++)
            {
                yield return new HexCoord(q, r);
            }
        }
    }
}
