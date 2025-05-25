using UnityEngine;

public class Pipe : Interactable
{
    [Header("Pipe Settings")]
    public string pipeID;
    public Transform valvePosition;
    public GameObject valvePrefab;
    public Faucet connectedFaucet;

    [Header("Valve Placement Settings")]
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;
    public float scaleMultiplier = 1f;

    [Header("Audio Settings")]
    public AudioClip valvePlacementSound;
    private AudioSource audioSource;

    private bool hasValve = false;
    private GameObject placedValve;

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
        return hasValve ? "Valvula colocada" : "Precisa de valvula";
    }

    public override void Interact()
    {
        Debug.Log("This pipe " + (hasValve ? "has" : "needs") + " a valve");
    }

    public void ValvePlaced()
    {
        if (hasValve || valvePrefab == null || valvePosition == null) return;

        placedValve = Instantiate(valvePrefab, valvePosition.position, valvePosition.rotation);
        placedValve.transform.SetParent(valvePosition);
        placedValve.transform.localPosition = positionOffset;
        placedValve.transform.localRotation = Quaternion.Euler(rotationOffset);
        placedValve.transform.localScale = valvePrefab.transform.localScale * scaleMultiplier;

        Collider valveCollider = placedValve.GetComponent<Collider>();
        if (valveCollider != null) valveCollider.enabled = false;

        Interactable valveInteractable = placedValve.GetComponent<Interactable>();
        if (valveInteractable != null) valveInteractable.enabled = false;

        placedValve.SetActive(true);

        hasValve = true;
        connectedFaucet?.ActivateFaucet();

        if (valvePlacementSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(valvePlacementSound);
        }

        Debug.Log("Valve placed on pipe " + pipeID);
    }

    public void RemoveValve()
    {
        if (!hasValve || placedValve == null) return;

        Destroy(placedValve);
        hasValve = false;
        connectedFaucet?.DeactivateFaucet();
    }
}
