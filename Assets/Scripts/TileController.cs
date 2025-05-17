using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider), typeof(MeshRenderer))]
public class TileController : MonoBehaviour
{
    [HideInInspector] public Vector2Int gridPos;
    [HideInInspector] public bool isFilled;

    [Header("Tile Materials")]
    public Material emptyMaterial;
    public Material filledMaterial;

    /// <summary>
    /// Invoked when a Player collider first steps on this tile.
    /// Passes (this tile, triggering collider).
    /// </summary>
    public UnityEvent<TileController, Collider> onTileFilled;

    private MeshRenderer mr;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        ResetTile();
    }

    /// <summary>
    /// Clears the fill state and resets the material to empty.
    /// </summary>
    public void ResetTile()
    {
        isFilled = false;
        if (mr != null && emptyMaterial != null)
            mr.material = emptyMaterial;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isFilled && (other.CompareTag("Player1") || other.CompareTag("Player2")))
        {
            isFilled = true;
            if (filledMaterial != null)
                mr.material = filledMaterial;

            onTileFilled?.Invoke(this, other);
        }
    }
}