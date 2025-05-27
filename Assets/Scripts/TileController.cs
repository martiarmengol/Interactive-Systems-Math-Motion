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

    private void Awake()
    {
        colorChanger = GetComponent<ColorChangeOnTouch>();
        colorChanger?.ResetToInitialColor();
    }

    public void NotifyTrigger(Collider other)
    {
        // Permitir múltiples activaciones si IsSelected = true
        if (colorChanger != null && colorChanger.IsSelected)
        {
            colorChanger.NotifyTrigger(other);
            if (!isFilled)
            {
                isFilled = true;
                onTileFilled?.Invoke(this);
            }
        }
        else if (!isFilled && other.CompareTag("Player"))
        {
            isFilled = true;
            colorChanger?.NotifyTrigger(other);
            onTileFilled?.Invoke(this);
        }
    }

    public void NotifyExit(Collider other)
    {
        if (colorChanger != null && colorChanger.IsSelected)
        {
            colorChanger.NotifyExit(other);
            isFilled = false; // Restablecer isFilled para permitir reactivación
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

    [Header("Mode 3 only")]
    public int tileValue = 1;

}