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
    private bool canInteract = true;
    private float cooldownTime = 1f;
    private KeyItem heldKey;

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
            // Prioridade para CustomInteractable
            CustomInteractable custom = hit.collider.GetComponent<CustomInteractable>();
            if (custom != null)
            {
                ShowInteractionUI(true, custom.GetInteractionMessage());

                if (Input.GetKeyDown(interactionKey) && canInteract)
                {
                    custom.Interact();
                    StartCoroutine(Cooldown());
                }
                return;
            }

            // Check for key items
            KeyItem key = hit.collider.GetComponent<KeyItem>();
            if (key != null && heldKey == null)
            {
                currentInteractable = key;
                ShowInteractionUI(true, key.GetInteractionMessage());

                if (Input.GetKeyDown(interactionKey) && canInteract)
                {
                    key.Interact();
                    heldKey = key;
                    StartCoroutine(Cooldown());
                }
                return;
            }

            // Check for doors
            Door door = hit.collider.GetComponent<Door>();
            if (door != null)
            {
                currentInteractable = door;
                ShowInteractionUI(true, door.GetInteractionMessage());

                if (Input.GetKeyDown(interactionKey) && canInteract)
                {
                    door.Interact();
                    StartCoroutine(Cooldown());
                }
                return;
            }

            // Outros Interactables
            currentInteractable = hit.collider.GetComponent<Interactable>();

            if (currentInteractable != null)
            {
                ShowInteractionUI(true, currentInteractable.GetInteractionMessage());

                if (Input.GetKeyDown(interactionKey) && canInteract)
                {
                    currentInteractable.Interact();
                    StartCoroutine(Cooldown());
                }
            }
            else
            {
                Animatable currentAnimatable = hit.collider.GetComponent<Animatable>();
                if (currentAnimatable != null)
                {
                    ShowInteractionUI(true, currentAnimatable.GetInteractionMessage());

                    if (Input.GetKeyDown(interactionKey) && canInteract)
                    {
                        currentAnimatable.Interact();
                        StartCoroutine(Cooldown());
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

    public bool TryUseKeyOnDoor(Door door)
    {
        if (heldKey != null)
        {
            if (heldKey.TryUseKey(door))
            {
                heldKey = null;
                return true;
            }
        }
        return false;
    }

    private IEnumerator Cooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(cooldownTime);
        canInteract = true;
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