using UnityEngine;

public class IsSelected : MonoBehaviour
{
    private Renderer rend;
    private bool isLit = false;

    [Header("Configuraci�n de colores")]
    public Color initialColor = Color.white;
    public Color litColor = Color.green;

    public delegate void TileLitHandler(IsSelected tile);
    public event TileLitHandler OnTileLit;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = initialColor;
    }

    // Este m�todo debe llamarse externamente cuando haya colisi�n
    public void NotifyTrigger(Collider other)
    {
        if (!isLit && other.CompareTag("Player"))
        {
            isLit = true;
            rend.material.color = litColor;
            OnTileLit?.Invoke(this);
        }
    }
}
