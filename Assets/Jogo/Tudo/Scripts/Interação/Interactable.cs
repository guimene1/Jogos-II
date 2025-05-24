using UnityEngine;
using System;

public class Interactable : MonoBehaviour
{
    public string customMessage;

    public event Action OnInteract;

    public virtual string GetInteractionMessage()
    {
        return string.IsNullOrEmpty(customMessage) ? "Interagir com " + gameObject.name : customMessage;
    }

    public virtual void Interact()
    {
        Debug.Log($"Interagiu com {gameObject.name}");
        OnInteract?.Invoke();
    }
}