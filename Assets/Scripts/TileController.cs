using UnityEngine;
using UnityEngine.Events;
using System.Collections;


[RequireComponent(typeof(Collider))]
public class TileController : MonoBehaviour
{

    [Header("Blinking Settings")]
    public Color blinkColor = Color.yellow;  // Color asignable desde el Inspector
    private Coroutine blinkRoutine;

    private Renderer rend;

    [HideInInspector] public Vector2Int gridPos;
    [HideInInspector] public bool isFilled = false;

    public Collider interactionCollider;
    public UnityEvent<TileController> onTileFilled;

    public ColorChangeOnTouch colorChanger;

    [Header("Audio")]
    [Tooltip("Sound to play when a player steps on this tile.")]
    public AudioClip stepSound;
    [Tooltip("Volume for the step sound (0..1).")]
    [Range(0f, 100f)]


    public GameMode1Manager gameManager;



    public float stepVolume = 1f;

    private void Awake()
    {
        colorChanger = GetComponent<ColorChangeOnTouch>();
        rend = GetComponent<Renderer>();
        colorChanger?.ResetToInitialColor();
    }

    public void NotifyTrigger(Collider other)
    {
        // Play the step sound exactly once when the tile transitions to ‘filled’.
        if ((!isFilled && other.CompareTag("Player")) ||
             (colorChanger != null && colorChanger.IsSelected && !isFilled && other.CompareTag("Player")))
        {
            // 1) Play the audio cue:
            if (stepSound != null)
            {
                // Use PlayClipAtPoint so you don't need a per-tile AudioSource:
                AudioSource.PlayClipAtPoint(stepSound, transform.position, stepVolume);
            }

            // 2) Then continue with the existing “fill” logic:
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

        // Notificar salida para posible cancelación
        if (other.CompareTag("Player") && gameManager != null)
        {
            gameManager.CancelCheck();
        }
    }


    public void ResetTile()
    {
        isFilled = false;
        colorChanger?.ResetToInitialColor();
    }

    public void FillInstant()
    {
        isFilled = true;
        colorChanger?.ForceLitColor();
    }

    public void StartBlink()
    {
        if (blinkRoutine == null && isFilled)
        {
            blinkRoutine = StartCoroutine(BlinkCoroutine());
        }
    }

    public void StopBlink()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
            // Volver al color lit
            colorChanger?.ForceLitColor();
        }
    }

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
