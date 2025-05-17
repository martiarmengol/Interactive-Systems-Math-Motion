using UnityEngine;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Mode 2: Players set columns and rows to match a multiplication operation.
/// </summary>
public class GameMode2Manager : MonoBehaviour
{
    [Header("References")]
    public BoardManager board;
    public OperationGenerator opGen;
    public TMP_Text operationText;

    [Header("Win VFX")]
    public ParticleSystem winVFX;

    private int targetRows, targetCols;
    private HashSet<int> rowsDone = new HashSet<int>();
    private HashSet<int> colsDone = new HashSet<int>();

    void Start()
    {
        StartRound();
    }

    void StartRound()
    {
        // Spawn and reset detectors
        board.SpawnDetectors();
        rowsDone.Clear(); colsDone.Clear();

        foreach (var tile in board.Tiles)
        {
            tile.ResetTile();
            tile.onTileFilled.RemoveAllListeners();
            tile.onTileFilled.AddListener(OnTileFilled);
        }

        // Generate multiplication problem
        var op = opGen.GetNextMultiplication();
        targetCols = op.a;
        targetRows = op.b;
        operationText.text = $"{op.a} × {op.b} = ?";

        // Prepare VFX
        winVFX.Stop();
    }

    void OnTileFilled(TileController tile, Collider who)
    {
        bool isPlayer1 = who.CompareTag("Player1");

        if (isPlayer1)
        {
            // Player1 fills columns along top row (y == 0)
            if (tile.gridPos.y == 0)
                colsDone.Add(tile.gridPos.x);
        }
        else
        {
            // Player2 fills rows along first column (x == 0)
            if (tile.gridPos.x == 0)
                rowsDone.Add(tile.gridPos.y);
        }

        // Check completion
        if (colsDone.Count >= targetCols && rowsDone.Count >= targetRows)
            Win();
    }

    void Win()
    {
        // Unsubscribe to prevent extra triggers
        foreach (var tile in board.Tiles)
            tile.onTileFilled.RemoveAllListeners();

        // Visually fill the target rectangle
        for (int y = 0; y < targetRows; y++)
            for (int x = 0; x < targetCols; x++)
            {
                int idx = y * board.detectorCols + x;
                var t = board.Tiles[idx];
                t.GetComponent<MeshRenderer>().material = t.filledMaterial;
            }

        // Celebrate!
        winVFX.Play();

        // Next round
        Invoke(nameof(StartRound), 2f);
    }
}
