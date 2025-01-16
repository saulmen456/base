using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeckCanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    public void Pausa()
    {
        Time.timeScale = 0f; 
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;      
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }
    public void Salir()
    {
        // Cambiar orientación a Portrait antes de cargar la escena
        Screen.orientation = ScreenOrientation.Portrait;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Home");
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void FinJuego()
    {
        Application.Quit();
    }
}
