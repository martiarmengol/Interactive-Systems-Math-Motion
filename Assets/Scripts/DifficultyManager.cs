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

    public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Easy;

    public UnityEvent<Difficulty> OnDifficultyChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruye el duplicado
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Mantiene este objeto al cambiar de escena
    }


    public void SetDifficulty(Difficulty newDifficulty)
    {
        if (CurrentDifficulty != newDifficulty)
        {
            CurrentDifficulty = newDifficulty;
            Debug.Log($"Dificultad cambiada a: {CurrentDifficulty}");
            OnDifficultyChanged?.Invoke(newDifficulty);
        }
    }

}
