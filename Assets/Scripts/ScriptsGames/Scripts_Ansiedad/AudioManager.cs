using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;  // Asegúrate de incluir esta línea

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---------- Audio Clip ----------")]
    public AudioClip bg;
    public AudioClip button;
    public AudioClip sphere;

    [Header("---------- Audio Mixer Parameters ----------")]
    [SerializeField] AudioMixer audioMixer; 
    private string frequencyGainParam = "paramEQ_FreqGain";  // Nombre del parámetro expuesto
    [SerializeField] private float currentGain = 0.3f; // Frecuencia inicial
    [SerializeField] private float targetGain = 3f;  //Frecuencia final
    [SerializeField] private float increaseSpeed = 0.3f;  // Velocidad de aumento de Frecuencia

    void Start()
    {
        musicSource.clip = bg;
        musicSource.Play();
        audioMixer.SetFloat(frequencyGainParam, currentGain);
    }

    void Update()
    {
        // Aumentar gradualmente la frecuencia
        if (currentGain < targetGain)
        {
            currentGain += Time.deltaTime * increaseSpeed;
            audioMixer.SetFloat(frequencyGainParam, currentGain);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void Stop(AudioClip clip)
    {
        musicSource.Pause();
    }

    public void Play(AudioClip clip)
    {
        musicSource.UnPause();
    }
}
