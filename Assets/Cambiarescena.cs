using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cambiarescena : MonoBehaviour
{
    private Botones botones; // Referencia al NetworkManager
    private void Start()
    {
        botones = GameObject.FindObjectOfType<Botones>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(CambiarEscenaConPuntaje());
        }
    }

    private IEnumerator CambiarEscenaConPuntaje()
    {
        // Llamar al método FinalScore del script Botones
        botones.FinalScore();

        // Esperar un breve momento para simular la ejecución del guardado
        yield return new WaitForSeconds(1f);

        // Cambiar a la escena "Home"
        SceneManager.LoadScene("Home");
    }
}
