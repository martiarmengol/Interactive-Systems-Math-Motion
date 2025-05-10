using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeOnTouch : MonoBehaviour
{
    private Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rend.material.color = new Color(Random.value, Random.value, Random.value);
        }
    }
}
