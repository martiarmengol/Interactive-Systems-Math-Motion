using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        var src = GetComponent<AudioSource>();
        if (src != null && !src.isPlaying)
            src.Play();
    }
}


