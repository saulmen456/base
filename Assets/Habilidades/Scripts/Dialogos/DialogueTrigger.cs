using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;  // Referencia al DialogueManager


    // Detectar cuando el jugador entra en el trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))  // Verificamos si el objeto que entra es el jugador
        {
            dialogueManager.gameObject.SetActive(true);  // Activar el cuadro de diálogo
            dialogueManager.SetTrigger(this);
        }
    }
}
