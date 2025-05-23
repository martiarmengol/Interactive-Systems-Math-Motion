using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(OperationGenerator2))]
public class GameMode2Manager : MonoBehaviour
{
    [Header("UI & Board")]
    public UIManager ui;
    public BoardManager boardManager;

    [Header("Optional VFX")]
    public ParticleSystem celebrationVFX;

    OperationGenerator2 opGen;
    bool roundActive;
    bool originSet;
    TileController originTile;

    void Start()
    {
        opGen = GetComponent<OperationGenerator2>();
        opGen.OnOperationGenerated += OnNewOperation;
        opGen.Generate();
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
            var first = boardManager.Tiles.FirstOrDefault(t => t.isFilled);
            if (first != null)
            {
                originSet = true;
                originTile = first;
                Debug.Log($"[Mode2] Origin at {first.gridPos}");
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

        // NEW DEBUGS:
        Debug.Log($"[Mode2] CountH → neg:{negH}, pos:{posH}   CountV → neg:{negV}, pos:{posV}");
        Debug.Log($"[Mode2] Computed size {width}×{height}   Target {a}×{b}");

        bool straight = (width == a && height == b);
        bool swapped = (width == b && height == a);
        if (!straight && !swapped) return;

        Debug.Log($"[Mode2] ✔ Match {width}×{height} ({(straight ? "straight" : "swapped")})");
        roundActive = false;
        StartCoroutine(CompleteAfterDelay(width, height, negH, negV));
    }



    IEnumerator CompleteAfterDelay(int width, int height, int negH, int negV)
    {
        if (celebrationVFX) celebrationVFX.Play();

        Debug.Log("[Mode2] Waiting 2 s…");
        yield return new WaitForSeconds(2f);

        int startX = originTile.gridPos.x - negH;
        int startY = originTile.gridPos.y - negV;
        Debug.Log($"[Mode2] Lighting {width}×{height} from ({startX},{startY})");

        // light-up the whole rectangle
        foreach (var t in boardManager.Tiles)
        {
            var p = t.gridPos;
            if (p.x >= startX && p.x < startX + width
             && p.y >= startY && p.y < startY + height)
            {
                t.FillInstant();
            }
        }

        Debug.Log("[Mode2] Done.");
        opGen.Generate();
    }

    void CountDir(TileController origin, bool horiz, out int neg, out int pos)
    {
        neg = pos = 0;
        var all = boardManager.Tiles;
        // positive direction
        for (int d = 1; ; d++)
        {
            int x = horiz ? origin.gridPos.x + d : origin.gridPos.x;
            int y = horiz ? origin.gridPos.y : origin.gridPos.y + d;
            var t = all.FirstOrDefault(t2 => t2.gridPos.x == x && t2.gridPos.y == y);
            if (t != null && t.isFilled) pos++; else break;
        }
        // negative direction
        for (int d = 1; ; d++)
        {
            int x = horiz ? origin.gridPos.x - d : origin.gridPos.x;
            int y = horiz ? origin.gridPos.y : origin.gridPos.y - d;
            var t = all.FirstOrDefault(t2 => t2.gridPos.x == x && t2.gridPos.y == y);
            if (t != null && t.isFilled) neg++; else break;
        }
    }
}
