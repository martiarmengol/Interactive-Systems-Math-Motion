using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;    // ← for LoadScene
using TMPro;                         // ← for TextMeshProUGUI

[RequireComponent(typeof(OperationGenerator2))]
public class GameMode2Manager : MonoBehaviour
{
    [Header("UI & Board")]
    public UIManager ui; // Reference to UI manager
    public BoardManager boardManager; // Reference to board manager

    [Header("Origin Highlight")]
    public Color originColor = Color.yellow; // Color for origin tile

    [Header("Win VFX & SFX")]
    public ParticleSystem celebrationVFX; // Particle effect for win
    public AudioSource winAudioSource; // Audio source for win sound
    public AudioClip winAudioClip; // Win sound clip
    [Range(0f, 1f)]
    public float winVolume = 1f; // Volume for win sound

    [Header("Lose VFX & SFX")]
    public AudioSource loseAudioSource; // Audio source for lose sound
    public AudioClip loseAudioClip; // Lose sound clip
    [Range(0f, 1f)]
    public float loseVolume = 1f; // Volume for lose sound

    [Header("Control Keys (dual-player)")]
    public ControlKeyManager controlKeyManager; // Reference to control key manager

    public TextMeshProUGUI countdownText;  // Reference to countdown text

    private Coroutine checkRoutine = null; // Coroutine for checking result
    private bool isCheckingResult = false; // Flag for checking result

    // Internal state:
    OperationGenerator2 opGen; // Reference to operation generator
    bool roundActive;       // Flag for active round
    bool originSet;         // Flag for origin set
    TileController originTile; // Reference to origin tile

    void Start()
    {
        opGen = GetComponent<OperationGenerator2>();

        // Configure based on difficulty
        if (DifficultyManager.Instance == null || boardManager == null)
        {
            // Default to difficult mode if null
            if (boardManager != null)
            {
                boardManager.detectorCols = 12;
                boardManager.detectorRows = 10;
                opGen.minOperand = 5;
                opGen.maxOperand = 9;
            }
        }

        if (DifficultyManager.Instance == null
            || DifficultyManager.Instance.CurrentDifficulty == Difficulty.Difficult)
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
            opGen.minOperand = 2;
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

    // Called when a new operation is generated
    void OnNewOperation(int a, int b, OperationGenerator2.SubMode mode)
    {
        // Update the UI
        ui.SetOperation(a, '×', b);

        // Reset the board
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

        // Set origin tile when first tile is filled
        if (!originSet)
        {
            originTile = boardManager.Tiles.FirstOrDefault(t => t.isFilled);
            if (originTile != null)
            {
                originSet = true;
                Debug.Log($"[Mode2] Origin chosen at {originTile.gridPos}");

                // Highlight origin tile
                Renderer mr = originTile.GetComponent<Renderer>();
                if (mr != null)
                    mr.material.color = originColor;
            }
        }
    }

    // Handle key combinations
    void HandleKeyCombination(KeyType key)
    {
        switch (key)
        {
            case KeyType.NEXT:
                // Check result if round is active
                if (roundActive && checkRoutine == null && !isCheckingResult)
                {
                    checkRoutine = StartCoroutine(CheckResultRoutine());
                }
                break;

            case KeyType.CLEAR:
                if (roundActive)
                {
                    Debug.Log("[Mode2] CLEAR pressed → reset this same operation");
                    ResetRound();  // Reset for same operation
                }
                break;

            case KeyType.MAIN_MENU:
                Debug.Log("[Mode2] MAIN_MENU pressed → back to Select Mode");
                SceneManager.LoadScene("Select Mode");
                break;
        }
    }

    // Coroutine for checking result
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
        ValidateCurrentAnswer();

        checkRoutine = null;
        isCheckingResult = false;
    }

    // Cancel the check
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

    // Validate the current answer
    void ValidateCurrentAnswer()
    {
        if (!originSet)
        {
            // No tiles filled, so it's wrong
            StartCoroutine(LoseSequence());
            return;
        }

        // Count filled tiles horizontally/vertically from origin
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
            // Correct answer
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

    // Coroutine for win sequence
    IEnumerator WinSequence(int w, int h, int negH, int negV)
    {
        // Play win VFX
        if (celebrationVFX != null)
            celebrationVFX.Play();

        // Play win SFX
        if (winAudioSource != null && winAudioClip != null)
            winAudioSource.PlayOneShot(winAudioClip, winVolume);

        // Fill all tiles in the rectangle
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

        // Wait for players to see the filled rectangle
        yield return new WaitForSeconds(5f);

        // Generate next operation
        opGen.Generate();
    }

    // Coroutine for lose sequence
    IEnumerator LoseSequence()
    {
        // Turn all tiles red
        foreach (var t in boardManager.Tiles)
        {
            var mr = t.GetComponent<Renderer>();
            if (mr != null)
            {
                mr.material.color = Color.red;
            }
        }

        // Play lose SFX
        if (loseAudioSource != null && loseAudioClip != null)
            loseAudioSource.PlayOneShot(loseAudioClip, loseVolume);

        // Wait for players to see the red board
        yield return new WaitForSeconds(2f);

        // Generate next operation
        opGen.Generate();
    }

    // Reset the round
    void ResetRound()
    {
        // Reset the board
        boardManager.SpawnDetectors();
        foreach (var t in boardManager.Tiles)
            t.ResetTile();

        originSet = false;
        roundActive = true;
        originTile = null;
    }

    // Count filled tiles in a direction from origin
    void CountDir(TileController origin, bool horiz, out int neg, out int pos)
    {
        neg = pos = 0;
        var all = boardManager.Tiles;

        // Count in positive direction
        for (int d = 1; ; d++)
        {
            int x = horiz ? origin.gridPos.x + d : origin.gridPos.x;
            int y = horiz ? origin.gridPos.y : origin.gridPos.y + d;
            var t = all.FirstOrDefault(t2 => t2.gridPos.x == x && t2.gridPos.y == y);
            if (t != null && t.isFilled) pos++;
            else break;
        }

        // Count in negative direction
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
