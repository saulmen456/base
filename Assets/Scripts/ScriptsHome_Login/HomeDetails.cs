using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class HomeDetails : MonoBehaviour
{
    [SerializeField] private Text userName; // Referencia al componente de texto
    [SerializeField] private Image perfil; // Referencia al componente de imagen
    [SerializeField] private Image perfil2;

    void Start()
    {
        if (userName == null)
        {
            Debug.LogError("El campo de texto userName no está asignado en el inspector.");
            return;
        }

        if (perfil == null)
        {
            Debug.LogError("El campo de imagen perfil no está asignado en el inspector.");
            return;
        }

        // Obtener y mostrar el nombre del usuario
        if (NetworkManager.Instance != null && !string.IsNullOrEmpty(NetworkManager.Instance.Usuario))
        {
            userName.text = NetworkManager.Instance.Usuario;
            // Obtener URL de la imagen y cargarla
            string imageUrl = NetworkManager.Instance.ObtenerUrlImagenPerfil(NetworkManager.Instance.usuarioId);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                StartCoroutine(CargarImagen(imageUrl));
            }
            else
            {
                Debug.LogWarning("URL de imagen vacía o nula.");
            }
        }
        else
        {
            Debug.LogWarning("No se pudo obtener el usuario desde NetworkManager.");
            userName.text = "Usuario desconocido";
        }
    }

    IEnumerator CargarImagen(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al cargar la imagen: " + request.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                perfil.sprite = sprite;
                perfil2.sprite = sprite;
            }
        }
    }
}
