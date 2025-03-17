using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 2.0f;
    public LayerMask interactionLayer;
    public KeyCode interactionKey = KeyCode.E;

    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;

    private Camera playerCamera;
    private Interactable currentInteractable;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange, interactionLayer))
        {
            currentInteractable = hit.collider.GetComponent<Interactable>();

            if (currentInteractable != null)
            {
                ShowInteractionUI(true, currentInteractable.GetInteractionMessage());

                if (Input.GetKeyDown(interactionKey))
                {
                    currentInteractable.Interact();
                }
            }
            else
            {
                Animatable currentAnimatable = hit.collider.GetComponent<Animatable>();
                if (currentAnimatable != null)
                {
                    ShowInteractionUI(true, currentAnimatable.GetInteractionMessage());

                    if (Input.GetKeyDown(interactionKey))
                    {
                        currentAnimatable.Interact();
                    }
                }
            }
        }
        else
        {
            currentInteractable = null;
            ShowInteractionUI(false);
        }
    }

    private void ShowInteractionUI(bool state, string message = "")
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(state);

            if (state && interactionText != null)
            {
                interactionText.text = message;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}