using System;
using System.Collections;
using System.Data.SqlClient;
using UnityEngine;
using UnityEngine.UI;

public class AgendaManager : MonoBehaviour
{
    public Text fullNameEstudianteText1;
    public Text fullNamePsicologoText1;
    public Text fechaText1;
    public Text horaText1;
    public Text fullNameEstudianteText2;
    public Text fullNamePsicologoText2;
    public Text fechaText2;
    public Text horaText2;

    public Button agendarCitaButton1;
    public Button agendarCitaButton2;

    private int estudianteId;
    private int psicologoId;
    private DateTime fecha1;
    private TimeSpan hora1;
    private DateTime fecha2;
    private TimeSpan hora2;

    public GameObject panelToMove;
    private Vector2 initialPanelPosition;

    public string connectionString = "Data Source=psytest1-aws.c32saqi0apl2.sa-east-1.rds.amazonaws.com;Initial Catalog=PsyTestBD;User ID=adminsql;Password=martin12345;Application Name=PsyTest_App;";

    [Header("Scheduled Appointments Panel")]
    public GameObject scheduledAppointmentPrefab; // Prefab para la cita agendada
    public Transform scheduledAppointmentsParent; // Panel donde se mostrarán las citas agendadas

    void Start()
    {
        // Almacenar la posición inicial del panel
        if (panelToMove != null)
        {
            RectTransform panelRectTransform = panelToMove.GetComponent<RectTransform>();
            initialPanelPosition = panelRectTransform.anchoredPosition;
        }

        // Cargar datos del usuario logeado y configurar las citas
        LoadUserData();
        ConfigureAppointments();

        // Asignar las funciones de agendado a los botones
        agendarCitaButton1.onClick.AddListener(() => AgendarCita(true));
        agendarCitaButton2.onClick.AddListener(() => AgendarCita(false));

    }

    void LoadUserData()
    {
        // Obtener los detalles del usuario logeado desde el NetworkManager
        UserData userData = NetworkManager.Instance.ObtenerDetallesUsuario(NetworkManager.Instance.usuarioId);

        if (userData != null)
        {
            estudianteId = NetworkManager.Instance.usuarioId; // Usar el ID del usuario logeado
            string nombreCompletoEstudiante = $"{userData.nombre} {userData.apellidoPaterno} {userData.apellidoMaterno}";
            fullNameEstudianteText1.text = nombreCompletoEstudiante;
            fullNameEstudianteText2.text = nombreCompletoEstudiante;

            // Obtener datos predeterminados para el psicólogo
            LoadPsychologistData();
        }
        else
        {
            Debug.LogError("No se pudieron cargar los detalles del usuario.");
        }
    }

    void LoadPsychologistData()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string queryPsicologo = "SELECT TOP 1 id, nombre, apellidoPaterno, apellidoMaterno FROM DetallePsicologo ORDER BY id";
            using (SqlCommand commandPsicologo = new SqlCommand(queryPsicologo, connection))
            {
                using (SqlDataReader reader = commandPsicologo.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        psicologoId = reader.GetInt32(0);
                        string nombreCompletoPsicologo = $"{reader.GetString(1)} {reader.GetString(2)} {reader.GetString(3)}";
                        fullNamePsicologoText1.text = nombreCompletoPsicologo;
                        fullNamePsicologoText2.text = nombreCompletoPsicologo;
                    }
                }
            }
        }
    }

    void ConfigureAppointments()
    {
        fecha1 = GetNextAvailableDate(DateTime.Now.Date);
        hora1 = new TimeSpan(9, 0, 0); // Primera cita a las 9:00 AM
        fechaText1.text = fecha1.ToString("yyyy-MM-dd");
        horaText1.text = hora1.ToString(@"hh\:mm");

        fecha2 = fecha1; // La misma fecha para la segunda cita
        hora2 = new TimeSpan(14, 0, 0); // Segunda cita a las 2:00 PM
        fechaText2.text = fecha2.ToString("yyyy-MM-dd");
        horaText2.text = hora2.ToString(@"hh\:mm");
    }

    DateTime GetNextAvailableDate(DateTime startDate)
    {
        DateTime nextDate = startDate;
        while (nextDate.DayOfWeek != DayOfWeek.Monday && nextDate.DayOfWeek != DayOfWeek.Wednesday && nextDate.DayOfWeek != DayOfWeek.Friday)
        {
            nextDate = nextDate.AddDays(1);
        }
        return nextDate;
    }

    public void AgendarCita(bool isFirstAppointment)
    {
        DateTime fechaActual = DateTime.Now.Date;
        DateTime selectedDate;
        TimeSpan selectedHora;

        if (isFirstAppointment)
        {
            selectedDate = DateTime.Parse(fechaText1.text);
            selectedHora = TimeSpan.Parse(horaText1.text);
        }
        else
        {
            selectedDate = DateTime.Parse(fechaText2.text);
            selectedHora = TimeSpan.Parse(horaText2.text);
        }

        if (selectedDate < fechaActual)
        {
            Debug.LogError("No se pueden agendar citas en días anteriores al día actual.");
            return;
        }

        if (selectedDate.DayOfWeek != DayOfWeek.Monday && selectedDate.DayOfWeek != DayOfWeek.Wednesday && selectedDate.DayOfWeek != DayOfWeek.Friday)
        {
            Debug.LogError("Las citas sólo pueden agendarse los lunes, miércoles y viernes.");
            return;
        }

        if (selectedHora != new TimeSpan(9, 0, 0) && selectedHora != new TimeSpan(14, 0, 0))
        {
            Debug.LogError("Las citas sólo pueden ser a las 9:00 AM o a las 2:00 PM.");
            return;
        }

        InsertAppointment(estudianteId, psicologoId, selectedDate, selectedHora, "Disponible", true);
    }

    public void MovePanelToOverlapParent()
    {
        if (panelToMove != null)
        {
            RectTransform panelRectTransform = panelToMove.GetComponent<RectTransform>();
            if (panelRectTransform != null)
            {
                // Obtener la altura del objeto padre
                RectTransform parentRectTransform = panelRectTransform.parent.GetComponent<RectTransform>();
                float parentHeight = parentRectTransform.rect.height;

                // Calcular la posición objetivo (superponerse al objeto padre)
                Vector2 targetPosition = new Vector2(panelRectTransform.anchoredPosition.x, parentHeight / 2 - panelRectTransform.rect.height / 2);

                // Iniciar la corutina para mover el panel
                StartCoroutine(MovePanel(panelRectTransform, targetPosition, 0.5f)); // 0.5f es la duración del movimiento
            }
        }
    }

    public void MovePanelToInitialPosition()
    {
        if (panelToMove != null)
        {
            RectTransform panelRectTransform = panelToMove.GetComponent<RectTransform>();
            if (panelRectTransform != null)
            {
                // Iniciar la corutina para mover el panel a la posición inicial
                StartCoroutine(MovePanel(panelRectTransform, initialPanelPosition, 0.5f)); // 0.5f es la duración del movimiento
            }
        }
    }

    private IEnumerator MovePanel(RectTransform panelRectTransform, Vector2 targetPosition, float duration)
    {
        Vector2 startPosition = panelRectTransform.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            panelRectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null; // Esperar el siguiente frame
        }

        // Asegurarse de que la posición final sea exactamente la deseada
        panelRectTransform.anchoredPosition = targetPosition;
    }

    void InsertAppointment(int estudianteId, int psicologoId, DateTime fecha, TimeSpan hora, string situacion, bool estado)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO Cita (estudianteId, psicologoId, fecha, hora, situacion, fechaRegistro, fechaModificacion, estado) " +
                           "VALUES (@estudianteId, @psicologoId, @fecha, @hora, @situacion, GETDATE(), GETDATE(), @estado)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@estudianteId", estudianteId);
                command.Parameters.AddWithValue("@psicologoId", psicologoId);
                command.Parameters.AddWithValue("@fecha", fecha);
                command.Parameters.AddWithValue("@hora", hora);
                command.Parameters.AddWithValue("@situacion", situacion);
                command.Parameters.AddWithValue("@estado", estado);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        InstantiateScheduledAppointment(estudianteId, psicologoId, fecha, hora);
    }

    void InstantiateScheduledAppointment(int estudianteId, int psicologoId, DateTime fecha, TimeSpan hora)
    {
        // Instanciar el prefab de la cita agendada
        GameObject scheduledAppointment = Instantiate(scheduledAppointmentPrefab, scheduledAppointmentsParent);
        if (scheduledAppointment != null)
        {
            // Obtener y configurar los componentes del prefab
            Text scheduledNameText = scheduledAppointment.transform.Find("NameText").GetComponent<Text>();
            Text scheduledTimeText = scheduledAppointment.transform.Find("TimeText").GetComponent<Text>();
            Text scheduledLocationText = scheduledAppointment.transform.Find("LocationText").GetComponent<Text>();
            Button removeButton = scheduledAppointment.transform.Find("RemoveButton").GetComponent<Button>(); // Asegúrate de que el nombre sea correcto

            // Asignar los valores de la cita
            scheduledNameText.text = "Cita con: " + "Psicólogo ID: " + psicologoId; // Aquí puedes obtener el nombre del psicólogo si lo deseas
            scheduledTimeText.text = "Hora: " + hora.ToString(@"hh\:mm"); // Hora de la cita
            scheduledLocationText.text = "Ubicación: " + "Miraflores"; // Aquí puedes obtener la ubicación real si la tienes

            // Asignar el método al botón de eliminación
            removeButton.onClick.AddListener(() => RemoveScheduledAppointment(scheduledAppointment));
        }
    }

    public void RemoveScheduledAppointment(GameObject appointment)
    {
        Destroy(appointment);
    }
}
