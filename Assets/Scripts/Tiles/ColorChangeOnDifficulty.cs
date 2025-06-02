using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ColorChangeOnDifficulty : MonoBehaviour
{
    [Header("This key's difficulty setting")]
    public Difficulty difficultyType;

    [Header("Colors")]
    public Color selectedColor = Color.green;    // Color when selected
    public Color unselectedColor = Color.white;  // Default color

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>(); // Cache renderer
    }

    void Start()
    {
        // Subscribe to difficulty change event
        DifficultyManager.Instance.OnDifficultyChanged.AddListener(UpdateColor);
        UpdateColor(DifficultyManager.Instance.CurrentDifficulty); // Set initial color
    }

    void OnDestroy()
    {
        // Unsubscribe
        if (DifficultyManager.Instance != null)
            DifficultyManager.Instance.OnDifficultyChanged.RemoveListener(UpdateColor);
    }

    // Change color based on current difficulty
    private void UpdateColor(Difficulty currentDifficulty)
    {
        rend.material.color = (currentDifficulty == difficultyType) ? selectedColor : unselectedColor;
    }
}
