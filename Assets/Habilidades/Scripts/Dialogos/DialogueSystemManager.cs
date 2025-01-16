using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueSystemManager : MonoBehaviour
{
    private List<int> allSelectedResponseValues = new List<int>();
    public static DialogueSystemManager Instance;

    [SerializeField] private int totalNPCs = 0;
    [SerializeField] private int talkedToNPCs = 0;

    [Header("Feedback UI")]
    public GameObject feedbackPanel; // Panel que muestra el mensaje final
    [SerializeField] private GameObject[] performanceFaces;

    private NetworkManager networkManager; // Referencia al NetworkManager

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        networkManager = GameObject.FindObjectOfType<NetworkManager>(); // Obtener referencia al NetworkManager
    }

    public void SetTotalNPCs(int npcCount)
    {
        totalNPCs = npcCount;
        Debug.Log($"Total de NPCs a hablar: {totalNPCs}");
    }

    public void AddResponseValue(int responseValue)
    {
        allSelectedResponseValues.Add(responseValue);
        Debug.Log($"Valor de respuesta agregado: {responseValue} - Total acumulado de valores: {allSelectedResponseValues.Count}");
    }

    // Llamado por DialogueManager cuando un NPC termina el diálogo
    public void NotifyDialogueEnded()
    {
        talkedToNPCs++;
        Debug.Log($"NPCs hablados: {talkedToNPCs} de {totalNPCs}");

        if (talkedToNPCs >= totalNPCs)
        {
            ShowFinalScore();
        }
    }

    private int CalculateAverageResponseValue()
    {
        if (allSelectedResponseValues.Count == 0) return 1;

        float sum = 0f;
        foreach (int value in allSelectedResponseValues)
        {
            sum += value;
        }

        // Calculamos el promedio original entre 1 y 3
        float averageValue = sum / allSelectedResponseValues.Count;

        // Escalar el promedio a un rango de 1 a 4
        float scaledAverage = 1 + ((averageValue - 1) / 2) * 3;

        // Redondear el promedio escalado a un entero entre 1 y 4
        return Mathf.Clamp(Mathf.RoundToInt(scaledAverage), 1, 4);
    }

    private void ShowFinalScore()
    {
        int averageScore = CalculateAverageResponseValue();

        foreach (var face in performanceFaces)
        {
            face.SetActive(false);
        }

        switch (averageScore)
        {
            case 1:
                performanceFaces[0]?.SetActive(true); // Cara triste
                break;
            case 2:
                performanceFaces[1]?.SetActive(true); // Cara normal
                break;
            case 3:
                performanceFaces[2]?.SetActive(true); // Cara felíz
                break;
            case 4:
                performanceFaces[2]?.SetActive(true); // Cara felíz
                break;
            default:
                Debug.LogWarning("Puntuación no válida. Intenta de nuevo.");
                break;
        }

        feedbackPanel.SetActive(true); // Activar el panel de retroalimentación

        // Guardar el resultado en la base de datos
        if (networkManager != null)
        {
            int testId = 5; // Reemplaza con el ID del test actual (asegúrate que este ID exista en la tabla Test)
            networkManager.GuardarResultadoEnBaseDeDatos(testId, averageScore);
        }
    }
}