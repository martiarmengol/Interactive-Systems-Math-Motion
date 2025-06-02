using UnityEngine;

public class HitboxForwarder : MonoBehaviour
{
    // Reference to the TileController this hitbox is associated with
    public TileController tile;

    // Called when another collider enters the trigger collider
    void OnTriggerEnter(Collider other)
    {
        // Forward the trigger event to the associated tile
        tile?.NotifyTrigger(other);
    }

    // Called when another collider exits the trigger collider
    void OnTriggerExit(Collider other)
    {
        // Forward the exit event to the associated tile
        tile?.NotifyExit(other);
    }
}
