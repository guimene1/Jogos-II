using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    private Interactable interactable;
    public Color interactionColor = Color.green;
    private Color originalColor;
    private Renderer objectRenderer;

    void Start()
    {
        interactable = GetComponent<Interactable>();
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        if (interactable != null)
        {
            interactable.OnInteract += ChangeColor;
        }
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.OnInteract -= ChangeColor;
        }
    }

    private void ChangeColor()
    {
        if (objectRenderer != null)
        {
            if (objectRenderer.material.color == originalColor)
            {
                objectRenderer.material.color = interactionColor;
            }
            else
            {
                objectRenderer.material.color = originalColor;
            }
        }
    }
}