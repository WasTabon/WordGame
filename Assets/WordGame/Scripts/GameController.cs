using UnityEngine;

public class GameController : MonoBehaviour
{
    public HexGrid grid;
    public ScoreManager scoreManager;
    public EscapeTimer escapeTimer;
    public GameObject timerHUDRoot;
    public EdgeHighlighter edgeHighlighter;
    public Tutorial tutorial;
    public PanController panController;

    public int numberCellCount = 3;
    public int numberCellMinValue = 4;
    public float escapeBaseSeconds = 30f;
    public float escapeSecondsPerRadius = 12f;

    public int exploreRadius = 3;
    public float exploreCellSize = 75f;

    private float gameStartTime;
    private bool timeRecorded;
    private int currentEscapeLevel;

    private void Start()
    {
        Debug.Assert(grid != null, "GameController: grid missing!");

        if (scoreManager != null) scoreManager.ResetScore();
        gameStartTime = Time.time;
        timeRecorded = false;

        GameStats.RecordGameStarted(GameMode.Current);

        if (GameMode.Current == GameMode.Mode.Escape)
        {
            currentEscapeLevel = LevelManager.CurrentLevel;
            grid.gridRadius = LevelManager.GetRadiusForLevel(currentEscapeLevel);
            grid.cellSize = LevelManager.GetCellSizeForLevel(currentEscapeLevel);
            Debug.Log("[GameController] Escape Level " + currentEscapeLevel + " (radius=" + grid.gridRadius + ", cellSize=" + grid.cellSize + ")");
        }
        else
        {
            grid.gridRadius = exploreRadius;
            grid.cellSize = exploreCellSize;
        }

        var center = HexCoord.Zero;
        var result = BoardGenerator.Generate(grid.gridRadius, center);
        grid.Build(result.letters);

        var centerCell = grid.GetCell(center);
        Debug.Assert(centerCell != null, "GameController: center cell not found!");
        if (centerCell != null) centerCell.SetVacant(true);

        var numberCells = NumberCellPlacer.Place(result.placedPaths, numberCellCount, numberCellMinValue);
        foreach (var pair in numberCells)
        {
            var cell = grid.GetCell(pair.Key);
            if (cell != null) cell.SetMinWordLength(pair.Value);
        }
        Debug.Log("[GameController] Placed " + numberCells.Count + " number cells.");

        if (panController != null) panController.SetPanMode(false);

        SetupModeSpecific();
    }

    private void SetupModeSpecific()
    {
        bool isEscape = GameMode.Current == GameMode.Mode.Escape;

        if (timerHUDRoot != null) timerHUDRoot.SetActive(isEscape);
        if (edgeHighlighter != null && isEscape) edgeHighlighter.Activate();

        bool tutorialWillShow = tutorial != null && !Tutorial.IsTutorialDone(GameMode.Current);

        if (escapeTimer != null && isEscape && !tutorialWillShow)
        {
            float seconds = escapeBaseSeconds + grid.gridRadius * escapeSecondsPerRadius;
            escapeTimer.Begin(seconds);
            Debug.Log("[GameController] Escape mode timer: " + seconds + "s");
        }

        if (tutorial != null) tutorial.TryShow();

        if (escapeTimer != null && isEscape && tutorialWillShow)
        {
            StartCoroutine(StartTimerAfterTutorial());
        }
    }

    private System.Collections.IEnumerator StartTimerAfterTutorial()
    {
        while (tutorial != null && tutorial.gameObject.activeSelf) yield return null;
        float seconds = escapeBaseSeconds + grid.gridRadius * escapeSecondsPerRadius;
        escapeTimer.Begin(seconds);
        Debug.Log("[GameController] Escape mode timer started after tutorial: " + seconds + "s");
    }

    public int CurrentEscapeLevel { get { return currentEscapeLevel; } }

    public void RecordPlayedTime()
    {
        if (timeRecorded) return;
        timeRecorded = true;
        GameStats.AddTimePlayed(Time.time - gameStartTime);
    }
}
