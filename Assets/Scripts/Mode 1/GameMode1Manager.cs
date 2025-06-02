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
    public ParticleSystem winVFX;
    public ParticleSystem loseVFX;

    [Header("Control Keys")]
    public ControlKeyManager controlKeyManager;

    [Header("UI")]
    public TextMeshProUGUI resultText;

    private int targetCount;
    private HashSet<TileController> filled = new HashSet<TileController>();
    private bool isCheckingResult = false;

    private Coroutine checkRoutine = null;

    public TextMeshProUGUI countdownText;

    void Start()
    {
        if (controlKeyManager != null)
        {
            controlKeyManager.OnKeyCombinationPressed += HandleKeyCombo;
        }

        StartRound();
    }

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

    IEnumerator CheckResultRoutine()
    {
        isCheckingResult = true;
        float countdown = 3f;
        countdownText.gameObject.SetActive(true);

        foreach (var tile in filled)
        {
            tile.StartBlink();
        }

        while (countdown > 0f)
        {
            countdown -= Time.deltaTime;
            countdownText.text = $"Checking result in: {countdown:F1}s";
            yield return null;
        }

        foreach (var tile in filled)
        {
            tile.StopBlink();
        }

        countdownText.gameObject.SetActive(false);
        CheckResult();

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

        foreach (var tile in filled)
        {
            tile.StopBlink();
        }

        isCheckingResult = false;
        countdownText.gameObject.SetActive(false);
        Debug.Log("Confirmación cancelada por salida de casilla.");
    }

    void OnTileFilled(TileController tile)
    {
        filled.Add(tile);
        tile.onTileFilled.RemoveListener(OnTileFilled);
        tile.onTileFilled.AddListener(OnTileFilled);
    }

    void StartRound()
    {
        board.SpawnDetectors();
        filled.Clear();
        isCheckingResult = false;
        resultText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);

        foreach (var t in board.Tiles)
        {
            t.ResetTile();
            t.onTileFilled.RemoveListener(OnTileFilled);
            t.onTileFilled.AddListener(OnTileFilled);
        }

        var op = opGen.GetNextOperation();
        targetCount = op.result;
        ui.SetOperation(op.a, op.op, op.b);

        Debug.Log($"Nuevo round - Objetivo: {targetCount}");
    }

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

    void Win()
    {
        Debug.Log("¡Victoria! Resultado correcto");
        resultText.text = "¡Victoria!";
        resultText.gameObject.SetActive(true);
        // winVFX?.Play();
        StartCoroutine(RestartAfterDelay(2f));
    }

    void Lose()
    {
        Debug.Log("Derrota. Resultado incorrecto");
        resultText.text = "Derrota";
        resultText.gameObject.SetActive(true);
        // loseVFX?.Play();
        StartCoroutine(RestartAfterDelay(2f));
    }

    void ResetTilesOnly()
    {
        Debug.Log("CLEAR presionado: reiniciando los tiles sin cambiar el objetivo.");
        board.SpawnDetectors();
        filled.Clear();
        isCheckingResult = false;
        resultText.gameObject.SetActive(false);

        foreach (var t in board.Tiles)
        {
            t.ResetTile();
            t.onTileFilled.RemoveListener(OnTileFilled);
            t.onTileFilled.AddListener(OnTileFilled);
        }
    }

    IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartRound();
    }

    void OnDestroy()
    {
        if (controlKeyManager != null)
        {
            controlKeyManager.OnKeyCombinationPressed -= HandleKeyCombo;
        }
    }
}


