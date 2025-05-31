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

    [Header("Click Effects")]
    public GameObject clickEffectPrefab; // Prefab do efeito visual ao clicar
    public float effectDuration = 0.3f; // Duração do efeito
    public Vector3 effectOffset = new Vector3(0, 0, 0.5f); // Posição relativa do efeito

    [Header("Audio")]
    public AudioClip clickSound; // Som para cada clique
    public AudioClip successSound; // Som quando completa a ação especial
    public AudioClip healSound; // Som de cura (novo)
    private AudioSource audioSource;

    [Header("Health Settings")]
    public bool givesHealth = false; // Se o item recupera vida
    public int healthAmount = 25; // Quantidade de vida recuperada

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

        // Configura o AudioSource
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
            // Toca som de clique
            if (clickSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(clickSound);
            }

            // Cria efeito visual
            if (clickEffectPrefab != null && heldItemInstance != null)
            {
                Vector3 effectPosition = heldItemInstance.transform.position + 
                                     heldItemInstance.transform.TransformDirection(effectOffset);
                GameObject effect = Instantiate(clickEffectPrefab, effectPosition, heldItemInstance.transform.rotation);
                Destroy(effect, effectDuration);
            }

            // Contagem de cliques
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

            if (clickCount >= 7)
            {
                ActivateSpecialAction();
                clickCount = 0;
            }
        }
    }

    private void ActivateSpecialAction()
    {
        // Ação de desaparecer objeto
        if (objectToDisappear != null)
        {
            objectToDisappear.SetActive(false);
        }

        // Ação de spawnar objeto
        if (objectToSpawn != null && spawnPosition != null)
        {
            GameObject spawnedObj = Instantiate(
                objectToSpawn, 
                spawnPosition.position, 
                spawnPosition.rotation * Quaternion.Euler(spawnRotation)
            );

            // Configura áudio no objeto spawnado
            AudioSource spawnAudio = spawnedObj.GetComponent<AudioSource>();
            if (spawnAudio == null) spawnAudio = spawnedObj.AddComponent<AudioSource>();
            if (successSound != null) spawnAudio.PlayOneShot(successSound);
        }

        // Ação de curar
        if (givesHealth)
        {
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healthAmount);
                if (healSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(healSound);
                }
            }
        }
        else if (successSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(successSound);
        }

        clickCount = 0;
    }

    private void OnDestroy()
    {
        PlayerInteraction.OnHoldingSpecialItem -= HandleSpecialItemInput;
    }
}