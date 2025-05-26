using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Call this from your button's OnClick
    public void LoadSelectMode()
    {
        SceneManager.LoadScene("Select Mode");
        
    }
}
