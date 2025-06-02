using UnityEngine;
using UnityEngine.Events;

public enum Difficulty
{
    Easy,
    Difficult
}

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    // Current selected difficulty (default: Difficult)
    public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Difficult;

    // Event triggered when difficulty changes
    public UnityEvent<Difficulty> OnDifficultyChanged;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene loads
    }

    // Change difficulty and notify listeners
    public void SetDifficulty(Difficulty newDifficulty)
    {
        if (CurrentDifficulty != newDifficulty)
        {
            CurrentDifficulty = newDifficulty;
            Debug.Log($"Difficulty changed to: {CurrentDifficulty}");
            OnDifficultyChanged?.Invoke(newDifficulty);
        }
    }
}
