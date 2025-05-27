using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;                   // ← for LoadScene
using System;                                        // ← for Action<>

[RequireComponent(typeof(OperationGenerator2))]
public class GameMode2Manager : MonoBehaviour
{
    [Header("UI & Board")]
    public UIManager ui;
    public BoardManager boardManager;

    [Header("Optional VFX")]
    public ParticleSystem celebrationVFX;

    [Header("Control Keys (dual-player)")]
    public ControlKeyManager controlKeyManager;       

    // internal state
    OperationGenerator2 opGen;
    bool roundActive;
    bool originSet;
    TileController originTile;

    void Start()
    {
        opGen = GetComponent<OperationGenerator2>();
        opGen.OnOperationGenerated += OnNewOperation;
        opGen.Generate();

        // subscribe to the dual-player buttons:
        if (controlKeyManager != null)
            controlKeyManager.OnKeyCombinationPressed += HandleKeyCombination;
    }

    void OnDestroy()
    {
        if (controlKeyManager != null)
            controlKeyManager.OnKeyCombinationPressed -= HandleKeyCombination;
    }

    void OnNewOperation(int a, int b, OperationGenerator2.SubMode mode)
    {
        ui.SetOperation(a, '×', b);
        boardManager.SpawnDetectors();
        foreach (var t in boardManager.Tiles)
            t.ResetTile();

        originSet = false;
        roundActive = true;
        Debug.Log($"[Mode2] New op {a}×{b}");
    }

    void Update()
    {
        if (!roundActive) return;

        // pick the very first filled tile as origin
        if (!originSet)
        {
            originTile = boardManager.Tiles.FirstOrDefault(t => t.isFilled);
            if (originTile != null)
            {
                originSet = true;
                Debug.Log($"[Mode2] Origin at {originTile.gridPos}");
            }
        }
        else
        {
            TryComplete();
        }
    }

    void TryComplete()
    {
        int a = opGen.operandA, b = opGen.operandB;
        CountDir(originTile, true, out int negH, out int posH);
        CountDir(originTile, false, out int negV, out int posV);

        int width = negH + posH + 1;
        int height = negV + posV + 1;

        Debug.Log($"[Mode2] Computed size {width}×{height}   Target {a}×{b}");
        if ((width == a && height == b) || (width == b && height == a))
        {
            Debug.Log($"[Mode2] ✔ Match {width}×{height}");
            roundActive = false;
            StartCoroutine(CompleteAfterDelay(width, height, negH, negV));
        }
    }

    IEnumerator CompleteAfterDelay(int w, int h, int negH, int negV)
    {
        if (celebrationVFX) celebrationVFX.Play();
        yield return new WaitForSeconds(2f);

        int sx = originTile.gridPos.x - negH;
        int sy = originTile.gridPos.y - negV;
        foreach (var t in boardManager.Tiles)
        {
            var p = t.gridPos;
            if (p.x >= sx && p.x < sx + w &&
                p.y >= sy && p.y < sy + h)
            {
                t.FillInstant();
            }
        }

        Debug.Log("[Mode2] Rectangle filled — waiting for NEXT or CLEAR");
        // note: **do not** immediately call opGen.Generate() here any more.
        // now we wait for the NEXT button.
    }

    // ----------------------------------------------------------------
    // Dual-player control keys
    void HandleKeyCombination(KeyType key)
    {
        switch (key)
        {
            case KeyType.NEXT:
                if (!roundActive)
                {
                    Debug.Log("[Mode2] NEXT pressed → next operation");
                    opGen.Generate();
                }
                break;

            case KeyType.CLEAR:
                Debug.Log("[Mode2] CLEAR pressed → reset this round");
                ResetRound();
                break;

            case KeyType.MAIN_MENU:
                Debug.Log("[Mode2] MAIN_MENU pressed → back to Select Mode");
                SceneManager.LoadScene("Select Mode");
                break;
        }
    }

    void ResetRound()
    {
        // exactly the same as OnNewOperation but preserving the current a×b
        boardManager.SpawnDetectors();
        foreach (var t in boardManager.Tiles)
            t.ResetTile();

        originSet = false;
        roundActive = true;
    }

    // ----------------------------------------------------------------
    void CountDir(TileController o, bool horiz, out int neg, out int pos)
    {
        neg = pos = 0;
        var all = boardManager.Tiles;
        // positive
        for (int d = 1; ; d++)
        {
            int x = horiz ? o.gridPos.x + d : o.gridPos.x;
            int y = horiz ? o.gridPos.y : o.gridPos.y + d;
            var t = all.FirstOrDefault(t2 => t2.gridPos.x == x && t2.gridPos.y == y);
            if (t != null && t.isFilled) pos++; else break;
        }
        // negative
        for (int d = 1; ; d++)
        {
            int x = horiz ? o.gridPos.x - d : o.gridPos.x;
            int y = horiz ? o.gridPos.y : o.gridPos.y - d;
            var t = all.FirstOrDefault(t2 => t2.gridPos.x == x && t2.gridPos.y == y);
            if (t != null && t.isFilled) neg++; else break;
        }
    }
}
