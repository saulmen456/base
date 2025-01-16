// NPCManager.cs
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public DialogueTrigger[] npcTriggers; // Array de triggers para los NPCs en la escena

    private void Start()
    {
        // Configuramos el total de NPCs en el DialogueSystemManager
        DialogueSystemManager.Instance.SetTotalNPCs(npcTriggers.Length);
    }
}
