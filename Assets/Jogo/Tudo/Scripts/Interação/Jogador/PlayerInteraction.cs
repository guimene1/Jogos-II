using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 2.0f;
    public LayerMask interactionLayer;
    public KeyCode interactionKey = KeyCode.E;
    public static event Action OnHoldingSpecialItem;
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;

    private Camera playerCamera;
    private Interactable currentInteractable;
    private bool canInteract = true;
    private float cooldownTime = 1f;
    private KeyItem heldKey;
    private ValveItem heldValve;

    // Novo: Referência ao SpecialClickItem
    private SpecialClickItem heldSpecialClickItem;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        // Novo: Invoca o evento somente se estiver segurando o item especial
        if (heldSpecialClickItem != null)
        {
            OnHoldingSpecialItem?.Invoke();
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange, interactionLayer))
        {
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

            ValveItem valve = hit.collider.GetComponent<ValveItem>();
            if (valve != null && heldValve == null && heldKey == null && heldSpecialClickItem == null)
            {
                currentInteractable = valve;
                ShowInteractionUI(true, valve.GetInteractionMessage());

                if (Input.GetKeyDown(interactionKey) && canInteract)
                {
                    valve.Interact();
                    heldValve = valve;
                    StartCoroutine(Cooldown());
                }
                return;
            }

            Pipe pipe = hit.collider.GetComponent<Pipe>();
            if (pipe != null && heldValve != null)
            {
                ShowInteractionUI(true, "Place valve on pipe");

                if (Input.GetKeyDown(interactionKey) && canInteract)
                {
                    if (TryPlaceValveOnPipe(pipe))
                    {
                        StartCoroutine(Cooldown());
                    }
                }
                return;
            }

            // Verifica se é um item especial
            SpecialClickItem special = hit.collider.GetComponent<SpecialClickItem>();
            if (special != null && heldKey == null && heldValve == null && heldSpecialClickItem == null)
            {
                currentInteractable = special;
                ShowInteractionUI(true, special.GetInteractionMessage());

                if (Input.GetKeyDown(interactionKey) && canInteract)
                {
                    special.Interact();
                    heldSpecialClickItem = special;
                    StartCoroutine(Cooldown());
                }
                return;
            }

            KeyItem key = hit.collider.GetComponent<KeyItem>();
            if (key != null && heldKey == null && heldValve == null && heldSpecialClickItem == null)
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

    public bool TryPlaceValveOnPipe(Pipe pipe)
    {
        if (heldValve != null)
        {
            if (heldValve.TryPlaceValve(pipe))
            {
                heldValve = null;
                return true;
            }
        }
        return false;
    }

    public void ClearHeldSpecialClickItem()
    {
        heldSpecialClickItem = null;
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
