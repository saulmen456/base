using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenDarkenEffect : MonoBehaviour
{
    public Image darkOverlay;
    public Transform player;
    public Transform musicCenter;
    public float maxDarknessDistance = 5f;
    private float maxAlpha = 240f / 255f; 

    void Update()
    {
        float distance = Vector3.Distance(player.position, musicCenter.position);
        float darknessLevel = Mathf.Clamp01(distance / maxDarknessDistance) * maxAlpha;
        darkOverlay.color = new Color(0, 0, 0, darknessLevel);
    }
}
