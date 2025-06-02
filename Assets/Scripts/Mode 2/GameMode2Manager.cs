using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;    // ← for LoadScene
using TMPro;                         // ← for TextMeshProUGUI


[RequireComponent(typeof(OperationGenerator2))]
public class GameMode2Manager : MonoBehaviour
{
    [Header("UI & Board")]
    public UIManager ui;
    public BoardManager boardManager;


    [Header("Win VFX & SFX")]
    public ParticleSystem celebrationVFX;
    public AudioSource winAudioSource;
    public AudioClip winAudioClip;
    [Range(0f, 1f)]
    public float winVolume = 1f;

    [Header("Lose VFX & SFX")]
    public ParticleSystem loseVFX;
    public AudioSource loseAudioSource;
    public AudioClip loseAudioClip;
    [Range(0f, 1f)]
    public float loseVolume = 1f;

    [Header("Control Keys (dual-player)")]
    public ControlKeyManager controlKeyManager;

    public TextMeshProUGUI countdownText;  // referencia al contador visual

    private Coroutine checkRoutine = null;
    private bool isCheckingResult = false;


    // ─────────────────────────────────────────────────────────────
    // Internal state:
    OperationGenerator2 opGen;
    bool roundActive;       // “true” while we are waiting for students to fill in tiles
    bool originSet;         // “true” once we've seen at least one filled tile
    TileController originTile;

    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        opGen = GetComponent<OperationGenerator2>();

        // Configuración según dificultad
        if (DifficultyManager.Instance == null || boardManager == null)
        {
            // Modo difícil por defecto si alguno es null
            if (boardManager != null)
            {
                boardManager.detectorCols = 12;
                boardManager.detectorRows = 10;
                opGen.minOperand = 5;
                opGen.maxOperand = 9;
            }
        } // <-- Aquí faltaba cerrar el primer if

        if (DifficultyManager.Instance == null || DifficultyManager.Instance.CurrentDifficulty == Difficulty.Difficult)
        {
            boardManager.detectorCols = 12;
            boardManager.detectorRows = 10;
            opGen.minOperand = 5;
            opGen.maxOperand = 9;
        }
        else // Easy
        {
            boardManager.detectorCols = 6;
            boardManager.detectorRows = 5;
            opGen.minOperand = 1;
            opGen.maxOperand = 4;
        }

        opGen.OnOperationGenerated += OnNewOperation;
        opGen.Generate();

        if (controlKeyManager != null)
            controlKeyManager.OnKeyCombinationPressed += HandleKeyCombination;
    }


    void OnDestroy()
    {
        if (controlKeyManager != null)
            controlKeyManager.OnKeyCombinationPressed -= HandleKeyCombination;
    }

    // Called every time OperationGenerator2 generates a brand‐new a×b
    void OnNewOperation(int a, int b, OperationGenerator2.SubMode mode)
    {

        // Update the UI (e.g. “5 × 4”)
        ui.SetOperation(a, '×', b);

        // Re‐spawn all detectors (full reset)
        boardManager.SpawnDetectors();
        foreach (var t in boardManager.Tiles)
            t.ResetTile();

        // Reset state flags
        originSet = false;
        roundActive = true;
        originTile = null;

        Debug.Log($"[Mode2] New op {a}×{b}");
    }

    void Update()
    {
        if (!roundActive)
            return;

        // As soon as they step on the very first tile, record it as “origin”
        if (!originSet)
        {
            originTile = boardManager.Tiles.FirstOrDefault(t => t.isFilled);
            if (originTile != null)
            {
                originSet = true;
                Debug.Log($"[Mode2] Origin chosen at {originTile.gridPos}");
            }
        }
    }

    // ─────────────────────────────────────────────────────────────
    // This method only runs when the players press BOTH “NEXT” keys
    // (i.e. when KeyType.NEXT is fired by ControlKeyManager).
    void HandleKeyCombination(KeyType key)
    {
        switch (key)
        {
            case KeyType.NEXT:
                // If we are still “roundActive == true”, that means they have not yet validated.
                // Once they press NEXT, we check “did they actually build the correct rectangle?”
                if (roundActive && checkRoutine == null && !isCheckingResult)
                {
                    checkRoutine = StartCoroutine(CheckResultRoutine());
                }
                break;
                // If roundActive is false, then we are already in a “win/lose sequence” or paused state.
                // In this simple implementation, we ignore “NEXT” presses once roundActive==false,
                // because the coroutines themselves will auto‐advance or reset after their delay.

            case KeyType.CLEAR:
                if (roundActive)
                {
                    Debug.Log("[Mode2] CLEAR pressed → reset this same operation");
                    ResetRound();  // same a×b, let them try again
                }
                break;

            case KeyType.MAIN_MENU:
                Debug.Log("[Mode2] MAIN_MENU pressed → back to Select Mode");
                SceneManager.LoadScene("Select Mode");
                break;
        }
    }

    IEnumerator CheckResultRoutine()
    {
        isCheckingResult = true;
        float countdown = 3f;
        countdownText.gameObject.SetActive(true);

        foreach (var tile in boardManager.Tiles.Where(t => t.isFilled))
        {
            tile.StartBlink();
        }

        while (countdown > 0f)
        {
            countdown -= Time.deltaTime;
            countdownText.text = $"Checking in: {countdown:F1}s";
            yield return null;
        }

        foreach (var tile in boardManager.Tiles.Where(t => t.isFilled))
        {
            tile.StopBlink();
        }

        countdownText.gameObject.SetActive(false);
        ValidateCurrentAnswer(); // llamada a la validación original

        checkRoutine = null;
        isCheckingResult = false;
    }

    public void CancelCheck()
    {
        if (checkRoutine != null)
        {
            StopCoroutine(checkRoutine);
            checkRoutine = null;
        }

        foreach (var tile in boardManager.Tiles.Where(t => t.isFilled))
        {
            tile.StopBlink();
        }

        isCheckingResult = false;
        countdownText.gameObject.SetActive(false);
        Debug.Log("Confirmación cancelada por salida de casilla.");
    }


    // Called when NEXT is pressed the first time in a round
    void ValidateCurrentAnswer()
    {
        if (!originSet)
        {
            // They never stepped on any tile, so for sure it’s wrong
            StartCoroutine(LoseSequence());
            return;
        }

        // Count how many tiles are lit horizontally/vertically from origin
        int a = opGen.operandA;
        int b = opGen.operandB;
        CountDir(originTile, true, out int negH, out int posH);
        CountDir(originTile, false, out int negV, out int posV);

        int width = negH + posH + 1;
        int height = negV + posV + 1;

        Debug.Log($"[Mode2] Computed size {width}×{height}   Target {a}×{b}");

        // Freeze further Update calls
        roundActive = false;

        if ((width == a && height == b) || (width == b && height == a))
        {
            // They got the correct a×b (or b×a):
            Debug.Log($"[Mode2] ✔ Correct answer!");
            StartCoroutine(WinSequence(width, height, negH, negV));
        }
        else
        {
            // Wrong shape
            Debug.Log($"[Mode2] ✖ Incorrect answer.");
            StartCoroutine(LoseSequence());
        }
    }

    // ─────────────────────────────────────────────────────────────
    IEnumerator WinSequence(int w, int h, int negH, int negV)
    {
        // 1) Play Win VFX
        if (celebrationVFX != null)
            celebrationVFX.Play();

        // 2) Play Win SFX
        if (winAudioSource != null && winAudioClip != null)
            winAudioSource.PlayOneShot(winAudioClip, winVolume);

        // 4) Immediately fill all tiles in that rectangle (no extra wait)
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

        // 5) Wait a few seconds so players can see the filled rectangle
        yield return new WaitForSeconds(5f);

        // 6) Automatically generate the next operation
        opGen.Generate();
    }


    IEnumerator LoseSequence()
    {
        // 1) Turn every tile’s mesh red immediately
        foreach (var t in boardManager.Tiles)
        {
            var mr = t.GetComponent<Renderer>();
            if (mr != null)
            {
                mr.material.color = Color.red;
            }
        }

        // 2) Play Lose SFX (if you assigned one)
        if (loseAudioSource != null && loseAudioClip != null)
            loseAudioSource.PlayOneShot(loseAudioClip, loseVolume);

        // 3) (Text output has been removed)

        // 4) Wait so they can see the red‐board / SFX
        yield return new WaitForSeconds(2f);

        // 5) Automatically generate the next operation
        opGen.Generate();
    }



    void ResetRound()
    {

        // Re‐spawn all detectors (tiles) and clear them
        boardManager.SpawnDetectors();
        foreach (var t in boardManager.Tiles)
            t.ResetTile();

        originSet = false;
        roundActive = true;
        originTile = null;
    }

    // ─────────────────────────────────────────────────────────────
    // Count how many consecutively‐filled tiles exist in the positive
    // and negative directions from “origin”. Used for forming the rectangle.
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
            if (t != null && t.isFilled) pos++;
            else break;
        }

        // negative direction
        for (int d = 1; ; d++)
        {
            int x = horiz ? origin.gridPos.x - d : origin.gridPos.x;
            int y = horiz ? origin.gridPos.y : origin.gridPos.y - d;
            var t = all.FirstOrDefault(t2 => t2.gridPos.x == x && t2.gridPos.y == y);
            if (t != null && t.isFilled) neg++;
            else break;
        }
    }
}
