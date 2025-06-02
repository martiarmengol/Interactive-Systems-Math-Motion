using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameMode1Manager : MonoBehaviour
{
    [Header("Game Components")]
    public BoardManager board;
    public OperationGenerator opGen;
    public UIManager ui;

    [Header("Control Keys")]
    public ControlKeyManager controlKeyManager;

    [Header("Win Effects")]
    public ParticleSystem celebrationVFX;
    public AudioSource winAudioSource;
    public AudioClip winAudioClip;
    [Range(0f, 1f)] public float winVolume = 1f;

    [Header("Lose Effects")]
    public ParticleSystem loseVFX;
    public AudioSource loseAudioSource;
    public AudioClip loseAudioClip;
    [Range(0f, 1f)] public float loseVolume = 1f;

    // Game state variables
    private int targetCount;
    private HashSet<TileController> filled = new HashSet<TileController>();
    private bool isCheckingResult = false;
    private Coroutine checkRoutine = null;
    public TextMeshProUGUI countdownText;

    void Start()
    {
        // Set up control key listeners
        if (controlKeyManager != null)
        {
            controlKeyManager.OnKeyCombinationPressed += HandleKeyCombo;
        }

        StartRound();
    }

    // Control key inputs
    private void HandleKeyCombo(KeyType keyType)
    {
        if (keyType == KeyType.NEXT && checkRoutine == null && !isCheckingResult)
        {
            checkRoutine = StartCoroutine(CheckResultRoutine());
        }
        else if (keyType == KeyType.CLEAR)
        {
            ResetTilesOnly();
        }
        else if (keyType == KeyType.MAIN_MENU)
        {
            SceneManager.LoadScene("Select Mode");
        }
    }

    // Countdown before checking results
    IEnumerator CheckResultRoutine()
    {
        isCheckingResult = true;
        float countdown = 3f;
        countdownText.gameObject.SetActive(true);

        // Visual feedback during countdown
        foreach (var tile in filled)
        {
            tile.StartBlink();
        }

        while (countdown > 0f)
        {
            countdown -= Time.deltaTime;
            countdownText.text = $"Checking in: {countdown:F1}s";
            yield return null;
        }

        // Clean up countdown effects
        foreach (var tile in filled)
        {
            tile.StopBlink();
        }

        countdownText.gameObject.SetActive(false);
        CheckResult();

        checkRoutine = null;
        isCheckingResult = false;
    }

    // Cancel ongoing countdown
    public void CancelCheck()
    {
        if (checkRoutine != null)
        {
            StopCoroutine(checkRoutine);
            checkRoutine = null;
        }

        foreach (var tile in filled)
        {
            tile.StopBlink();
        }

        isCheckingResult = false;
        countdownText.gameObject.SetActive(false);
        Debug.Log("Confirmation canceled by tile exit.");
    }

    // Track filled tiles
    void OnTileFilled(TileController tile)
    {
        filled.Add(tile);
        tile.onTileFilled.RemoveListener(OnTileFilled);
        tile.onTileFilled.AddListener(OnTileFilled);
    }

    // Initialize new round
    void StartRound()
    {
        // Set difficulty parameters
        if (DifficultyManager.Instance == null || board == null)
        {
            if (board != null)
            {
                SetHardDifficultySettings();
            }
        }
        else if (DifficultyManager.Instance.CurrentDifficulty == Difficulty.Difficult)
        {
            SetHardDifficultySettings();
        }
        else // Easy
        {
            SetEasyDifficultySettings();
        }

        // Initialize board
        board.SpawnDetectors();
        filled.Clear();
        isCheckingResult = false;
        countdownText.gameObject.SetActive(false);

        // Reset all tiles
        foreach (var t in board.Tiles)
        {
            t.ResetTile();
            t.onTileFilled.RemoveListener(OnTileFilled);
            t.onTileFilled.AddListener(OnTileFilled);
        }

        // Generate new operation
        var op = opGen.GetNextOperation();
        targetCount = op.result;
        ui.SetOperation(op.a, op.op, op.b);

        Debug.Log($"New round - Target: {targetCount}");
    }

    // Check if player solved correctly
    void CheckResult()
    {
        if (filled.Count == targetCount)
        {
            Win();
        }
        else
        {
            Lose();
        }
    }

    // Win state handling
    void Win()
    {
        Debug.Log("Victory! Correct result");
        celebrationVFX.Play();
        winAudioSource.PlayOneShot(winAudioClip, winVolume);
        StartCoroutine(RestartAfterDelay(2f));
    }

    // Lose state handling
    void Lose()
    {
        Debug.Log("Defeat. Incorrect result");
        foreach (var t in board.Tiles)
        {
            var mr = t.GetComponent<Renderer>();
            if (mr != null)
            {
                mr.material.color = Color.red;
            }
        }

        loseAudioSource.PlayOneShot(loseAudioClip, loseVolume);
        StartCoroutine(RestartAfterDelay(2f));
    }

    // Reset tiles without changing target
    void ResetTilesOnly()
    {
        Debug.Log("CLEAR pressed: resetting tiles without changing target");
        board.SpawnDetectors();
        filled.Clear();
        isCheckingResult = false;

        foreach (var t in board.Tiles)
        {
            t.ResetTile();
            t.onTileFilled.RemoveListener(OnTileFilled);
            t.onTileFilled.AddListener(OnTileFilled);
        }
    }

    // Restart after delay
    IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartRound();
    }

    // Clean up event listeners
    void OnDestroy()
    {
        if (controlKeyManager != null)
        {
            controlKeyManager.OnKeyCombinationPressed -= HandleKeyCombo;
        }
    }

    // Difficulty setting helpers
    private void SetHardDifficultySettings()
    {
        board.detectorCols = 12;
        board.detectorRows = 10;
        opGen.minValue = 5;
        opGen.maxValue = 20;
        opGen.mode = OperationGenerator.Mode.Mixed;
    }

    private void SetEasyDifficultySettings()
    {
        board.detectorCols = 6;
        board.detectorRows = 5;
        opGen.minValue = 1;
        opGen.maxValue = 10;
        opGen.mode = OperationGenerator.Mode.Addition;
    }
}