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

    void Start()
    {
        // Suscribir al evento de combinación de teclas
        if (controlKeyManager != null)
        {
            controlKeyManager.OnKeyCombinationPressed += HandleKeyCombination;
        }

        StartRound();
    }

    void HandleKeyCombination(KeyType keyType)
    {
        if (keyType == KeyType.NEXT && !isCheckingResult)
        {
            isCheckingResult = true;
            CheckResult();
        }
        else if (keyType == KeyType.CLEAR)
        {
            ResetTilesOnly();
        }
        else if (keyType == KeyType.MAIN_MENU)
        {
            Debug.Log("MAIN_MENU presionado: cargando escena 'Select Mode'");
            SceneManager.LoadScene("Select Mode");
        }
    }

    void StartRound()
    {
        // Resetear estado
        board.SpawnDetectors();
        filled.Clear();
        isCheckingResult = false;

        // Ocultar texto de resultado
        resultText.gameObject.SetActive(false);

        // Resetear tiles
        foreach (var t in board.Tiles)
        {
            t.ResetTile();
            t.onTileFilled.RemoveListener(OnTileFilled);
            t.onTileFilled.AddListener(OnTileFilled);
        }

        // Generar nueva operación
        var op = opGen.GetNextOperation();
        targetCount = op.result;
        ui.SetOperation(op.a, op.op, op.b);

        Debug.Log($"Nuevo round - Objetivo: {targetCount}");
    }


    void OnTileFilled(TileController tile)
    {
        filled.Add(tile);
        Debug.Log($"Casillas llenadas: {filled.Count}/{targetCount}");
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
        //winVFX.Play();
        StartCoroutine(RestartAfterDelay(2f));
    }

    void Lose()
    {
        Debug.Log("Derrota. Resultado incorrecto");
        resultText.text = "Derrota";
        resultText.gameObject.SetActive(true);
        //loseVFX.Play();
        StartCoroutine(RestartAfterDelay(2f));
    }

    void ResetTilesOnly()
    {
        Debug.Log("CLEAR presionado: reiniciando los tiles sin cambiar el objetivo.");

        // Resetear estado
        board.SpawnDetectors();
        filled.Clear();
        isCheckingResult = false;

        // Ocultar texto de resultado
        resultText.gameObject.SetActive(false);

        // Resetear tiles
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
        // Desuscribir del evento al destruir
        if (controlKeyManager != null)
        {
            controlKeyManager.OnKeyCombinationPressed -= HandleKeyCombination;
        }
    }
}