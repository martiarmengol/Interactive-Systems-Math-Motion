using UnityEngine;

public class HeightMaterialChanger : MonoBehaviour
{
    public Material defaultMaterial;  // Material when below threshold
    public Material aboveMaterial;    // Material when above threshold

    private Renderer rend;
    private bool isAbove = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("No Renderer found on object", this);
            return;
        }

        rend.sharedMaterial = defaultMaterial;  // Set initial material
    }

    void Update()
    {
        if (transform.position.y > 1f)
        {
            if (!isAbove)
            {
                rend.sharedMaterial = aboveMaterial;  // Switch to above material
                isAbove = true;
            }
        }
        else
        {
            if (isAbove)
            {
                rend.sharedMaterial = defaultMaterial;  // Revert to default
                isAbove = false;
            }
        }
    }
}
