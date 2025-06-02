using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages audio playback across different scenes
public class AudioManager : MonoBehaviour
{
    void Awake()
    {
        // Keep audio playing when changing scenes
        DontDestroyOnLoad(gameObject);

        // Start playing audio if not already playing
        var src = GetComponent<AudioSource>();
        if (src != null && !src.isPlaying)
            src.Play();
    }
}


