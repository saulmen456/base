using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBttns : MonoBehaviour
{
    private SpriteRenderer bttnSprite;
    public int thisBttnNum;
    private ColorsManager theGM;
    public AudioClip buttonSound; // Clip de sonido para el botón
    private AudioSource audioSource;

    void Start()
    {
        bttnSprite = GetComponent<SpriteRenderer>();
        theGM = FindObjectOfType<ColorsManager>();
        audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource
    }

    void OnMouseDown()
    {
        // Cambiar color y reproducir sonido solo si el juego está activo
        if (theGM.GameActive)
        {
            bttnSprite.color = new Color(bttnSprite.color.r, bttnSprite.color.g, bttnSprite.color.b, 1f);
            audioSource.PlayOneShot(buttonSound); // Reproduce el sonido del botón
        }
    }

    void OnMouseUp()
    {
        // Cambiar color y notificar el botón presionado solo si el juego está activo
        if (theGM.GameActive)
        {
            bttnSprite.color = new Color(bttnSprite.color.r, bttnSprite.color.g, bttnSprite.color.b, 0.25f);
            theGM.ColourPressed(thisBttnNum);
        }
    }
}
