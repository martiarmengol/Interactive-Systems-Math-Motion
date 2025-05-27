using UnityEngine;

public class HitboxForwarder : MonoBehaviour
{
    public TileController tile;
    public int delay = 0;

    void OnTriggerEnter(Collider other)
    {
        tile?.NotifyTrigger(other);
    }

    void OnTriggerExit(Collider other)
    {
        tile?.NotifyExit(other);
    }
}
