using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

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
    public RectTransform floatingScoresParent;
    public GameController gameController;

    public Color lineColor = new Color(0.91f, 0.65f, 0.27f, 0.85f);
    public float lineWidth = 18f;

    private readonly List<HexCell> selected = new List<HexCell>();
    private readonly List<RectTransform> lines = new List<RectTransform>();

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

    private void Update()
    {
        if (gameOverShown) return;

        if (PanController.Instance != null && PanController.Instance.ShouldBlockWordInput(-1))
        {
            if (isSelecting) ClearSelection();
            return;
        }

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
        if (PanController.Instance != null && PanController.Instance.PanModeActive) return false;
        Debug.Assert(grid != null, "WordBuilder: grid not assigned!");

        if (cell == null || cell.IsVacant) return false;
        if (!grid.HasAnyVacantNeighbor(cell.Coord)) return false;

        ClearSelection();
        AddCell(cell);
        isSelecting = true;
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySelectStart();
        return true;
    }

    private HexCell RaycastCellUnderPointer()
    {
        var pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        for (int i = 0; i < results.Count; i++)
        {
            var hc = results[i].gameObject.GetComponentInParent<HexCell>();
            if (hc != null) return hc;
        }
        return null;
    }

    private void HandleCellEntered(HexCell cell)
    {
        if (cell == null) return;
        if (selected.Count == 0) return;

        if (selected.Count >= 2 && selected[selected.Count - 2] == cell)
        {
            var last = selected[selected.Count - 1];
            last.SetSelected(false);
            selected.RemoveAt(selected.Count - 1);
            if (lines.Count > 0)
            {
                Destroy(lines[lines.Count - 1].gameObject);
                lines.RemoveAt(lines.Count - 1);
            }
            UpdatePreview();
            return;
        }

        if (selected.Contains(cell)) return;
        if (cell.IsVacant) return;

        var prev = selected[selected.Count - 1];
        if (!IsNeighbor(prev.Coord, cell.Coord)) return;

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
        if (selected.Count > 0) DrawLine(selected[selected.Count - 1], cell);
        int chainIndex = selected.Count;
        selected.Add(cell);
        cell.SetSelected(true);
        UpdatePreview();
        if (chainIndex > 0 && SoundManager.Instance != null)
            SoundManager.Instance.PlaySelectAdd(chainIndex - 1);
    }

    private void DrawLine(HexCell from, HexCell to)
    {
        if (linesContainer == null) return;
        var go = new GameObject("Line", typeof(RectTransform));
        go.transform.SetParent(linesContainer, false);
        var img = go.AddComponent<Image>();
        img.color = lineColor;
        img.raycastTarget = false;

        var rt = go.GetComponent<RectTransform>();
        Vector2 a = from.GetComponent<RectTransform>().anchoredPosition;
        Vector2 b = to.GetComponent<RectTransform>().anchoredPosition;
        Vector2 mid = (a + b) * 0.5f;
        Vector2 diff = b - a;
        float len = diff.magnitude;
        rt.anchoredPosition = mid;
        rt.sizeDelta = new Vector2(len, lineWidth);
        rt.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);

        lines.Add(rt);
    }

    private void UpdatePreview()
    {
        if (preview == null) return;
        string word = "";
        for (int i = 0; i < selected.Count; i++) word += selected[i].Letter;
        preview.SetWord(word);
    }

    private int ComputeRequiredLength()
    {
        int max = 0;
        for (int i = 0; i < selected.Count; i++)
        {
            if (selected[i].MinWordLength > max) max = selected[i].MinWordLength;
        }
        return max;
    }

    private void SubmitAndClear()
    {
        isSelecting = false;
        if (selected.Count == 0)
        {
            ClearSelection();
            return;
        }

        string word = "";
        for (int i = 0; i < selected.Count; i++) word += selected[i].Letter;

        int requiredMin = ComputeRequiredLength();
        ValidationResult result = validator != null
            ? validator.Validate(word, requiredMin)
            : ValidationResult.NotInDictionary;

        if (result == ValidationResult.Valid)
        {
            if (validator != null) validator.MarkUsed(word);
            bool numberBonus = selected.Count > 0 && selected[0].MinWordLength > 0;
            int scoreDelta = 0;
            Vector2 popupPos = Vector2.zero;
            if (selected.Count > 0)
            {
                Vector2 sum = Vector2.zero;
                for (int i = 0; i < selected.Count; i++)
                    sum += selected[i].GetComponent<RectTransform>().anchoredPosition;
                popupPos = sum / selected.Count;
            }

            ApplyValidWord();
            if (scoreManager != null) scoreDelta = scoreManager.AddWord(word, numberBonus);
            GameStats.RecordWord(word, scoreDelta);

            if (preview != null)
            {
                string flashMsg = numberBonus ? "✓ " + word + " ×2" : "✓ " + word;
                preview.FlashSuccess(flashMsg);
            }

            if (floatingScoresParent != null && scoreDelta > 0)
            {
                string popupText = numberBonus ? "+" + scoreDelta + " ×2" : "+" + scoreDelta;
                FloatingScorePopup.Spawn(floatingScoresParent, popupPos, popupText, new Color(0.31f, 0.80f, 0.51f, 1f));
            }

            if (SoundManager.Instance != null) SoundManager.Instance.PlaySuccess();
            Debug.Log("[WordBuilder] Accepted: " + word + (numberBonus ? " (number bonus)" : ""));
            CheckEndgameAfterValidWord();
        }
        else
        {
            string msg = MessageFor(result);
            if (preview != null) preview.FlashError(msg);
            if (SoundManager.Instance != null) SoundManager.Instance.PlayError();
            if (ScreenShaker.Instance != null) ScreenShaker.Instance.Shake(25f, 0.25f);
            Debug.Log("[WordBuilder] Rejected (" + result + "): " + word);
            ClearSelection(false);
        }
    }

    private string MessageFor(ValidationResult r)
    {
        switch (r)
        {
            case ValidationResult.TooShort: return "TOO SHORT";
            case ValidationResult.NotInDictionary: return "NOT IN DICTIONARY";
            case ValidationResult.AlreadyUsed: return "ALREADY USED";
            default: return "INVALID";
        }
    }

    private void ApplyValidWord()
    {
        for (int i = 0; i < selected.Count; i++) selected[i].SetVacant(true);
        ClearSelection(true);
    }

    private void ClearSelection(bool clearPreview = true)
    {
        for (int i = 0; i < selected.Count; i++) selected[i].SetSelected(false);
        selected.Clear();
        for (int i = 0; i < lines.Count; i++) Destroy(lines[i].gameObject);
        lines.Clear();
        if (clearPreview) UpdatePreview();
    }

    private void UpdatePreviewForce()
    {
        if (preview != null) preview.SetWord("");
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
            Debug.Log("[WordBuilder] Escape win! Score: " + finalScore + ", time left: " + timeLeft);

            GameStats.RecordEscapeWin();
            if (gameController != null) gameController.RecordPlayedTime();

            if (SoundManager.Instance != null) SoundManager.Instance.PlayWin();
            if (winPopup != null) winPopup.ShowResult(finalScore, timeLeft);
            else Debug.LogWarning("WordBuilder: winPopup not assigned!");
            return;
        }

        if (DeadlockDetector.HasAnyValidWord(grid, validator)) return;

        if (GameMode.Current == GameMode.Mode.Explore)
        {
            Debug.Log("[WordBuilder] Explore deadlock — continuing to next stage");
            if (SoundManager.Instance != null) SoundManager.Instance.PlayVacantPop();
            if (gameController != null) gameController.ContinueExplore();
            return;
        }

        gameOverShown = true;
        if (escapeTimer != null) escapeTimer.Stop();
        int score = scoreManager != null ? scoreManager.CurrentScore : 0;
        Debug.Log("[WordBuilder] Deadlock! Final score: " + score);

        if (GameMode.Current == GameMode.Mode.Escape) GameStats.RecordEscapeLoss();
        if (gameController != null) gameController.RecordPlayedTime();

        if (SoundManager.Instance != null) SoundManager.Instance.PlayLose();
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

        GameStats.RecordEscapeLoss();
        if (gameController != null) gameController.RecordPlayedTime();

        if (SoundManager.Instance != null) SoundManager.Instance.PlayLose();
        if (gameOverPopup != null) gameOverPopup.ShowResult(score, "TIME'S UP");
        else Debug.LogWarning("WordBuilder: gameOverPopup not assigned!");
    }

    public void ClearAndUnlock()
    {
        ClearSelection();
        isSelecting = false;
        gameOverShown = false;
    }
}
