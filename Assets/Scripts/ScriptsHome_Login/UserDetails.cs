using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UserDetails : MonoBehaviour
{
    [SerializeField] private Text fullNameText; // Campo para mostrar el nombre completo
    [SerializeField] private Text emailText; // Campo para mostrar el correo institucional
    [SerializeField] private Text personalEmailText; // Campo para mostrar el correo personal
    [SerializeField] private Text dniText; // Campo para mostrar el DNI
    [SerializeField] private Text phoneText; // Campo para mostrar el celular
    [SerializeField] private Text birthDateText; // Campo para mostrar la fecha de nacimiento
    [SerializeField] private Text addressText; // Campo para mostrar la dirección

    void Start()
    {
        if (fullNameText == null || emailText == null || personalEmailText == null ||
            dniText == null || phoneText == null || birthDateText == null || addressText == null)
        {
            Debug.LogError("Uno o más campos de texto no están asignados en el inspector.");
            return;
        }

        // Llamar a la función para cargar los detalles del usuario
        StartCoroutine(CargarDetallesUsuario());
    }

    private IEnumerator CargarDetallesUsuario()
    {
        // Esperar a que NetworkManager esté inicializado
        yield return new WaitUntil(() => NetworkManager.Instance != null);

        if (NetworkManager.Instance.usuarioId <= 0)
        {
            Debug.LogWarning("Usuario no identificado en NetworkManager.");
            fullNameText.text = "Nombre no disponible";
            emailText.text = "Correo no disponible";
            personalEmailText.text = "Correo personal no disponible";
            dniText.text = "DNI no disponible";
            phoneText.text = "Celular no disponible";
            birthDateText.text = "Fecha de nacimiento no disponible";
            addressText.text = "Dirección no disponible";
            yield break;
        }

        // Obtener detalles del usuario desde la base de datos
        UserData userData = NetworkManager.Instance.ObtenerDetallesUsuario(NetworkManager.Instance.usuarioId);

        if (userData != null)
        {
            fullNameText.text = $"{userData.nombre} {userData.apellidoPaterno} {userData.apellidoMaterno}";
            emailText.text = userData.correoInstitucional;
            personalEmailText.text = userData.correoPersonal;
            dniText.text = userData.dni;
            phoneText.text = userData.celular;
            birthDateText.text = userData.fechaNacimiento.ToString("dd/MM/yyyy");
            addressText.text = userData.direccion;
        }
        else
        {
            fullNameText.text = "Nombre no disponible";
            emailText.text = "Correo no disponible";
            personalEmailText.text = "Correo personal no disponible";
            dniText.text = "DNI no disponible";
            phoneText.text = "Celular no disponible";
            birthDateText.text = "Fecha de nacimiento no disponible";
            addressText.text = "Dirección no disponible";
        }
    }
}

// Clase para manejar los datos del usuario
public class UserData
{
    public string nombre { get; set; }
    public string apellidoPaterno { get; set; }
    public string apellidoMaterno { get; set; }
    public string correoInstitucional { get; set; }
    public string correoPersonal { get; set; }
    public string dni { get; set; }
    public string celular { get; set; }
    public DateTime fechaNacimiento { get; set; }
    public string direccion { get; set; }
}
