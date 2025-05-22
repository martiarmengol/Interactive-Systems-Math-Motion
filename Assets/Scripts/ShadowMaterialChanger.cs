using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowMaterialChanger : MonoBehaviour
{
    public Transform player;              // Referencia al jugador

    [Header("Color por altura")]
    public Color lowColor = Color.black;  // Color cerca del suelo
    public Color highColor = Color.red;   // Color en el aire

    public float heightThreshold = 2.0f;  // Punto a partir del cual cambia

    private Renderer rend;
    private Material shadowMat;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("No se encontró Renderer en la sombra.");
            enabled = false;
            return;
        }

        // Usamos una instancia del material para no afectar a otros
        shadowMat = rend.material;
    }

    void Update()
    {
        if (player == null) return;

        if (player.position.y > heightThreshold)
            shadowMat.color = highColor;
        else
            shadowMat.color = lowColor;
    }
}
