using UnityEngine;

[System.Serializable]
public class ResponseH
{
    public string responseText; // Texto de la respuesta
    public Dialogue nextDialogue; // El siguiente diálogo que se muestra al elegir esta respuesta
    public int responseValue;
}
