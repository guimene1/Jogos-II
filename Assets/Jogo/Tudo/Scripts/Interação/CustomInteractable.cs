using UnityEngine;

public class CustomInteractable : MonoBehaviour
{
    [TextArea]
    public string interactionMessage = "Mensagem padrão de interação";

    public virtual string GetInteractionMessage()
    {
        return interactionMessage;
    }

    public virtual void Interact()
    {
        Debug.Log("Interagiu com " + gameObject.name);
    }
}
