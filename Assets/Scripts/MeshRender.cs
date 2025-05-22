using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRender : MonoBehaviour
{
    public GameObject player;  // Asigna aquí el jugador en el Inspector
    public float heightThreshold = 2.0f;  // Punto a partir del cual cambia
    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && meshRenderer != null)
        {
            float alturaJugador = player.transform.position.y;

            // Se desactiva si la altura del jugador es mayor a 1
            meshRenderer.enabled = alturaJugador <= heightThreshold;
        }
    }
}

