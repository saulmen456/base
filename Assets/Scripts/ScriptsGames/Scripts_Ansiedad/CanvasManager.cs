using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    [SerializeField]private GameObject botonPausa;
    [SerializeField]private GameObject menuPausa;
    AudioManager audioManager;

    private void Awake() 
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void Pausa()
    {
        Time.timeScale = 0f;
        audioManager.PlaySFX(audioManager.button);
        audioManager.Stop(audioManager.bg);
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);
    }
    public void Reanudar()
    {
        Time.timeScale = 1f;
        audioManager.Play(audioManager.bg);
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
