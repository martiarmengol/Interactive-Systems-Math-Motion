using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    [Header("Manual Board Size (World Units)")]
    public float boardWidth = 8f;  // Ancho manual del tablero.
    public float boardHeight = 8f; // Alto manual del tablero.

    [Header("How many detectors?")]
    public int detectorCols = 8;
    public int detectorRows = 8;

    [Header("Your Detector Prefab")]
    public GameObject detectorPrefab;
    public Transform detectorsParent;

    [Header("Hitbox Prefab")]
    public GameObject hitboxPrefab;

    // Constantes internas para referencia
    const int boardCols = 8;
    const int boardRows = 8;

    private List<TileController> spawnedTiles = new List<TileController>();
    public IReadOnlyList<TileController> Tiles => spawnedTiles;

    void Start()
    {
        if (!detectorPrefab || !detectorsParent)
        {
            Debug.LogError("BoardManager: Missing references!", this);
            return;
        }

        SpawnDetectors();
    }

    public void SpawnDetectors()
    {
        // Limpiar detectores antiguos
        foreach (Transform c in detectorsParent)
            Destroy(c.gameObject);
        spawnedTiles.Clear();

        float boardW = boardWidth;
        float boardH = boardHeight;

        float tileW = boardW / detectorCols;
        float tileH = boardH / detectorRows;

        float detectorW = tileW * 0.8f;
        float detectorH = tileH * 0.8f;
        Vector3 detectorScale = new Vector3(detectorW, 1f, detectorH);

        Vector3 origin = new Vector3(
            -boardW / 2f + tileW / 2f,  // centro horizontal
            -1.5f,                      // altura fija
            -50f + tileH / 2f           // anclado al fondo
        );

        float spacingX = tileW;
        float spacingZ = tileH;

        for (int i = 0; i < detectorCols; i++)
        {
            for (int j = 0; j < detectorRows; j++)
            {
                Vector3 basePos = origin + new Vector3(i * spacingX, 0f, j * spacingZ);

                // Crear detector visual
                GameObject go = Instantiate(detectorPrefab, basePos, Quaternion.identity, detectorsParent);
                go.transform.localScale = detectorScale;
                go.name = $"Detector_{i}_{j}";

                var tile = go.GetComponent<TileController>();
                tile.gridPos = new Vector2Int(i, j);
                spawnedTiles.Add(tile);

                // Desactivar collider del propio detector visual (opcional)
                Collider ownCollider = go.GetComponent<Collider>();
                if (ownCollider)
                    ownCollider.enabled = false;

                // Crear hitbox transparente (trigger)
                if (hitboxPrefab)
                {
                    float hitboxHeight = 2f;
                    Vector3 hitboxScale = new Vector3(tileW, hitboxHeight, tileH);
                    Vector3 hitboxPos = basePos + new Vector3(0f, hitboxHeight / 2f, 0f);

                    GameObject hitbox = Instantiate(hitboxPrefab, hitboxPos, Quaternion.identity, detectorsParent);
                    hitbox.transform.localScale = hitboxScale;
                    hitbox.name = $"Hitbox_{i}_{j}";

                    // Asignar collider al TileController
                    Collider hitboxCollider = hitbox.GetComponent<Collider>();
                    tile.interactionCollider = hitboxCollider;

                    // Conectar el hitbox con el TileController para reenviar eventos
                    var forwarder = hitbox.GetComponent<HitboxForwarder>();
                    if (forwarder != null)
                    {
                        forwarder.tile = tile;
                    }
                }
            }
        }
    }
}
