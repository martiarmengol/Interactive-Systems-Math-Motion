using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFollower : MonoBehaviour
{

    public Transform player;     // Asigna aquí el transform del jugador
    public float fixedY = 0.1f;  // Altura fija de la sombra (por ejemplo, justo encima del suelo)
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = player.position;
        targetPos.y = fixedY; // mantenemos y constante
        transform.position = targetPos;
    }
}
