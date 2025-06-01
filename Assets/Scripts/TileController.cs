using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TileController : MonoBehaviour
{
    [HideInInspector] public Vector2Int gridPos;
    [HideInInspector] public bool isFilled = false;

    public Collider interactionCollider;
    public UnityEvent<TileController> onTileFilled;

    public ColorChangeOnTouch colorChanger;

    [Header("Audio")]
    [Tooltip("Sound to play when a player steps on this tile.")]
    public AudioClip stepSound;
    [Tooltip("Volume for the step sound (0..1).")]
    [Range(0f, 1f)]
    public float stepVolume = 1f;

    private void Awake()
    {
        colorChanger = GetComponent<ColorChangeOnTouch>();
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
            isFilled = false; // allow reactivation
        }
        else if (isFilled && other.CompareTag("Player"))
        {
            colorChanger?.NotifyExit(other);
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
}
