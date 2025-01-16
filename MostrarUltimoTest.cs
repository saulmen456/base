using UnityEngine;
using UnityEngine.UI;

public class MostrarUltimoTest : MonoBehaviour
{
    public Text nombreUltimoTestText; // Caja de texto para mostrar el nombre del �ltimo test
    private NetworkManager networkManager;

    private void Start()
    {
        // Busca el componente NetworkManager en la escena
        networkManager = FindObjectOfType<NetworkManager>();
        ActualizarNombreUltimoTest();
    }

    public void ActualizarNombreUltimoTest()
    {
        if (networkManager != null && nombreUltimoTestText != null)
        {
            nombreUltimoTestText.text = $"{networkManager.nombreUltimoTest}";
        }
        else
        {
            Debug.LogError("No se encontr� el NetworkManager o la caja de texto no est� asignada.");
        }
    }
}
