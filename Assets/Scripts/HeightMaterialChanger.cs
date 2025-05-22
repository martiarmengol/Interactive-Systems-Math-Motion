using UnityEngine;

public class HeightMaterialChanger : MonoBehaviour
{
    public Material defaultMaterial;
    public Material aboveMaterial;

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

        rend.sharedMaterial = defaultMaterial;
    }

    void Update()
    {
        if (transform.position.y > 1f)
        {
            if (!isAbove)
            {
                rend.sharedMaterial = aboveMaterial;
                isAbove = true;
            }
        }
        else
        {
            if (isAbove)
            {
                rend.sharedMaterial = defaultMaterial;
                isAbove = false;
            }
        }
    }
}
