using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleController : MonoBehaviour
{
    public RectTransform circle;  
    public RectTransform[] buttons;  
    public RectTransform[] panels;  
    public RectTransform canvasRectTransform;  
    public float slideSpeed = 5f;  
    public float exitSmoothingTime = 0.3f; 

    private Vector3 targetPosition, targetPanelPosition, velocity = Vector3.zero, previousPanelVelocity = Vector3.zero;   
    private RectTransform activePanel, previousPanel;  
    private bool dimensionsChanged = false;  // Bandera para detectar cambio de dimensiones

    void Start()
    {
        if (buttons.Length > 0)
        {
            targetPosition = buttons[0].position;
            circle.position = targetPosition;
        }

        if (panels.Length > 0)
        {
            activePanel = panels[0];
            RepositionActivePanel();  // Asegura que el primer panel esté centrado
        }
    }

    void Update()
    {
        circle.position = Vector3.Lerp(circle.position, targetPosition, Time.deltaTime * slideSpeed);

        if (activePanel != null)
            activePanel.position = Vector3.SmoothDamp(activePanel.position, targetPanelPosition, ref velocity, 0.3f);

        if (previousPanel != null)
        {
            Vector3 exitPosition = new Vector3(-canvasRectTransform.rect.width * 1.05f, previousPanel.position.y, previousPanel.position.z);
            previousPanel.position = Vector3.SmoothDamp(previousPanel.position, exitPosition, ref previousPanelVelocity, exitSmoothingTime);

            if (Vector3.Distance(previousPanel.position, exitPosition) < 0.1f)
            {
                previousPanel.gameObject.SetActive(false);
                previousPanel = null;
            }
        }

        // Si hubo un cambio de dimensiones, reposiciona el panel
        if (dimensionsChanged)
        {
            RepositionActivePanel();
            dimensionsChanged = false;  // Reinicia la bandera
        }
    }

    void OnRectTransformDimensionsChange()
    {
        dimensionsChanged = true;  // Activa la bandera para reposicionar en el próximo Update
    }

    public void MoveToButton(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < buttons.Length && panels[buttonIndex] != activePanel)
        {
            targetPosition = buttons[buttonIndex].position;
            ShowPanel(buttonIndex);  
        }
    }

    private void ShowPanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= panels.Length) return;

        if (activePanel != null)
        {
            previousPanel = activePanel;  
            previousPanelVelocity = Vector3.zero;  
        }

        activePanel = panels[panelIndex];
        SetPanelPosition(activePanel, canvasRectTransform.rect.width);  
        targetPanelPosition = new Vector3(0, activePanel.position.y, activePanel.position.z);
        activePanel.gameObject.SetActive(true);
    }

    private void SetPanelPosition(RectTransform panel, float xPosition)
    {
        Vector2 anchoredPosition = panel.anchoredPosition;
        anchoredPosition.x = xPosition;
        anchoredPosition.y = 0;
        panel.anchoredPosition = anchoredPosition;
    }

    private void RepositionActivePanel()
    {
        // Centra el activePanel en el canvas para la orientación actual
        SetPanelPosition(activePanel, 0);
        activePanel.anchoredPosition = new Vector2(0, 0);
        targetPanelPosition = activePanel.position;
    }
}
