using UnityEngine;

public class GameController : MonoBehaviour
{
    public HexGrid grid;

    private void Start()
    {
        Debug.Assert(grid != null, "GameController: grid missing!");

        grid.Build();

        var center = grid.GetCell(HexCoord.Zero);
        Debug.Assert(center != null, "GameController: center cell not found!");
        if (center != null) center.SetVacant(true);
    }
}
