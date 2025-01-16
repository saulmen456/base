using UnityEngine;
using System.Collections;

public class OrientationManager : MonoBehaviour
{
    public enum ScreenOrientationSetting
    {
        Portrait,
        Landscape
    }

    [SerializeField] private ScreenOrientationSetting orientationSetting;

    private void Start()
    {
        StartCoroutine(SetOrientationWithDelay());
    }

    private IEnumerator SetOrientationWithDelay()
    {
        yield return new WaitForSeconds(0.1f); // Espera un pequeño retraso
        SetOrientation();
    }

    private void SetOrientation()
    {
        if (orientationSetting == ScreenOrientationSetting.Portrait)
        {
            Screen.orientation = ScreenOrientation.Portrait;
            Debug.Log("Orientación configurada a Portrait");
        }
        else if (orientationSetting == ScreenOrientationSetting.Landscape)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Debug.Log("Orientación configurada a Landscape");
        }
    }

    // Verificar y actualizar la orientación en cada cuadro (si es necesario)
    private void Update()
    {
        if (orientationSetting == ScreenOrientationSetting.Portrait && Screen.orientation != ScreenOrientation.Portrait)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else if (orientationSetting == ScreenOrientationSetting.Landscape && Screen.orientation != ScreenOrientation.LandscapeLeft)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }
}
