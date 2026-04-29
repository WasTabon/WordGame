using UnityEngine;

public class GameController : MonoBehaviour
{
    public HexGrid grid;
    public ScoreManager scoreManager;

    public int numberCellCount = 3;
    public int numberCellMinValue = 4;

    private void Start()
    {
        Debug.Assert(grid != null, "GameController: grid missing!");

        if (scoreManager != null) scoreManager.ResetScore();

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
    }
}
