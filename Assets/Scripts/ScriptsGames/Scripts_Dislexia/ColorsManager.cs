using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ColorsManager : MonoBehaviour
{
    public SpriteRenderer[] colours;
    private int colourSelect;
    public float stayLit;
    private float stayLitCounter;
    public float waitBetweenLights;
    private float waitBetweenCounter;
    private bool shouldBeLit;
    private bool shouldBeDark;
    private bool waitingForNextSequence; // Bandera para el intervalo de espera
    public List<int> activeSequence;
    private int posInSequence;
    private bool gameActive;
    private int inputInSequence;
    public Text scoreText;
    public Text livesText;
    public GameObject gameOverPanel;

    public AudioClip successSound;
    public AudioClip errorSound;
    private AudioSource audioSource;

    private int lives = 3;
    private List<int> scoresPerLife = new List<int>();
    private int maxRoundsPerLife = 10;
    private int roundsThisLife = 0;
    private float initialStayLit;
    private float initialWaitBetweenLights;
    private Vector3[] initialPositions;
    [SerializeField] private GameObject[] performanceFaces;

    private NetworkManager networkManager; // Referencia al NetworkManager

    public bool GameActive => gameActive;

    void Start()
    {
        gameOverPanel.SetActive(false);
        initialStayLit = stayLit;
        initialWaitBetweenLights = waitBetweenLights;
        audioSource = GetComponent<AudioSource>();
        initialPositions = new Vector3[colours.Length];
        networkManager = GameObject.FindObjectOfType<NetworkManager>(); // Obtener referencia al NetworkManager

        for (int i = 0; i < colours.Length; i++)
        {
            initialPositions[i] = colours[i].transform.position;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = $"Ronda: {roundsThisLife}";
        livesText.text = $"Secuencias restantes: {lives}";
    }

    void Update()
    {
        if (waitingForNextSequence) return;

        if (shouldBeLit || shouldBeDark)
        {
            gameActive = false;
        }

        if (shouldBeLit)
        {
            stayLitCounter -= Time.deltaTime;
            if (stayLitCounter < 0)
            {
                colours[activeSequence[posInSequence]].color = new Color(colours[activeSequence[posInSequence]].color.r, colours[activeSequence[posInSequence]].color.g, colours[activeSequence[posInSequence]].color.b, 0.25f);
                shouldBeLit = false;
                shouldBeDark = true;
                waitBetweenCounter = waitBetweenLights;
                posInSequence++;
            }
        }

        if (shouldBeDark)
        {
            waitBetweenCounter -= Time.deltaTime;
            if (posInSequence >= activeSequence.Count)
            {
                shouldBeDark = false;
                gameActive = true;
            }
            else
            {
                if (waitBetweenCounter < 0)
                {
                    PlayButtonSound(activeSequence[posInSequence]);
                    colours[activeSequence[posInSequence]].color = new Color(colours[activeSequence[posInSequence]].color.r, colours[activeSequence[posInSequence]].color.g, colours[activeSequence[posInSequence]].color.b, 1f);
                    stayLitCounter = stayLit;
                    shouldBeLit = true;
                    shouldBeDark = false;
                }
            }
        }
    }

    public void StartGame()
    {
        lives = 3;
        scoresPerLife.Clear();
        roundsThisLife = 0;
        stayLit = initialStayLit;
        waitBetweenLights = initialWaitBetweenLights;
        gameOverPanel.SetActive(false);
        ResetRound();
        UpdateUI();
    }

    private void ResetRound()
    {
        activeSequence.Clear();
        posInSequence = 0;
        inputInSequence = 0;
        colourSelect = Random.Range(0, colours.Length);
        activeSequence.Add(colourSelect);

        colours[activeSequence[posInSequence]].color = new Color(colours[activeSequence[posInSequence]].color.r, colours[activeSequence[posInSequence]].color.g, colours[activeSequence[posInSequence]].color.b, 1f);
        PlayButtonSound(activeSequence[posInSequence]);

        stayLitCounter = stayLit;
        shouldBeLit = true;
        gameActive = false;
    }

    public void ColourPressed(int whichBttn)
    {
        if (gameActive && lives > 0)
        {
            if (activeSequence[inputInSequence] == whichBttn)
            {
                inputInSequence++;
                if (inputInSequence >= activeSequence.Count)
                {
                    roundsThisLife++;
                    IncreaseSpeed();

                    audioSource.PlayOneShot(successSound);

                    if (roundsThisLife >= maxRoundsPerLife)
                    {
                        EndLife();
                        return;
                    }

                    StartCoroutine(ShuffleButtonsThenNextSequence(1f));
                    UpdateUI();
                }
            }
            else
            {
                audioSource.PlayOneShot(errorSound);
                StartCoroutine(EndLifeWithDelay(1f));
            }
        }
    }

    private IEnumerator ShuffleButtonsThenNextSequence(float delay)
    {
        waitingForNextSequence = true;

        List<Vector3> availablePositions = new List<Vector3>(initialPositions);
        foreach (var button in colours)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector3 newPosition = availablePositions[randomIndex];
            availablePositions.RemoveAt(randomIndex);

            StartCoroutine(MoveButtonToPosition(button, newPosition, 0.5f));
        }

        yield return new WaitForSeconds(delay);

        waitingForNextSequence = false;
        posInSequence = 0;
        inputInSequence = 0;
        colourSelect = Random.Range(0, colours.Length);
        activeSequence.Add(colourSelect);

        colours[activeSequence[posInSequence]].color = new Color(colours[activeSequence[posInSequence]].color.r, colours[activeSequence[posInSequence]].color.g, colours[activeSequence[posInSequence]].color.b, 1f);
        PlayButtonSound(activeSequence[posInSequence]);

        stayLitCounter = stayLit;
        shouldBeLit = true;
        gameActive = false;
    }

    private IEnumerator MoveButtonToPosition(SpriteRenderer button, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = button.transform.position;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            button.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        button.transform.position = targetPosition;
    }

    private IEnumerator EndLifeWithDelay(float delay)
    {
        gameActive = false;
        yield return new WaitForSeconds(delay);

        EndLife();
    }

    private void PlayButtonSound(int buttonIndex)
    {
        ColorBttns button = colours[buttonIndex].GetComponent<ColorBttns>();
        if (button != null && button.buttonSound != null)
        {
            audioSource.PlayOneShot(button.buttonSound);
        }
    }

    private void IncreaseSpeed()
    {
        stayLit = Mathf.Max(0.1f, stayLit * 0.70f);
        waitBetweenLights = Mathf.Max(0.2f, waitBetweenLights * 0.90f);
    }

    private void EndLife()
    {
        scoresPerLife.Add(roundsThisLife);
        roundsThisLife = 0;

        if (lives > 1)
        {
            lives--;
            stayLit = initialStayLit;
            waitBetweenLights = initialWaitBetweenLights;
            ResetRound();
        }
        else
        {
            lives = 0;
            gameActive = false;
            DisplayFinalScore();
            gameOverPanel.SetActive(true);
        }

        UpdateUI();
    }

    private void DisplayFinalScore()
    {
        int averageScore = scoresPerLife.Count > 0
            ? Mathf.RoundToInt(Mathf.Clamp01((float)scoresPerLife.Average() / maxRoundsPerLife) * 3 + 1)
            : 0;

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

        // Guardar el resultado en la base de datos
        if (networkManager != null)
        {
            int testId = 3; // Reemplaza con el ID del test actual
            networkManager.GuardarResultadoEnBaseDeDatos(testId, averageScore);
        }
    }
}