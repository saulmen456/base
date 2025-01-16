// DialogueManager.cs (modificado)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public Transform responseButtonParent;
    public GameObject responseButtonPrefab;
    public Dialogue startingDialogue;
    public GameObject dialogueBox;

    public float textDisplaySpeed = 0.1f;
    public float timeBeforeClosing = 1f;

    private Dialogue currentDialogue;
    private PlayerActions playerActions;
    private DialogueTrigger currentTrigger;

    void Start()
    {
        playerActions = FindObjectOfType<PlayerActions>();
        if (startingDialogue != null)
        {
            ShowDialogue(startingDialogue);
        }
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(dialogue.dialogueText));

        if (playerActions != null)
        {
            playerActions.DisableMovement();
        }

        foreach (Transform child in responseButtonParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetTrigger(DialogueTrigger trigger)
    {
        currentTrigger = trigger;
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textDisplaySpeed);
        }

        DisplayResponses();
    }

    private void DisplayResponses()
    {
        if (currentDialogue.responsesh.Count > 0)
        {
            foreach (ResponseH responseh in currentDialogue.responsesh)
            {
                GameObject button = Instantiate(responseButtonPrefab, responseButtonParent);
                button.GetComponentInChildren<TextMeshProUGUI>().text = responseh.responseText;
                button.GetComponent<Button>().onClick.AddListener(() => OnResponseSelected(responseh));
            }
        }
        else
        {
            StartCoroutine(CloseDialogueAfterSeconds());
        }
    }

    private void OnResponseSelected(ResponseH selectedResponse)
    {
        Debug.Log($"Respuesta seleccionada: {selectedResponse.responseText} - Valor: {selectedResponse.responseValue}");
        
        // Agregar el valor de la respuesta seleccionada al sistema centralizado
        DialogueSystemManager.Instance.AddResponseValue(selectedResponse.responseValue);

        if (selectedResponse.nextDialogue != null)
        {
            ShowDialogue(selectedResponse.nextDialogue);
        }
        else
        {
            StartCoroutine(CloseDialogueAfterSeconds());
        }
    }


    private IEnumerator CloseDialogueAfterSeconds()
    {
        yield return new WaitForSeconds(timeBeforeClosing);

        if (playerActions != null)
        {
            playerActions.EnableMovement();
        }

        dialogueBox.SetActive(false);

        if (currentTrigger != null)
        {
            currentTrigger.gameObject.SetActive(false);
        }

        // Notificar al DialogueSystemManager que este NPC terminó su diálogo
        DialogueSystemManager.Instance.NotifyDialogueEnded();
    }
}
