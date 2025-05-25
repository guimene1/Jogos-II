using UnityEngine;

public class Faucet : Interactable
{
    [Header("Faucet Settings")]
    public GameObject objectToDestroy;
    public GameObject objectToSpawn;
    public Transform spawnPosition;

    [Header("Spawn Adjustments")]
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;
    public Vector3 scaleMultiplier = Vector3.one;

    [Header("Audio Settings")]
    public AudioClip faucetInteractionSound;
    private AudioSource audioSource;

    private bool isActive = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public override string GetInteractionMessage()
    {
        return !isActive ? "Torneira já ativada" : "Precisa de água";
    }

    public override void Interact()
    {
        if (!isActive) return;

        if (faucetInteractionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(faucetInteractionSound);
        }

        if (objectToDestroy != null) 
            Destroy(objectToDestroy);

        if (objectToSpawn != null && spawnPosition != null)
        {
            GameObject spawnedObject = Instantiate(
                objectToSpawn,
                spawnPosition.position + positionOffset,
                spawnPosition.rotation * Quaternion.Euler(rotationOffset)
            );

            spawnedObject.transform.localScale = new Vector3(
                spawnedObject.transform.localScale.x * scaleMultiplier.x,
                spawnedObject.transform.localScale.y * scaleMultiplier.y,
                spawnedObject.transform.localScale.z * scaleMultiplier.z
            );
        }

        isActive = false;
    }

    public void ActivateFaucet()
    {
        isActive = true;
    }

    public void DeactivateFaucet()
    {
        isActive = false;
    }
}
