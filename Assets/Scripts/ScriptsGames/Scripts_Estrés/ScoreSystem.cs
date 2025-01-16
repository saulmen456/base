using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    public Transform player;
    public Transform musicCenter;
    public Text scoreText;
    public GameObject gameOverPanel; // Panel de Game Over

    private float score = 0;
    private float timeOffPosition = 0;
    public float maxAllowedDeviationTime = 5f;
    public float scoreIncrement = 10f;
    private bool isGameOver = false;
    private float totalTime = 45f;
    private float gameTimeElapsed = 0f;
    [SerializeField] private GameObject[] performanceFaces;

    private NetworkManager networkManager; // Referencia al NetworkManager

    void Start()
    {
        gameOverPanel.SetActive(false);
        networkManager = GameObject.FindObjectOfType<NetworkManager>(); // Obtener referencia al NetworkManager
    }

    void Update()
    {
        if (isGameOver) return;

        gameTimeElapsed += Time.deltaTime;

        float distance = Vector3.Distance(player.position, musicCenter.position);
        if (distance < 1.0f)
        {
            timeOffPosition = 0;
            score += scoreIncrement * Time.deltaTime;
        }
        else
        {
            timeOffPosition += Time.deltaTime;
            if (timeOffPosition > maxAllowedDeviationTime || gameTimeElapsed >= totalTime)
            {
                GameOver();
            }
        }
        scoreText.text = "Score: " + Mathf.FloorToInt(score);
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over! Final Score: " + Mathf.FloorToInt(score));

        gameOverPanel.SetActive(true);

        FindObjectOfType<AcelerometroEstrés>().enabled = false;
        FindObjectOfType<CircleMovement>().enabled = false;

        DisplayFinalScore();
    }

    private void DisplayFinalScore()
    {
        float normalizedScore = Mathf.Clamp01(score / (totalTime * scoreIncrement));
        int finalScore = Mathf.RoundToInt(normalizedScore * 3) + 1;

        foreach (var face in performanceFaces)
        {
            face.SetActive(false);
        }

        switch (finalScore)
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

        // Guardar el resultado en la base de datos
        if (networkManager != null)
        {
            int testId = 4; // Reemplaza con el ID del test actual (asegúrate que este ID exista en la tabla Test)
            networkManager.GuardarResultadoEnBaseDeDatos(testId, finalScore);
        }
    }
}