using UnityEngine;

public class ShadowMaterialChanger : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;  // Reference to player's transform

    [Header("Color Settings")]
    [Tooltip("Shadow color when player is grounded")]
    public Color lowColor = Color.black;  // Default shadow color

    [Tooltip("Shadow color when player is airborne")]
    public Color highColor = Color.red;   // Color when above threshold

    [Header("Height Threshold")]
    [Tooltip("Height at which shadow color changes")]
    public float heightThreshold = 2.0f;  // Y-value (height threshold)

    private Renderer rend;
    private Material shadowMat;

    void Start()
    {
        // Get the Renderer component
        rend = GetComponent<Renderer>();

        // Check
        if (rend == null)
        {
            Debug.LogError("Renderer component not found on shadow object.", this);
            enabled = false;
            return;
        }

        // Create material instance
        shadowMat = rend.material;
    }

    void Update()
    {
        // Exit if player reference is missing
        if (player == null) return;

        // Change color based on player's height
        shadowMat.color = player.position.y > heightThreshold
            ? highColor
            : lowColor;
    }
}