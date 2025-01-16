using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public Transform player; // Referencia al transform del jugador
    public Vector3 offset; // Offset de la cámara en relación al jugador
    void Start()
    {
        offset = transform.position - player.position;
    }

   
    void Update()
    {
        Vector3 newPosition = player.position + offset;
        // Opcional: Si solo quieres cambiar el offset en el eje Y
        newPosition.y = player.position.y + offset.y;

        // Asignar la nueva posición a la cámara
        transform.position = newPosition;
    }
}
