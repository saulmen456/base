using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioDistortionController : MonoBehaviour
{
    public Transform player;
    public Transform musicCenter;
    public AudioMixer audioMixer;
    public float maxDistortionDistance = 5f;

    void Update()
    {
        float distance = Vector3.Distance(player.position, musicCenter.position);
        float distortionLevel = Mathf.Clamp01(distance / maxDistortionDistance);

        audioMixer.SetFloat("Pitch", Mathf.Lerp(1.0f, 0.2f, distortionLevel));
        audioMixer.SetFloat("Lowpass", Mathf.Lerp(1.0f, 0.1f, distortionLevel));
    }
}
