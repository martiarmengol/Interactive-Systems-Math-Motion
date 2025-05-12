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
    public Material emptyMaterial;                      // assign gray
    public Material filledMaterial;                     // assign green

    // Event will pass this tile up to the manager
    public UnityEvent<TileController> onTileFilled;

    private MeshRenderer mr;

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        mr.material = emptyMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        // only fill once, only if it's a player
        if (!isFilled && other.CompareTag("Player"))
        {
            isFilled = true;
            mr.material = filledMaterial;
            onTileFilled?.Invoke(this);
        }
    }

    public void ResetTile()
    {
        isFilled = false;
        mr.material = emptyMaterial;
    }
}


