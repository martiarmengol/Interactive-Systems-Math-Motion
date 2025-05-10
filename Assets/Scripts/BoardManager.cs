using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int columns = 8;
    public int rows = 8;
    public float cellSize = 5f;

    [Header("Level Mask (optional)")]
    // If you want “holes” or fewer detectors, 
    // size this at [columns,rows], and only spawn
    // when mask[x,y] == true.
    public bool[,] levelMask = null;

    [Header("References")]
    public GameObject detectorPrefab;
    public Transform detectorsParent;

    void Start()
    {
        GenerateDetectors();
    }

    void GenerateDetectors()
    {
        // Clear out any old detectors
        if (detectorsParent != null)
            foreach (Transform t in detectorsParent)
                Destroy(t.gameObject);

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // if you’ve supplied a mask and it’s false, skip this cell
                if (levelMask != null && levelMask.GetLength(0) == columns &&
                                        levelMask.GetLength(1) == rows &&
                                        levelMask[x, y] == false)
                    continue;

                // center the grid around world‐zero (optional)
                Vector3 offset = new Vector3(
                    -((columns - 1) * cellSize) / 2f,
                    0,
                    -((rows - 1) * cellSize) / 2f
                );
                Vector3 worldPos = offset + new Vector3(x * cellSize, 0, y * cellSize);

                var det = Instantiate(detectorPrefab,
                                      worldPos,
                                      Quaternion.identity,
                                      detectorsParent);
                det.name = $"Detector_{x}_{y}";
            }
        }
    }
}
