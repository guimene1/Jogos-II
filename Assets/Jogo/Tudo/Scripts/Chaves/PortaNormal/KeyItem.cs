using UnityEngine;

public class KeyItem : Interactable
{
    [Header("Key Settings")]
    public string doorID;
    public GameObject keyModel;

    [Header("Positioning")]
    [Tooltip("Drag here the empty Transform where the key should appear")]
    public Transform targetPosition;

    [Tooltip("Fine position adjustments")]
    public Vector3 positionOffset = Vector3.zero;

    [Tooltip("Fine rotation adjustments")]
    public Vector3 rotationOffset = Vector3.zero;

    [Tooltip("Scale correction factor")]
    public float scaleCorrection = 1f;

    private bool isHeld = false;
    private GameObject heldKeyInstance;
    private Vector3 originalSize;

    void Awake()
    {
        // Calculate the actual size using the mesh bounds
        originalSize = GetObjectWorldSize(keyModel);
    }

    public override string GetInteractionMessage()
    {
        return string.IsNullOrEmpty(customMessage) ? "Pegar " + gameObject.name : customMessage;
    }

    public override void Interact()
    {
        if (!isHeld) PickUpKey();
    }

    private void PickUpKey()
    {
        if (targetPosition == null)
        {
            Debug.LogError("Assign an empty Transform to 'targetPosition' in the Inspector!");
            return;
        }

        // Create key at the target position
        heldKeyInstance = Instantiate(keyModel);

        // Calculate corrected scale
        float sizeCorrectionFactor = CalculateCorrectionFactor();
        Vector3 correctedScale = keyModel.transform.localScale * sizeCorrectionFactor * scaleCorrection;

        // Set parent first (worldPositionStays = false to ignore parent scale)
        heldKeyInstance.transform.SetParent(targetPosition, false);

        // Apply position and rotation relative to parent
        heldKeyInstance.transform.localPosition = positionOffset;
        heldKeyInstance.transform.localRotation = Quaternion.Euler(rotationOffset);

        // Apply corrected scale
        heldKeyInstance.transform.localScale = correctedScale;

        // Disable original key
        gameObject.SetActive(false);
        isHeld = true;
    }

    private float CalculateCorrectionFactor()
    {
        // Compare the actual size with a reference size (1 unit)
        return 1f / Mathf.Max(originalSize.x, originalSize.y, originalSize.z);
    }

    private Vector3 GetObjectWorldSize(GameObject obj)
    {
        // Get the renderer bounds if available
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            return rend.bounds.size;
        }

        // Fallback for objects without renderer
        return obj.transform.lossyScale;
    }

    public bool TryUseKey(Door door)
    {
        if (isHeld && door.doorID == this.doorID)
        {
            if (heldKeyInstance != null) Destroy(heldKeyInstance);
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}