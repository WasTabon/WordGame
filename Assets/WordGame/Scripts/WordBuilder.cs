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
    public WordValidator validator;
    public ScoreManager scoreManager;
    public GameOverPopup gameOverPopup;
    public WinPopup winPopup;
    public EscapeTimer escapeTimer;

    public float lineThickness = 14f;
    public Color lineColor = new Color(1f, 1f, 1f, 0.65f);

    private readonly List<HexCell> selected = new List<HexCell>();
    private readonly List<GameObject> lineObjects = new List<GameObject>();
    private bool isSelecting;

    private bool gameOverShown;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if (escapeTimer != null)
        {
            escapeTimer.OnTimeout -= HandleTimeout;
            escapeTimer.OnTimeout += HandleTimeout;
        }
    }

    private void OnDisable()
    {
        if (escapeTimer != null) escapeTimer.OnTimeout -= HandleTimeout;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (gameOverShown) return;
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
        if (gameOverShown) return false;
        Debug.Assert(grid != null, "WordBuilder: grid not assigned!");

        if (cell == null || cell.IsVacant) return false;
        if (!grid.HasAnyVacantNeighbor(cell.Coord)) return false;

        ClearSelection();
        AddCell(cell);
        isSelecting = true;
        return true;
    }

    private void CheckEndgameAfterValidWord()
    {
        if (gameOverShown) return;

        if (GameMode.Current == GameMode.Mode.Escape && EscapeWinDetector.HasReachedEdge(grid))
        {
            gameOverShown = true;
            if (escapeTimer != null) escapeTimer.Stop();
            int finalScore = scoreManager != null ? scoreManager.CurrentScore : 0;
            float timeLeft = escapeTimer != null ? escapeTimer.TimeLeft : 0f;
            Debug.Log("[WordBuilder] Escape win! Score: " + finalScore + " + bonus, time left: " + timeLeft);

            if (winPopup != null) winPopup.ShowResult(finalScore, timeLeft);
            else Debug.LogWarning("WordBuilder: winPopup not assigned!");
            return;
        }

        if (DeadlockDetector.HasAnyValidWord(grid, validator)) return;

        gameOverShown = true;
        if (escapeTimer != null) escapeTimer.Stop();
        int score = scoreManager != null ? scoreManager.CurrentScore : 0;
        Debug.Log("[WordBuilder] Deadlock! Final score: " + score);

        string title = GameMode.Current == GameMode.Mode.Escape ? "TRAPPED" : "NO MORE WORDS";
        if (gameOverPopup != null) gameOverPopup.ShowResult(score, title);
        else Debug.LogWarning("WordBuilder: gameOverPopup not assigned!");
    }

    private void HandleTimeout()
    {
        if (gameOverShown) return;
        gameOverShown = true;
        ClearSelection();
        isSelecting = false;

        int score = scoreManager != null ? scoreManager.CurrentScore : 0;
        Debug.Log("[WordBuilder] Time's up! Score: " + score);

        if (gameOverPopup != null) gameOverPopup.ShowResult(score, "TIME'S UP");
        else Debug.LogWarning("WordBuilder: gameOverPopup not assigned!");
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

        if (selected.Count < 2)
        {
            ClearSelection();
            return;
        }

        string word = BuildWordString();
        int requiredLen = ComputeRequiredLength();

        ValidationResult result = ValidationResult.Valid;
        if (validator != null) result = validator.Validate(word, requiredLen);
        else Debug.LogWarning("WordBuilder: validator not assigned, accepting all words.");

        if (result == ValidationResult.Valid)
        {
            if (validator != null) validator.MarkUsed(word);
            bool numberBonus = selected.Count > 0 && selected[0].MinWordLength > 0;
            ApplyValidWord();
            if (scoreManager != null) scoreManager.AddWord(word, numberBonus);
            if (preview != null)
            {
                string flashMsg = numberBonus ? "✓ " + word + " ×2" : "✓ " + word;
                preview.FlashSuccess(flashMsg);
            }
            Debug.Log("[WordBuilder] Accepted: " + word + (numberBonus ? " (number bonus)" : ""));
            CheckEndgameAfterValidWord();
        }
        else
        {
            string msg = MessageFor(result);
            if (preview != null) preview.FlashError(msg);
            Debug.Log("[WordBuilder] Rejected (" + result + "): " + word);
            ClearSelection(false);
        }
    }

    private void ApplyValidWord()
    {
        var coords = new List<HexCoord>(selected.Count);
        for (int i = 0; i < selected.Count; i++) coords.Add(selected[i].Coord);

        for (int i = 0; i < selected.Count; i++) selected[i].SetSelected(false);
        selected.Clear();

        for (int i = 0; i < lineObjects.Count; i++)
        {
            if (lineObjects[i] != null) Destroy(lineObjects[i]);
        }
        lineObjects.Clear();

        grid.MakeCellsVacant(coords);
    }

    private int ComputeRequiredLength()
    {
        int max = 0;
        for (int i = 0; i < selected.Count; i++)
        {
            int n = selected[i].MinWordLength;
            if (n > max) max = n;
        }
        return max;
    }

    private string MessageFor(ValidationResult r)
    {
        switch (r)
        {
            case ValidationResult.TooShort: return "TOO SHORT";
            case ValidationResult.NotInDictionary: return "NOT A WORD";
            case ValidationResult.AlreadyUsed: return "ALREADY USED";
            default: return "?";
        }
    }

    private string BuildWordString()
    {
        var sb = new StringBuilder(selected.Count);
        for (int i = 0; i < selected.Count; i++) sb.Append(selected[i].Letter);
        return sb.ToString();
    }

    private void ClearSelection(bool clearPreview = true)
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

        if (clearPreview) UpdatePreview();
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
