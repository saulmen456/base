using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialTimer : MonoBehaviour
{
    public Image timerCircle;
    public float totalTime = 10f;
    private float timeRemaining;
    private ScoreSystem scoreSystem;

    void Start()
    {
        timeRemaining = totalTime;
        scoreSystem = FindObjectOfType<ScoreSystem>();
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerCircle.fillAmount = timeRemaining / totalTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                timerCircle.fillAmount = 0;
                scoreSystem.GameOver();
            }
        }
    }
}
