using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WordBuilder : MonoBehaviour
{
    public static WordBuilder Instance { get; private set; }

    public HexGrid grid;
    public RectTransform linesContainer;
    public WordPreviewUI preview;

    public float lineThickness = 14f;
    public Color lineColor = new Color(1f, 1f, 1f, 0.65f);

    private readonly List<HexCell> selected = new List<HexCell>();
    private readonly List<GameObject> lineObjects = new List<GameObject>();
    private bool isSelecting;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (!isSelecting) return;

        if (!Input.GetMouseButton(0))
        {
            SubmitAndClear();
            return;
        }

        var cell = RaycastCellUnderPointer();
        if (cell != null) HandleCellEntered(cell);
    }

    public bool TryStartWord(HexCell cell)
    {
        Debug.Assert(grid != null, "WordBuilder: grid not assigned!");

        if (cell == null || cell.IsVacant) return false;
        if (!grid.HasAnyVacantNeighbor(cell.Coord)) return false;

        ClearSelection();
        AddCell(cell);
        isSelecting = true;
        return true;
    }

    private HexCell RaycastCellUnderPointer()
    {
        var es = EventSystem.current;
        if (es == null) return null;

        var evt = new PointerEventData(es) { position = Input.mousePosition };
        var results = new List<RaycastResult>();
        es.RaycastAll(evt, results);

        for (int i = 0; i < results.Count; i++)
        {
            var c = results[i].gameObject.GetComponent<HexCell>();
            if (c != null) return c;
        }
        return null;
    }

    private void HandleCellEntered(HexCell cell)
    {
        if (cell.IsVacant) return;
        if (selected.Count == 0) return;

        int idx = selected.IndexOf(cell);
        if (idx == selected.Count - 1) return;

        if (selected.Count >= 2 && idx == selected.Count - 2)
        {
            RemoveLastCell();
            return;
        }

        if (idx >= 0) return;

        var last = selected[selected.Count - 1];
        if (!IsNeighbor(last.Coord, cell.Coord)) return;

        AddCell(cell);
    }

    private bool IsNeighbor(HexCoord a, HexCoord b)
    {
        foreach (var n in a.Neighbors())
        {
            if (n == b) return true;
        }
        return false;
    }

    private void AddCell(HexCell cell)
    {
        if (selected.Count > 0)
        {
            DrawLine(selected[selected.Count - 1], cell);
        }
        selected.Add(cell);
        cell.SetSelected(true);
        UpdatePreview();
    }

    private void RemoveLastCell()
    {
        if (selected.Count == 0) return;
        var last = selected[selected.Count - 1];
        last.SetSelected(false);
        selected.RemoveAt(selected.Count - 1);

        if (lineObjects.Count > 0)
        {
            var lastLine = lineObjects[lineObjects.Count - 1];
            lineObjects.RemoveAt(lineObjects.Count - 1);
            if (lastLine != null) Destroy(lastLine);
        }
        UpdatePreview();
    }

    private void SubmitAndClear()
    {
        isSelecting = false;
        if (selected.Count >= 2)
        {
            string word = BuildWordString();
            Debug.Log("[WordBuilder] Submitted: " + word);
        }
        ClearSelection();
    }

    private string BuildWordString()
    {
        var sb = new StringBuilder(selected.Count);
        for (int i = 0; i < selected.Count; i++) sb.Append(selected[i].Letter);
        return sb.ToString();
    }

    private void ClearSelection()
    {
        for (int i = 0; i < selected.Count; i++)
        {
            if (selected[i] != null) selected[i].SetSelected(false);
        }
        selected.Clear();

        for (int i = 0; i < lineObjects.Count; i++)
        {
            if (lineObjects[i] != null) Destroy(lineObjects[i]);
        }
        lineObjects.Clear();

        UpdatePreview();
    }

    private void UpdatePreview()
    {
        if (preview != null) preview.SetWord(BuildWordString());
    }

    private void DrawLine(HexCell a, HexCell b)
    {
        Debug.Assert(linesContainer != null, "WordBuilder: linesContainer not assigned!");
        if (linesContainer == null) return;

        var aRT = a.GetComponent<RectTransform>();
        var bRT = b.GetComponent<RectTransform>();
        var aPos = aRT.anchoredPosition;
        var bPos = bRT.anchoredPosition;

        var mid = (aPos + bPos) * 0.5f;
        var delta = bPos - aPos;
        float dist = delta.magnitude;
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

        var go = new GameObject("Line", typeof(RectTransform));
        go.transform.SetParent(linesContainer, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(dist, lineThickness);
        rt.anchoredPosition = mid;
        rt.localRotation = Quaternion.Euler(0f, 0f, angle);

        var img = go.AddComponent<Image>();
        img.color = lineColor;
        img.raycastTarget = false;

        lineObjects.Add(go);
    }
}
