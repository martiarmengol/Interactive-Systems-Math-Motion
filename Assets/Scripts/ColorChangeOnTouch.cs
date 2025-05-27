using UnityEngine;

public class ColorChangeOnTouch : MonoBehaviour
{
    private Renderer rend;
    public bool isLit = false;

    [Header("Configuración de colores")]
    public Color initialColor = Color.white;
    public Color litColor = Color.green;

    [Header("Modo momentáneo")]
    public bool IsSelected = false; // Si está activo, el color vuelve al original al salir del trigger

    public delegate void TileLitHandler(ColorChangeOnTouch tile);
    public event TileLitHandler OnTileLit;
    public event TileLitHandler OnTileUnlit;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = initialColor;
    }

    // Llamado por el HitboxForwarder cuando el jugador entra
    public void NotifyTrigger(Collider other)
    {
        if (!isLit && other.CompareTag("Player"))
        {
            isLit = true;
            rend.material.color = litColor;
            OnTileLit?.Invoke(this);
        }
    }

    // Llamado por el HitboxForwarder cuando el jugador sale
    public void NotifyExit(Collider other)
    {
        if (IsSelected && isLit && other.CompareTag("Player"))
        {
            isLit = false;
            rend.material.color = initialColor;
            OnTileUnlit?.Invoke(this);
        }
    }

    public void ResetToInitialColor()
    {
        isLit = false;
        if (rend != null) rend.material.color = initialColor;
    }

    public void ForceLitColor()
    {
        isLit = true;
        if (rend != null) rend.material.color = litColor;
    }

}
