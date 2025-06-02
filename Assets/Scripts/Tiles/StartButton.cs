using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Name of the scene to load
    private const string NextScene = "Scene";


    // Called when the Start button is pressed in the menu
    public void OnStartPressed()
    {
        // Check for empty scene name
        if (string.IsNullOrEmpty(NextScene))
        {
            Debug.LogError("NextScene is empty!", this);
            return;
        }

        // Load the predefined scene
        SceneManager.LoadScene(NextScene);
    }
}