using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    [Header("Manual Board Size (World Units)")]
    public float boardWidth = 8f;  // Board width in world units
    public float boardHeight = 8f; // Board height in world units

    [Header("How many detectors?")]
    public int detectorCols = 8;   // Number of columns
    public int detectorRows = 8;   // Number of rows

    [Header("Your Detector Prefab")]
    public GameObject detectorPrefab;   // Prefab for visual detector
    public Transform detectorsParent;   // Parent transform for detectors

    [Header("Hitbox Prefab")]
    public GameObject hitboxPrefab;     // Prefab for hitbox trigger

    // Internal constants for reference
    const int boardCols = 8;
    const int boardRows = 8;

    private List<TileController> spawnedTiles = new List<TileController>(); // List of spawned tiles
    public IReadOnlyList<TileController> Tiles => spawnedTiles; // Public read-only access

    void Start()
    {
        // Check for required references
        if (!detectorPrefab || !detectorsParent)
        {
            Debug.LogError("BoardManager: Missing references!", this);
            return;
        }

        SpawnDetectors(); // Create the board
    }

    // Spawns all detector tiles and hitboxes
    public void SpawnDetectors()
    {
        // Remove old detectors
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

        // Calculate board origin (bottom left corner)
        Vector3 origin = new Vector3(
            -boardW / 2f + tileW / 2f,  // center horizontally
            -1.5f,                      // fixed height
            -50f + tileH / 2f           // anchored to back
        );

        float spacingX = tileW;
        float spacingZ = tileH;

        // Loop through grid positions
        for (int i = 0; i < detectorCols; i++)
        {
            for (int j = 0; j < detectorRows; j++)
            {
                Vector3 basePos = origin + new Vector3(i * spacingX, 0f, j * spacingZ);

                // Create visual detector
                GameObject go = Instantiate(detectorPrefab, basePos, Quaternion.identity, detectorsParent);
                go.transform.localScale = detectorScale;
                go.name = $"Detector_{i}_{j}";

                var tile = go.GetComponent<TileController>();
                tile.gridPos = new Vector2Int(i, j); // Set grid position
                tile.gameManager = FindObjectOfType<GameMode1Manager>(); // Link to game manager
                spawnedTiles.Add(tile);

                // Disable the detector's own collider (optional)
                Collider ownCollider = go.GetComponent<Collider>();
                if (ownCollider)
                    ownCollider.enabled = false;

                // Create transparent hitbox (trigger)
                if (hitboxPrefab)
                {
                    float hitboxHeight = 2f;
                    Vector3 hitboxScale = new Vector3(tileW, hitboxHeight, tileH);
                    Vector3 hitboxPos = basePos + new Vector3(0f, hitboxHeight / 2f, 0f);

                    GameObject hitbox = Instantiate(hitboxPrefab, hitboxPos, Quaternion.identity, detectorsParent);
                    hitbox.transform.localScale = hitboxScale;
                    hitbox.name = $"Hitbox_{i}_{j}";

                    // Assign hitbox collider to TileController
                    Collider hitboxCollider = hitbox.GetComponent<Collider>();
                    tile.interactionCollider = hitboxCollider;

                    // Link hitbox to TileController for event forwarding
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
