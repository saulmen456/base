using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountDownTimer : MonoBehaviour
{
    float currentTime = 0f;
    public float startingTime = 180;

    [SerializeField] Text countdownText;
    [SerializeField] GameObject losePanel; // Panel de "Perdiste"

    void Start()
    {
        currentTime = startingTime;
        losePanel.SetActive(false); // Asegúrate de que el panel esté oculto al inicio
    }

    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("0");

        if (currentTime <= 5)
        {
            countdownText.color = Color.red; // Cambia el color del texto a rojo
        }

        if (currentTime <= 0)
        {
            currentTime = 0;
            ShowLosePanel(); // Muestra el panel de "Perdiste"
        }
    }

    void ShowLosePanel()
    {
        losePanel.SetActive(true);
        Invoke("RestartScene", 2f); // Reinicia la escena después de 2 segundos
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
