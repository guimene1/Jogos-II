using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction2 : MonoBehaviour
{
    void Start()
    {
     public float interactionRange = 2.0f;
    public LayerMask interactionLayer;
    public KeyCode interactionKey = KeyCode.E;

    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;
    }

    void Update()
    {
    Collider[] interactableInRange = Physics.OverlapSphere
        (Transform.position, interactionRange, interactionLayer);   
    }
    if(interactableInRange.Length > 0)
{
    currentInteractable = interactableInRange[0].GetComponent<interactable>();
}
        if( currentInteractable != null)
{
    ShowInteractionUI(true, currentInteractable.GetinteractionMessage());

    if (Input.GetKeyDown(interactionKey))
    {
        currentInteractable.Interact();
    }
    else
    {

    }
}
}
