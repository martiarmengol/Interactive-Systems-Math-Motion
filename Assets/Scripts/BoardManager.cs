using UnityEngine;
using System.Collections.Generic;    


public class BoardManager : MonoBehaviour
{
    [Header("Board Mesh (one 8×8 board)")]
    public MeshRenderer boardMesh;

    [Header("How many detectors?")]
    [Tooltip("e.g. 2×2 for easy, 4×4 for medium, 8×8 for hard")]
    public int detectorCols = 8;
    public int detectorRows = 8;

    [Header("Your Detector Prefab")]
    public GameObject detectorPrefab;
    public Transform detectorsParent;

    // these are always 8×8 for your chessboard
    const int boardCols = 8;
    const int boardRows = 8;

    void Start()
    {
        if (!boardMesh || !detectorPrefab || !detectorsParent)
        {
            Debug.LogError("BoardManager: Missing references!", this);
            return;
        }
        SpawnDetectors();
    }

    private List<TileController> spawnedTiles = new List<TileController>();
    public IReadOnlyList<TileController> Tiles => spawnedTiles;

    public void SpawnDetectors()
    {
        // clear old
        foreach (Transform c in detectorsParent)
            Destroy(c.gameObject);
        spawnedTiles.Clear();

        // measure board and compute positions…
        // 1) measure your board
        float boardW = boardMesh.bounds.size.x;
        float boardH = boardMesh.bounds.size.z;

        // 2) compute the real tile size of an 8×8 board
        float tileW = boardW / (float)boardCols;
        float tileH = boardH / (float)boardRows;

        // 3) detector scale is always based on that tile size
        Vector3 detectorScale = new Vector3(tileW * 0.8f, 1f, tileH * 0.8f);

        // 4) find the world‐space center of the bottom‐left tile
        Vector3 origin = boardMesh.bounds.center - new Vector3(boardW / 2f - tileW / 2f, 0, boardH / 2f - tileH / 2f);

        // 5) spacing for dynamic grid of detectors
        float spacingX = detectorCols > 1
          ? (boardW - tileW) / (detectorCols - 1)
          : 0f;
        float spacingZ = detectorRows > 1
          ? (boardH - tileH) / (detectorRows - 1)
          : 0f;

        // instantiate NxM detectors
        for (int i = 0; i < detectorCols; i++)
            for (int j = 0; j < detectorRows; j++)
            {
                Vector3 pos = origin + new Vector3(i * spacingX, 0, j * spacingZ);
                GameObject go = Instantiate(detectorPrefab, pos, Quaternion.identity, detectorsParent);
                go.transform.localScale = detectorScale;
                go.name = $"Detector_{i}_{j}";
                
                var tile = go.GetComponent<TileController>();
                tile.gridPos = new Vector2Int(i, j);
                spawnedTiles.Add(tile);
            }
    }
}