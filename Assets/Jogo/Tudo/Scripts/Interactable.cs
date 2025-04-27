using UnityEngine;
using System;

public class Interactable : MonoBehaviour
{
    public string customMessage;

    public event Action OnInteract;

    public string GetInteractionMessage()
    {
        return string.IsNullOrEmpty(customMessage) ? "Interagir com " + gameObject.name : customMessage;
    }

    public void Interact()
    {
        Debug.Log($"Interagiu com {gameObject.name}");

        OnInteract?.Invoke();
    }
}