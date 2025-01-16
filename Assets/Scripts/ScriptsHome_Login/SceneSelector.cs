// SceneSelector.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelector : MonoBehaviour
{
    [SerializeField] private string[] sceneNames;
    private int selectedSceneIndex = -1;

    public void SelectScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneNames.Length)
        {
            selectedSceneIndex = sceneIndex;
            Debug.Log("Escena seleccionada: " + sceneNames[selectedSceneIndex]);
        }
        else
        {
            Debug.LogWarning("Índice de escena fuera de rango.");
        }
    }

    public void LoadSelectedScene()
    {
        if (selectedSceneIndex >= 0)
        {
            // Comprobar si la escena seleccionada es la número 2 y establecer la orientación en consecuencia
            bool isLandscape = selectedSceneIndex != 2;
            ScreenOrientation(isLandscape);

            string sceneToLoad = sceneNames[selectedSceneIndex];
            SceneManager.LoadScene(sceneToLoad);
            Debug.Log("Cargando escena: " + sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Ninguna escena ha sido seleccionada.");
        }
    }

    public void ScreenOrientation(bool isLandscape)
    {
        if (isLandscape)
            Screen.orientation = UnityEngine.ScreenOrientation.LandscapeLeft;
        else
            Screen.orientation = UnityEngine.ScreenOrientation.Portrait;
    }
}
