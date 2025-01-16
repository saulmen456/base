using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Botones : MonoBehaviour
{
    [SerializeField] private GameObject _boton;  // Referencia al primer botón
    [SerializeField] private GameObject _boton2; // Referencia al segundo botón
    [SerializeField] private GameObject _panel; // Referencia al panel que se va a cerrar
    public int cantidad;
    private NetworkManager networkManager; // Referencia al NetworkManager

    private void Awake()
    {
        networkManager = GameObject.FindObjectOfType<NetworkManager>(); // Obtener referencia al NetworkManager
    }
    // Incrementa la cantidad y cierra el panel
    public void RespuestaBuena()
    {
        cantidad += 1;
        CerrarPanel();
    }

    // Mantiene la cantidad igual y cierra el panel
    public void RespuestaMala()
    {
        CerrarPanel();
    }

    // Cierra el panel
    private void CerrarPanel()
    {
        if (_panel != null)
        {
            _panel.SetActive(false); // Desactiva el panel
        }
        else
        {
            Debug.LogWarning("El panel no está asignado en el inspector.");
        }
    }

    public void FinalScore()
    {
        if (networkManager != null)
        {
            int testId = 2; // Reemplaza con el ID del test actual
            networkManager.GuardarResultadoEnBaseDeDatos(testId, cantidad);
        }
    }
}