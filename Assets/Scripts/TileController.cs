using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TileController : MonoBehaviour
{
    [HideInInspector] public Vector2Int gridPos;
    [HideInInspector] public bool isFilled = false;

    [Header("Materials")]
    public Material emptyMaterial;
    public Material filledMaterial;

    public Collider interactionCollider;
    public UnityEvent<TileController> onTileFilled;

    private MeshRenderer mr;
    private ColorChangeOnTouch colorChanger;

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        colorChanger = GetComponent<ColorChangeOnTouch>();
        mr.material = emptyMaterial;
    }

    public void NotifyTrigger(Collider other)
    {
        if (!isFilled && other.CompareTag("Player"))
        {
            isFilled = true;
            mr.material = filledMaterial;
            colorChanger?.NotifyTrigger(other);
            onTileFilled?.Invoke(this);
            // ← no Debug.Log here any more
        }
    }

    public void NotifyExit(Collider other)
    {
        if (isFilled && other.CompareTag("Player"))
        {
            colorChanger?.NotifyExit(other);
        }
    }

    public void ResetTile()
    {
        isFilled = false;
        mr.material = emptyMaterial;
    }

    /// <summary>
    /// Used by GameMode2Manager to light up the final rectangle.
    /// </summary>
    public void FillInstant()
    {
        isFilled = true;
        mr.material = filledMaterial;
    }
}
