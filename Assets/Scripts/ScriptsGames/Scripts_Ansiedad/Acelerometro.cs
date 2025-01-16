using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Acelerometro : MonoBehaviour
{
    public int cantidad;
    private Rigidbody rb;
    [SerializeField]private float speed = 2;
    public Text coinText;
    AudioManager audioManager;
    private NetworkManager networkManager; // Referencia al NetworkManager

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cantidad = 0;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        networkManager = GameObject.FindObjectOfType<NetworkManager>(); // Obtener referencia al NetworkManager
    }

  
    void Update()
    {
        Vector3 tilt = Input.acceleration;//me da la aceleracion del celular movimiento del cel.
                                          //
        tilt = Quaternion.Euler(90,0,0)*tilt;//modificar el eje de la y 

        Vector3 force = new Vector3(tilt.x, 0, tilt.z) * speed;

        rb.AddForce(tilt*speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plataforma1"))
        {
            FinalScore();
            StartCoroutine(CambiarEscenaConPuntaje());
        }
        if (other.CompareTag("Moneda"))
        {
            audioManager.PlaySFX(audioManager.button);
            cantidad++;
            coinText.text="Puntos :"+ cantidad.ToString();
            Destroy(other.gameObject);
        }
    }

    public void FinalScore()
    {
        cantidad = cantidad;

        if (networkManager != null)
        {
            int testId = 1; // Reemplaza con el ID del test actual
            networkManager.GuardarResultadoEnBaseDeDatos(testId, cantidad);
        }
    }

    private IEnumerator CambiarEscenaConPuntaje()
    {
        yield return new WaitForSeconds(0.2f);

        // Cambiar a la escena "Home"
        SceneManager.LoadScene("Home");
    }
}
