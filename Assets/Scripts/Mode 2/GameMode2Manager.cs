using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;                   // ← for LoadScene
using System;                                        // ← for Action<>
using TMPro;                                         // ← for TextMeshProUGUI

[RequireComponent(typeof(OperationGenerator2))]
public class GameMode2Manager : MonoBehaviour
{
    [Header("UI & Board")]
    public UIManager ui;
    public BoardManager boardManager;

    [Header("Result Text (win/lose popup)")]
    public TextMeshProUGUI resultText;                  // ← NEW: assigns the “¡Victoria!” text

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

        // Hide resultText at the very beginning
        if (resultText != null)
            resultText.gameObject.SetActive(false);

        opGen.Generate();

        // Subscribe to the dual-player buttons:
        if (controlKeyManager != null)
            controlKeyManager.OnKeyCombinationPressed += HandleKeyCombination;
    }

    void OnDestroy()
    {
        if (controlKeyManager != null)
            controlKeyManager.OnKeyCombinationPressed -= HandleKeyCombination;
    }

    // This is called whenever OperationGenerator2 fires a new operation
    void OnNewOperation(int a, int b, OperationGenerator2.SubMode mode)
    {
        // Hide any leftover result text
        if (resultText != null)
            resultText.gameObject.SetActive(false);

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
        // Play celebration VFX
        if (celebrationVFX)
            celebrationVFX.Play();

        // Show the “¡Victoria!” text immediately
        if (resultText != null)
        {
            resultText.text = "¡Victoria!";
            resultText.gameObject.SetActive(true);
        }

        // Wait 2 seconds before filling the rectangle area
        yield return new WaitForSeconds(2f);

        // Compute start coordinates based on origin + negative offsets
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
        // Now the game is “paused” (roundActive = false), waiting for the player
        // to press NEXT or CLEAR. We do NOT immediately generate a new op here.
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
        // Hide any result text from the previous round
        if (resultText != null)
            resultText.gameObject.SetActive(false);

        // “Reset” is basically the same as OnNewOperation, but preserving a×b
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
        // positive direction
        for (int d = 1; ; d++)
        {
            int x = horiz ? o.gridPos.x + d : o.gridPos.x;
            int y = horiz ? o.gridPos.y : o.gridPos.y + d;
            var t = all.FirstOrDefault(t2 => t2.gridPos.x == x && t2.gridPos.y == y);
            if (t != null && t.isFilled) pos++;
            else break;
        }
        // negative direction
        for (int d = 1; ; d++)
        {
            int x = horiz ? o.gridPos.x - d : o.gridPos.x;
            int y = horiz ? o.gridPos.y : o.gridPos.y - d;
            var t = all.FirstOrDefault(t2 => t2.gridPos.x == x && t2.gridPos.y == y);
            if (t != null && t.isFilled) neg++;
            else break;
        }
    }
}
