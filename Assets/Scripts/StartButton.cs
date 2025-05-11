using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private const string NextScene = "Scene";

    public void OnStartPressed()
    {
        // Optional safety check
        if (string.IsNullOrEmpty(NextScene))
        {
            Debug.LogError("NextScene is empty!", this);
            return;
        }

        SceneManager.LoadScene(NextScene);
    }
}
