using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform[] minigamePositions;
    public float moveDuration = 2f;
    public float delayBeforeMove = 2f;
   // Nueva referencia al MinigameManager

    private Vector3 initialPosition;

    private void Start()
    {
        cameraTransform ??= Camera.main.transform;
        initialPosition = cameraTransform.position;
    }

    public void MoveCameraToMinigame(int minigameIndex)
    {
        if (minigameIndex >= 0 && minigameIndex < minigamePositions.Length)
            StartCoroutine(MoveCamera(minigamePositions[minigameIndex]));
    }

    public void MoveCameraToInitialPosition()
    {
        StartCoroutine(MoveCamera(initialPosition, true)); // Cambiado para pasar un flag
    }

    private IEnumerator MoveCamera(Vector3 targetPosition, bool isReturning = false)
    {
        yield return new WaitForSeconds(delayBeforeMove);
        Vector3 startPosition = cameraTransform.position;
        for (float elapsedTime = 0; elapsedTime < moveDuration; elapsedTime += Time.deltaTime)
        {
            cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            yield return null;
        }
        cameraTransform.position = targetPosition;

        if (isReturning) // Si estamos regresando a la posición inicial
        {
            // Llamar a EndMinigame
        }
    }

    private IEnumerator MoveCamera(Transform targetPosition)
    {
        yield return new WaitForSeconds(delayBeforeMove);
        Vector3 startPosition = cameraTransform.position;
        Vector3 endPosition = new Vector3(targetPosition.position.x, targetPosition.position.y, startPosition.z);
        for (float elapsedTime = 0; elapsedTime < moveDuration; elapsedTime += Time.deltaTime)
        {
            cameraTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            yield return null;
        }
        cameraTransform.position = endPosition;
    }
}
