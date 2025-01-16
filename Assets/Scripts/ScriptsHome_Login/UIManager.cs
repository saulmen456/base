using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject panel = null;
    [SerializeField] private GameObject m_loginUI = null;

    [SerializeField] private GameObject panel2 = null;

    [SerializeField] private GameObject panel3 = null;

    [Header("-------------------------Login-------------------------")]
    [SerializeField] private InputField m_loginUserNameInput = null;
    [SerializeField] private InputField m_loginPasswordInput = null;

    [Header("-------------------------Registro-------------------------")]
    [SerializeField] private Text m_text = null;
    [SerializeField] private InputField m_userNameInput = null;
    [SerializeField] private InputField m_emailInput = null;
    [SerializeField] private InputField m_password = null;
    [SerializeField] private InputField m_reEnterPassword = null;

    private NetworkManager m_networkManager = null;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        m_networkManager = GameObject.FindObjectOfType<NetworkManager>();
    }

    public void SubmitLogin()
    {
        if (m_loginUserNameInput.text == "" || m_loginPasswordInput.text == "")
        {
            ShowMessage("Asegúrese de llenar todos los campos");
            return;
        }

        ShowMessage("Procesando...");

        m_networkManager.IniciarSesion(m_loginUserNameInput.text, m_loginPasswordInput.text, delegate (Response response)
        {
            ShowMessage(response.message);
            if (response.done)
            {
                // Si la autenticación es exitosa, carga la nueva escena
                SceneManager.LoadScene("Home");
            }
        });
    }

    //--------------------------------------------------------------
    //--------------Intercambio entre login y register--------------
    //--------------------------------------------------------------

    public void ShowLogin()
    {
        panel3.SetActive(false);
        m_loginUI.SetActive(true);
    }

    public void ShowPanel2()
    {
        panel2.SetActive(true);
        panel.SetActive(false);
    }

    public void ShowPanel3()
    {
        panel3.SetActive(true);
        panel2.SetActive(false);
    }

    public void Skip()
    {
        panel.SetActive(false);
        panel2.SetActive(false);
        panel3.SetActive(false);
        m_loginUI.SetActive(true);
    }

    //--------------------------------------------------------------
    //-------------------Desvanecimiento de texto-------------------
    //--------------------------------------------------------------
    private void ShowMessage(string message)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        m_text.text = message;
        m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, 1);

        fadeCoroutine = StartCoroutine(FadeOutText(1f, 0.5f)); // Espera 1 segundo y se desvanece en medio segundo
    }

    private IEnumerator FadeOutText(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        Color originalColor = m_text.color;

        for (float t = 0.01f; t < duration; t += Time.deltaTime)
        {
            m_text.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, t / duration));
            yield return null;
        }

        m_text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
    }
}