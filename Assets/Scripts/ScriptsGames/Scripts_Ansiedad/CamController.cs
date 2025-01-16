using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public Transform player; // Referencia al transform del jugador
    public Vector3 offset; // Offset de la c�mara en relaci�n al jugador
    void Start()
    {
        offset = transform.position - player.position;
    }

   
    void Update()
    {
        Vector3 newPosition = player.position + offset;
        // Opcional: Si solo quieres cambiar el offset en el eje Y
        newPosition.y = player.position.y + offset.y;

        // Asignar la nueva posici�n a la c�mara
        transform.position = newPosition;
    }
}
