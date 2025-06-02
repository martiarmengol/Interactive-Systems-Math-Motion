using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRender : MonoBehaviour
{
    [Header("Player Reference")]
    public GameObject player;  // Reference to the player object 

    [Header("Render Settings")]
    [Tooltip("Height at which the mesh will disappear")]
    public float heightThreshold = 2.0f;  // Y threshold 

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        // Check for required components
        if (player == null || meshRenderer == null)
            return;

        // Get current player height
        float playerHeight = player.transform.position.y;

        // Toggle visibility based on height threshold
        // Mesh is visible only when player is below the threshold
        meshRenderer.enabled = playerHeight <= heightThreshold;
    }
}