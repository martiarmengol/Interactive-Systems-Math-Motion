using UnityEngine;

public class ShadowFollower : MonoBehaviour
{
    [Header("Player Reference")]
    [Tooltip("Assign the player's transform in the Inspector")]
    public Transform player;  // Reference to the player's transform

    [Header("Position Settings")]
    [Tooltip("Fixed Y position for the shadow (ground level)")]
    public float fixedY = 0.1f;  // Shadow's constant height (Y axis)

    void Update()
    {
        if (player == null)
            return;

        // Follow player's XZ position (Y constant)
        transform.position = new Vector3(
            player.position.x,
            fixedY,
            player.position.z
        );
    }
}