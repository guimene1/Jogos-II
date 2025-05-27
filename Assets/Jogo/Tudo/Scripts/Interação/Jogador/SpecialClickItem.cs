using UnityEngine;

public class SpecialClickItem : Interactable
{
    [Header("Special Item Settings")]
    public GameObject itemModel;
    public GameObject objectToDisappear;
    public GameObject objectToSpawn;
    public Transform spawnPosition;
public Vector3 spawnRotation = Vector3.zero;

    [Header("Holding Settings")]
    public Transform targetPosition;
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;
    public float scaleCorrection = 1f;

    [Header("Audio")]
    public AudioClip clickSound; // 🔊 Som do clique
    private AudioSource audioSource;

    private bool isHeld = false;
    private GameObject heldItemInstance;
    private int clickCount = 0;
    private float lastClickTime;
    private float doubleClickDelay = 2f;

    public override string GetInteractionMessage()
    {
        return string.IsNullOrEmpty(customMessage) ? "Pegar " + gameObject.name : customMessage;
    }

    public override void Interact()
    {
        if (!isHeld) PickUpItem();
    }

    private void PickUpItem()
    {
        if (targetPosition == null)
        {
            Debug.LogError("Assign an empty Transform to 'targetPosition' in the Inspector!");
            return;
        }

        heldItemInstance = Instantiate(itemModel);
        heldItemInstance.transform.SetParent(targetPosition, false);
        heldItemInstance.transform.localPosition = positionOffset;
        heldItemInstance.transform.localRotation = Quaternion.Euler(rotationOffset);
        heldItemInstance.transform.localScale *= scaleCorrection;

        // Adiciona um AudioSource no objeto segurado, se não existir
        audioSource = heldItemInstance.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = heldItemInstance.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        gameObject.SetActive(false);
        isHeld = true;

        PlayerInteraction.OnHoldingSpecialItem += HandleSpecialItemInput;
    }

    private void HandleSpecialItemInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 🔊 Toca som de clique se houver
            if (clickSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(clickSound);
            }

            if (Time.time - lastClickTime < doubleClickDelay)
            {
                clickCount++;
                Debug.Log("Click count: " + clickCount);
            }
            else
            {
                clickCount = 1;
            }

            lastClickTime = Time.time;

            if (clickCount >= 4)
            {
                ActivateSpecialAction();
                clickCount = 0;
            }
        }
    }

   private void ActivateSpecialAction()
    {
        if (objectToDisappear != null)
        {
            objectToDisappear.SetActive(false);
        }

        if (objectToSpawn != null && spawnPosition != null)
        {
            // Instancia o objeto e aplica a rotação personalizada
            GameObject spawnedObj = Instantiate(
                objectToSpawn, 
                spawnPosition.position, 
                spawnPosition.rotation * Quaternion.Euler(spawnRotation) // 🔄 Aplica a rotação extra
            );
        }

        // Não destrói o item segurado (opcional)
        clickCount = 0;

        // Toca um som de confirmação (opcional)
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    private void OnDestroy()
    {
        PlayerInteraction.OnHoldingSpecialItem -= HandleSpecialItemInput;
    }
}