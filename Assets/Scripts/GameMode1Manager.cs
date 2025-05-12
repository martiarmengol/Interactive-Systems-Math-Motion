using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode1Manager : MonoBehaviour
{
    public BoardManager board;         // your existing BoardManager
    public OperationGenerator opGen;
    public UIManager ui;
    public ParticleSystem winVFX;

    private int targetCount;
    private HashSet<TileController> filled = new HashSet<TileController>();

    void Start()
    {
        StartRound();
    }

    void StartRound()
    {
        // 1) Spawn & reset the board
        board.SpawnDetectors();             // now you control exactly when detectors appear
        filled.Clear();

        // 2) Reset every tile and subscribe
        foreach (var t in board.Tiles)
        {
            t.ResetTile();                  // ensure isFilled=false + gray mat
            // remove old listeners just in case
            t.onTileFilled.RemoveListener(OnTileFilled);
            t.onTileFilled.AddListener(OnTileFilled);
        }

        // 3) Generate a fresh operation
        var op = opGen.GetNextOperation();
        targetCount = op.result;
        ui.SetOperation(op.a, op.op, op.b);

        // 4) Update the UI counter to 0/target
        ui.UpdateScore(0, targetCount);

        // 5) (Optional) Move your winVFX into place so it’s ready
        winVFX.Stop();
    }

    void OnTileFilled(TileController tile)
    {
        filled.Add(tile);
        ui.UpdateScore(filled.Count, targetCount);

        if (filled.Count >= targetCount)
            Win();
    }

    void Win()
    {
        // Unsubscribe so no extra fills count
        foreach (var t in board.Tiles)
            t.onTileFilled.RemoveListener(OnTileFilled);

        winVFX.Play();
        // Restart after 2 sec (or whatever)
        Invoke(nameof(StartRound), 2f);
    }
}
