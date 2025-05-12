using UnityEngine;
public class ColorChangeOnTouch : MonoBehaviour
{
    private Renderer rend;
    private bool isLit = false;
    public delegate void TileLitHandler(ColorChangeOnTouch tile);
    public event TileLitHandler OnTileLit;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isLit && other.CompareTag("Player"))
        {
            isLit = true;
            rend.material.color = Color.green;
            OnTileLit?.Invoke(this);
        }
    }
}
