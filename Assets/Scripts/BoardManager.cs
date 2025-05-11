using UnityEngine;

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

    void SpawnDetectors()
    {
        // clean old
        foreach (Transform c in detectorsParent)
            Destroy(c.gameObject);

        // 1) measure your board
        float boardW = boardMesh.bounds.size.x;
        float boardH = boardMesh.bounds.size.z;

        // 2) compute the real tile size of an 8×8 board
        float tileW = boardW / (float)boardCols;
        float tileH = boardH / (float)boardRows;

        // 3) detector scale is always based on that tile size
        Vector3 detectorScale = new Vector3(tileW * 0.8f, 1f, tileH * 0.8f);

        // 4) find the world‐space center of the bottom‐left tile
        Vector3 origin = boardMesh.bounds.center
                       - new Vector3(boardW / 2f - tileW / 2f,
                                     0,
                                     boardH / 2f - tileH / 2f);

        // 5) spacing for dynamic grid of detectors
        float spacingX = detectorCols > 1
          ? (boardW - tileW) / (detectorCols - 1)
          : 0f;
        float spacingZ = detectorRows > 1
          ? (boardH - tileH) / (detectorRows - 1)
          : 0f;

        // 6) instantiate your NxM detectors
        for (int i = 0; i < detectorCols; i++)
        {
            for (int j = 0; j < detectorRows; j++)
            {
                Vector3 pos = origin
                            + new Vector3(i * spacingX,
                                          0,
                                          j * spacingZ);

                GameObject d = Instantiate(
                    detectorPrefab,
                    pos,
                    Quaternion.identity,
                    detectorsParent);

                d.transform.localScale = detectorScale;
                d.name = $"Detector_{i}_{j}";
            }
        }
    }
}


/*using UnityEngine;

/// <summary>
/// Spawns a grid of detector prefabs automatically centered
/// on the surface of an imported board FBX (or any MeshRenderer).
/// </summary>
public class BoardManager : MonoBehaviour
{
    [Header("Board Mesh")]
    [Tooltip("Drag in your FBX's GameObject (must have a MeshRenderer)")]
    public MeshRenderer boardMesh;

    [Header("Grid Dimensions")]
    public int columns = 8;
    public int rows = 8;

    [Header("Detector Prefab")]
    [Tooltip("Your Detector.prefab with BoxCollider(IsTrigger) + ColorChangeOnTouch")]
    public GameObject detectorPrefab;

    [Header("Parent for Spawned Detectors")]
    [Tooltip("An empty Transform to keep your Hierarchy tidy")]
    public Transform detectorsParent;

    void Start()
    {
        if (boardMesh == null || detectorPrefab == null || detectorsParent == null)
        {
            Debug.LogError("BoardManager: Missing references!", this);
            return;
        }
        GenerateDetectors();
    }

    void GenerateDetectors()
    {
        // 1) Clean up any old detectors
        foreach (Transform child in detectorsParent)
            Destroy(child.gameObject);

        // 2) Get the world-space size of the board:
        //    bounds.size.x is width → divide by columns → tile width
        Vector3 boardSize = boardMesh.bounds.size;
        float tileW = boardSize.x / columns;
        float tileH = boardSize.z / rows;

        // 3) Compute the center of the top-left tile:
        //    board center minus half-size plus half-one-tile
        Vector3 start = boardMesh.bounds.center - new Vector3(boardSize.x / 2f - tileW / 2f, 0, -boardSize.z / 2f + tileH / 2f);

        // 4) Loop & spawn
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // world position for this tile
                Vector3 pos = start
                            + new Vector3(x * tileW, 0, -y * tileH);

                GameObject det = Instantiate(
                    detectorPrefab,
                    pos,
                    Quaternion.identity,
                    detectorsParent);

                // scale to 80% of tile so it sits nicely inside
                det.transform.localScale = new Vector3(tileW * 0.8f, 1f, tileH * 0.8f);
                det.name = $"Detector_{x}_{y}";
            }
        }
    }
}*/
