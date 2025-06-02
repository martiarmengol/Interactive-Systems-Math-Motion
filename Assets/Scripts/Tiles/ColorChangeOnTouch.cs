using UnityEngine;

public class ColorChangeOnTouch : MonoBehaviour
{
    private Renderer rend;
    public bool isLit = false;

    [Header("Color settings")]
    public Color initialColor = Color.white;  // Default color
    public Color litColor = Color.green;      // Color when touched

    [Header("Momentary mode")]
    public bool IsSelected = false; // If true, color reverts on exit

    public delegate void TileLitHandler(ColorChangeOnTouch tile);
    public event TileLitHandler OnTileLit;    // Called when lit
    public event TileLitHandler OnTileUnlit;  // Called when unlit

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = initialColor; // Set initial color
    }

    // Called by HitboxForwarder when player enters
    public void NotifyTrigger(Collider other)
    {
        if (!isLit && other.CompareTag("Player"))
        {
            isLit = true;
            rend.material.color = litColor;
            OnTileLit?.Invoke(this);
        }
    }

    // Called by HitboxForwarder when player exits
    public void NotifyExit(Collider other)
    {
        if (IsSelected && isLit && other.CompareTag("Player"))
        {
            isLit = false;
            rend.material.color = initialColor;
            OnTileUnlit?.Invoke(this);
        }
    }

    // Manually reset to initial color
    public void ResetToInitialColor()
    {
        isLit = false;
        if (rend != null) rend.material.color = initialColor;
    }

    // Manually force lit color
    public void ForceLitColor()
    {
        isLit = true;
        if (rend != null) rend.material.color = litColor;
    }
}
