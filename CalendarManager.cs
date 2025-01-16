using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CalendarManager : MonoBehaviour
{
    [Header("Calendar Elements")]
    public Text monthYearText;
    public GameObject dayCellPrefab;
    public Transform gridParent;
    public Color selectedDayColor = Color.green; // Color verde para el día seleccionado
    public Color defaultTextColor = Color.black; // Color negro para los números de los días
    public Color adjacentMonthDayColor = Color.grey;
    public Color normalDayBackgroundColor = Color.white; // Fondo blanco por defecto
    public Color currentDayBackgroundColor = new Color(0.8f, 0.8f, 1.0f); // Fondo azul claro para el día actual
    public GameObject weekDayHeaderPrefab;
    public Button prevMonthButton, nextMonthButton;

    [Header("Appointment Panel Elements")]
    public GameObject appointmentPanel;
    public GameObject appointmentPrefab;
    public Transform appointmentListParent;


    private DateTime currentDate;
    private DateTime lastSelectedDate; // Para mantener un seguimiento del último día seleccionado

    public string connectionString = "Data Source=psytest1-aws.c32saqi0apl2.sa-east-1.rds.amazonaws.com;Initial Catalog=PsyTestBD;User ID=adminsql;Password=martin12345;Application Name=PsyTest_App;";

    void Start()
    {
        currentDate = DateTime.Now;
        lastSelectedDate = DateTime.MinValue; // Inicializar con un valor mínimo
        UpdateCalendar();
        prevMonthButton.onClick.AddListener(() => ChangeMonth(-1));
        nextMonthButton.onClick.AddListener(() => ChangeMonth(1));
        InvokeRepeating(nameof(CheckCurrentDay), 1f, 60f);
    }

    public void UpdateCalendar()
    {
        ClearChildren(gridParent);

        AddWeekDayHeaders();
        monthYearText.text = currentDate.ToString("MMMM yyyy");

        DateTime firstDay = new(currentDate.Year, currentDate.Month, 1);
        int startDay = (int)firstDay.DayOfWeek;
        int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

        AddDaysFromPreviousMonth(startDay, firstDay);
        AddDaysInCurrentMonth(daysInMonth, firstDay.Year, firstDay.Month);
        AddDaysFromNextMonth(42 - (startDay + daysInMonth));
    }

    void AddWeekDayHeaders()
    {
        string[] daysOfWeek = { "DO", "LU", "MA", "MI", "JU", "VI", "SA" };
        foreach (string day in daysOfWeek)
        {
            CreateTextCell(weekDayHeaderPrefab, gridParent, day);
        }
    }

    void AddDaysFromPreviousMonth(int startDay, DateTime firstDay)
    {
        int daysInPrevMonth = DateTime.DaysInMonth(firstDay.AddMonths(-1).Year, firstDay.AddMonths(-1).Month);
        int adjustedStartDay = startDay == 7 ? 0 : startDay;

        for (int i = adjustedStartDay; i > 0; i--)
        {
            CreateDayCell(daysInPrevMonth - i + 1, adjacentMonthDayColor, null);
        }
    }

    void AddDaysInCurrentMonth(int daysInMonth, int year, int month)
    {
        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime cellDate = new(year, month, day);
            Color textColor = defaultTextColor; // Color negro para los números de los días
            Color backgroundColor = normalDayBackgroundColor; // Fondo blanco por defecto

            if (cellDate == DateTime.Now.Date)
            {
                backgroundColor = currentDayBackgroundColor; // Fondo azul claro para el día actual
            }

            CreateDayCell(day, textColor, cellDate, backgroundColor);
        }
    }

    void AddDaysFromNextMonth(int remainingCells)
    {
        for (int i = 1; i <= remainingCells; i++)
        {
            CreateDayCell(i, adjacentMonthDayColor, null);
        }
    }

    void CreateDayCell(int day, Color textColor, DateTime? date, Color backgroundColor = default)
    {
        GameObject dayCell = Instantiate(dayCellPrefab, gridParent);
        Text dayText = dayCell.GetComponentInChildren<Text>();
        dayText.text = day.ToString();
        dayText.color = textColor;

        Image bgImage = dayCell.GetComponent<Image>();
        if (backgroundColor != Color.clear)
        {
            bgImage.color = backgroundColor;
        }
        else
        {
            bgImage.color = normalDayBackgroundColor; // Fondo blanco por defecto
        }

        if (date.HasValue)
        {
            dayCell.GetComponent<Button>().onClick.AddListener(() =>
            {
                // Restablecer el color del fondo del último día seleccionado
                if (lastSelectedDate != DateTime.MinValue)
                {
                    Transform lastDayCell = GetDayCell(lastSelectedDate);
                    if (lastDayCell != null)
                    {
                        Image lastBgImage = lastDayCell.GetComponent<Image>();
                        if (lastSelectedDate == DateTime.Now.Date)
                        {
                            lastBgImage.color = currentDayBackgroundColor; // Fondo azul claro para el día actual
                        }
                        else
                        {
                            lastBgImage.color = normalDayBackgroundColor; // Fondo blanco por defecto
                        }
                    }
                }

                // Resaltar el día seleccionado en verde
                dayCell.GetComponent<Image>().color = selectedDayColor;
                lastSelectedDate = date.Value;

                if (date.Value.DayOfWeek == DayOfWeek.Monday ||
                    date.Value.DayOfWeek == DayOfWeek.Wednesday ||
                    date.Value.DayOfWeek == DayOfWeek.Friday)
                {
                    Debug.Log("Día seleccionado: " + date.Value.ToString("yyyy-MM-dd"));
                    appointmentPanel.SetActive(true);
                    ShowAppointments(date.Value);
                }
                else
                {
                    appointmentPanel.SetActive(false);
                    ClearChildren(appointmentListParent);
                    Debug.Log("Día seleccionado sin citas: " + date.Value.ToString("yyyy-MM-dd"));
                }
            });
        }
    }

    void ChangeMonth(int increment)
    {
        currentDate = currentDate.AddMonths(increment);
        UpdateCalendar();
    }

    void CheckCurrentDay()
    {
        DateTime now = DateTime.Now;
        if (currentDate.Date != now.Date)
        {
            currentDate = now;
            UpdateCalendar();
        }
    }

    void ShowAppointments(DateTime date)
    {
        ClearChildren(appointmentListParent);

        NetworkManager networkManager = NetworkManager.Instance;
        if (networkManager == null)
        {
            Debug.LogError("NetworkManager is not initialized.");
            return;
        }

        List<Appointment> dailyAppointments = networkManager.ObtenerCitas(date);
        if (dailyAppointments == null || dailyAppointments.Count == 0)
        {
            Debug.Log("No appointments found for the selected date.");
            return;
        }

        foreach (var app in dailyAppointments)
        {
            // Obtener detalles del psicólogo
            PsychologistData psychologistData = networkManager.ObtenerDetallesPsicologo(app.PsychologistId);

            GameObject appCard = Instantiate(appointmentPrefab, appointmentListParent);
            if (appCard == null)
            {
                Debug.LogError("Failed to instantiate appointmentPrefab.");
                continue;
            }

            Text nameText = appCard.transform.Find("NameText").GetComponent<Text>();
            Text horaText = appCard.transform.Find("Hora").GetComponent<Text>();
            Text ubicacionText = appCard.transform.Find("Ubicación").GetComponent<Text>();
            Image profileImage = appCard.transform.Find("ImagenDePerfil").GetComponent<Image>();

            if (nameText != null) nameText.text = app.Name;
            if (horaText != null) horaText.text = app.Time;
            if (ubicacionText != null) ubicacionText.text = app.Location;

            // Cargar imagen del psicólogo si está disponible
            if (psychologistData != null && !string.IsNullOrEmpty(psychologistData.foto))
            {
                StartCoroutine(CargarImagen(psychologistData.foto, profileImage));
            }

            Button scheduleButton = appCard.transform.Find("Agendar").GetComponent<Button>();
            if (scheduleButton != null)
            {
                scheduleButton.GetComponentInChildren<Text>().text = "Agendar";
            }
        }
    }

    private IEnumerator CargarImagen(string url, Image image)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al cargar la imagen del psicólogo: " + request.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                image.sprite = sprite;
            }
        }
    }


    void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    void CreateTextCell(GameObject prefab, Transform parent, string text)
    {
        GameObject headerCell = Instantiate(prefab, parent);
        headerCell.GetComponentInChildren<Text>().text = text;
    }

    // Método para obtener la celda del día correspondiente a una fecha
    Transform GetDayCell(DateTime date)
    {
        foreach (Transform dayCell in gridParent)
        {
            Text dayText = dayCell.GetComponentInChildren<Text>();
            if (dayText != null && date.Day.ToString() == dayText.text)
            {
                return dayCell;
            }
        }
        return null;
    }

    [Serializable]
    public class Appointment
    {
        public string Name;
        public string Time;
        public string Location;
        public string EstudianteId;
        public string PsicologoId;
        public string Date;
        public Texture2D ProfileImage;
        public int PsychologistId;
    }
}