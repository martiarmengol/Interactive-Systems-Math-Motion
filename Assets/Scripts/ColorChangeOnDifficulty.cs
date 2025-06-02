using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ColorChangeOnDifficulty : MonoBehaviour
{
    [Header("Configuración de dificultad de esta tecla")]
    public Difficulty difficultyType; 

    [Header("Colores")]
    public Color selectedColor = Color.green;
    public Color unselectedColor = Color.white;

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    void Start()
    {
        // Suscribirse al evento de cambio de dificultad
        DifficultyManager.Instance.OnDifficultyChanged.AddListener(UpdateColor);
        UpdateColor(DifficultyManager.Instance.CurrentDifficulty);
    }

    void OnDestroy()
    {
        if (DifficultyManager.Instance != null)
            DifficultyManager.Instance.OnDifficultyChanged.RemoveListener(UpdateColor);
    }

    private void UpdateColor(Difficulty currentDifficulty)
    {
        rend.material.color = (currentDifficulty == difficultyType) ? selectedColor : unselectedColor;
    }
}
