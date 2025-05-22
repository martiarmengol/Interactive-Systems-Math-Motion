using System.Collections;
using System.Collections.Generic;
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
            Debug.Log("Tile filled: " + gridPos);

            colorChanger?.NotifyTrigger(other);  // Solo si el script existe
            onTileFilled?.Invoke(this);
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
}
