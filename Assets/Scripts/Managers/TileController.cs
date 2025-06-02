using UnityEngine;
using UnityEngine.Events;
using System.Collections;


[RequireComponent(typeof(Collider))]
public class TileController : MonoBehaviour
{

    [Header("Blinking Settings")]
    public Color blinkColor = Color.yellow;  // Color for blinking effect
    private Coroutine blinkRoutine;

    private Renderer rend;

    [HideInInspector] public Vector2Int gridPos; // Grid position
    [HideInInspector] public bool isFilled = false; // Is the tile filled?

    public Collider interactionCollider; // Collider for interaction
    public UnityEvent<TileController> onTileFilled; // Event when tile is filled

    public ColorChangeOnTouch colorChanger; // Handles color changes

    [Header("Audio")]
    [Tooltip("Sound to play when a player steps on this tile.")]
    public AudioClip stepSound; // Sound to play on step
    [Tooltip("Volume for the step sound (0..1).")]
    [Range(0f, 100f)]
    public float stepVolume = 1f; // Volume for step sound

    public GameMode1Manager gameManager; // Reference to game manager

    [Header("Difficulty Selection")]
    public bool isDifficultySelector = false; // Is this a difficulty selector tile?
    public Difficulty selectedDifficulty;  // Selected difficulty if used as selector


    private void Awake()
    {
        colorChanger = GetComponent<ColorChangeOnTouch>();
        rend = GetComponent<Renderer>();
        colorChanger?.ResetToInitialColor();
    }

    // Called when something triggers this tile
    public void NotifyTrigger(Collider other)
    {
        // Play the step sound and fill the tile if a player steps on it
        if ((!isFilled && other.CompareTag("Player")) ||
             (colorChanger != null && colorChanger.IsSelected && !isFilled && other.CompareTag("Player")))
        {
            // Play audio cue
            if (stepSound != null)
            {
                AudioSource.PlayClipAtPoint(stepSound, transform.position, stepVolume);
            }

            // If this tile is a difficulty selector, set the difficulty
            if (isDifficultySelector)
            {
                DifficultyManager.Instance?.SetDifficulty(selectedDifficulty);
                return;
            }

            // Fill logic
            if (colorChanger != null && colorChanger.IsSelected)
            {
                colorChanger.NotifyTrigger(other);
                isFilled = true;
                onTileFilled?.Invoke(this);
                return;
            }
            else
            {
                isFilled = true;
                colorChanger?.NotifyTrigger(other);
                onTileFilled?.Invoke(this);
                return;
            }
        }
    }

    // Called when something exits this tile
    public void NotifyExit(Collider other)
    {
        if (colorChanger != null && colorChanger.IsSelected)
        {
            colorChanger.NotifyExit(other);
            isFilled = false;
        }
        else if (isFilled && other.CompareTag("Player"))
        {
            colorChanger?.NotifyExit(other);
        }

        // Notify game manager for possible cancel
        if (other.CompareTag("Player") && gameManager != null)
        {
            gameManager.CancelCheck();
        }
    }


    // Reset tile to initial state
    public void ResetTile()
    {
        isFilled = false;
        colorChanger?.ResetToInitialColor();
    }

    // Instantly fill and light up the tile
    public void FillInstant()
    {
        isFilled = true;
        colorChanger?.ForceLitColor();
    }

    // Start blinking effect
    public void StartBlink()
    {
        if (blinkRoutine == null && isFilled)
        {
            blinkRoutine = StartCoroutine(BlinkCoroutine());
        }
    }

    // Stop blinking effect
    public void StopBlink()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
            colorChanger?.ForceLitColor(); // Restore lit color
        }
    }

    // Coroutine for blinking effect
    private IEnumerator BlinkCoroutine()
    {
        Color lit = colorChanger.litColor;
        Color alt = blinkColor;

        while (true)
        {
            rend.material.color = alt;
            yield return new WaitForSeconds(0.3f);
            rend.material.color = lit;
            yield return new WaitForSeconds(0.3f);
        }
    }
}
