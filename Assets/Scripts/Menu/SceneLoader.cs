using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Loading Settings")]
    [Tooltip("Delay before allowing scene changes (seconds)")]
    [SerializeField] private float loadingDelay = 2f;

    private bool canLoad = false;

    void Start()
    {
        // Enable scene loading after specified delay
        Invoke(nameof(EnableSceneLoading), loadingDelay);
    }


    // Enables scene loading after initial delay
    private void EnableSceneLoading()
    {
        canLoad = true;
    }


    // Loads specified scene if loading is permitted
    public void LoadScene(string sceneName)
    {
        if (!canLoad)
        {
            Debug.LogWarning($"Please wait {loadingDelay} seconds before changing scenes.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}