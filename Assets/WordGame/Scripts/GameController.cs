using UnityEngine;

public class GameController : MonoBehaviour
{
    public HexGrid grid;

    private void Start()
    {
        Debug.Assert(grid != null, "GameController: grid missing!");

        var center = HexCoord.Zero;
        var result = BoardGenerator.Generate(grid.gridRadius, center);
        grid.Build(result.letters);

        var centerCell = grid.GetCell(center);
        Debug.Assert(centerCell != null, "GameController: center cell not found!");
        if (centerCell != null) centerCell.SetVacant(true);
    }
}
