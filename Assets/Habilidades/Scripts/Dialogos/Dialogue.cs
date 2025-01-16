using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    [TextArea(3, 10)] 
    public string dialogueText; // El texto del diálogo

    public List<ResponseH> responsesh; // Lista de posibles respuestas
}
