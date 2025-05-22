using UnityEngine;

public class HitboxForwarder : MonoBehaviour
{
    public TileController tile;

    void OnTriggerEnter(Collider other)
    {
        tile?.NotifyTrigger(other);
    }

    void OnTriggerExit(Collider other)
    {
        tile?.NotifyExit(other);
    }
}
