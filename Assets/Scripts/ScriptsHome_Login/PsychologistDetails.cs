using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PsychologistDetails : MonoBehaviour
{
    [SerializeField] private Text psychologistName; // Campo para mostrar el nombre completo del psic�logo
    [SerializeField] private Image psychologistImage; // Imagen del psic�logo
    [SerializeField] private GameObject loadingIndicator; // Indicador de carga
    private AgendaManager agendaManager;

    void Start()
    {
        agendaManager = FindObjectOfType<AgendaManager>();
        if (psychologistName == null)
        {
            Debug.LogError("El campo de texto psychologistName no est� asignado en el inspector.");
            return;
        }

        if (psychologistImage == null)
        {
            Debug.LogError("El campo de imagen psychologistImage no est� asignado en el inspector.");
            return;
        }

        // Llamar para cargar detalles del psic�logo
        StartCoroutine(CargarDetallesPsicologo());
    }

    private IEnumerator CargarDetallesPsicologo()
    {
        yield return new WaitUntil(() => NetworkManager.Instance != null);

        if (NetworkManager.Instance.usuarioId <= 0)
        {
            Debug.LogWarning("Usuario no identificado en NetworkManager.");
            psychologistName.text = "Psic�logo desconocido";
            yield break;
        }

        // Obtener detalles del psic�logo desde NetworkManager
        PsychologistData psychologistData = NetworkManager.Instance.ObtenerDetallesPsicologo(NetworkManager.Instance.usuarioId);

        if (psychologistData != null)
        {
            psychologistName.text = $"{psychologistData.nombre} {psychologistData.apellidoPaterno} {psychologistData.apellidoMaterno}";

            if (!string.IsNullOrEmpty(psychologistData.foto))
            {
                Debug.Log($"Cargando imagen desde URL: {psychologistData.foto}");
                StartCoroutine(CargarImagen(psychologistData.foto));
            }
            else
            {
                Debug.LogWarning("No se encontr� una URL de imagen v�lida para el psic�logo.");
            }
        }
        else
        {
            psychologistName.text = "Datos del psic�logo";
        }
    }

    private IEnumerator CargarImagen(string url)
    {
        if (loadingIndicator != null)
        {
            loadingIndicator.SetActive(true);
        }

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(false);
            }

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al cargar la imagen del psic�logo: " + request.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                psychologistImage.sprite = sprite;
            }
        }
    }

    public void OnButtonClick()
    {
        // Llamar a la funci�n MovePanelToOverlapParent en AgendaManager
        if (agendaManager != null)
        {
            agendaManager.MovePanelToOverlapParent();
        }
        else
        {
            Debug.LogError("AgendaManager no encontrado.");
        }
    }
}

// Clase para manejar los datos del psic�logo
public class PsychologistData
{
    public string nombre { get; set; }
    public string apellidoPaterno { get; set; }
    public string apellidoMaterno { get; set; }
    public string foto { get; set; }
}