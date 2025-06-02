using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private bool canLoad = false;

    void Start()
    {
        // Después de 5 segundos, permitir la carga de escena
        Invoke(nameof(EnableSceneLoading), 2f);
    }

    void EnableSceneLoading()
    {
        canLoad = true;
    }

    public void LoadScene(string sceneName)
    {
        if (!canLoad)
        {
            Debug.Log("Espera 5 segundos antes de cambiar de escena.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
