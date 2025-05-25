using UnityEngine;

public class ValveItem : Interactable
{
    [Header("Valve Settings")]
    public string pipeID; // Matches with the Pipe component's ID
    public GameObject valveModel;

    [Header("Positioning")]
    [Tooltip("Drag here the empty Transform where the valve should appear when held")]
    public Transform targetPosition;

    [Tooltip("Fine position adjustments when held")]
    public Vector3 positionOffset = Vector3.zero;

    [Tooltip("Fine rotation adjustments when held")]
    public Vector3 rotationOffset = Vector3.zero;

    [Header("Scale Settings")]
    [Tooltip("Enable to use manual scale instead of auto-calculated scale")]
    public bool useManualScale = false;
    
    [Tooltip("Manual scale values (used if Use Manual Scale is enabled)")]
    public Vector3 manualScale = Vector3.one;
    
    [Tooltip("Scale correction factor (applied to either auto or manual scale)")]
    [Range(0.1f, 10f)]
    public float scaleCorrection = 1f;

    private bool isHeld = false;
    private GameObject heldValveInstance;
    private Vector3 originalSize;

    void Awake()
    {
        originalSize = GetObjectWorldSize(valveModel);
    }

    public override string GetInteractionMessage()
    {
        return string.IsNullOrEmpty(customMessage) ? "Take " + gameObject.name : customMessage;
    }

    public override void Interact()
    {
        if (!isHeld) PickUpValve();
    }

    private void PickUpValve()
    {
        if (targetPosition == null)
        {
            Debug.LogError("Assign an empty Transform to 'targetPosition' in the Inspector!");
            return;
        }

        heldValveInstance = Instantiate(valveModel);
        Vector3 correctedScale;

        if (useManualScale)
        {
            // Use manually set scale with correction factor
            correctedScale = new Vector3(
                manualScale.x * scaleCorrection,
                manualScale.y * scaleCorrection,
                manualScale.z * scaleCorrection
            );
        }
        else
        {
            // Use auto-calculated scale with correction factor
            float sizeCorrectionFactor = CalculateCorrectionFactor();
            correctedScale = valveModel.transform.localScale * sizeCorrectionFactor * scaleCorrection;
        }

        heldValveInstance.transform.SetParent(targetPosition, false);
        heldValveInstance.transform.localPosition = positionOffset;
        heldValveInstance.transform.localRotation = Quaternion.Euler(rotationOffset);
        heldValveInstance.transform.localScale = correctedScale;

        gameObject.SetActive(false);
        isHeld = true;
    }

    private float CalculateCorrectionFactor()
    {
        return 1f / Mathf.Max(originalSize.x, originalSize.y, originalSize.z);
    }

    private Vector3 GetObjectWorldSize(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null) return rend.bounds.size;
        return obj.transform.lossyScale;
    }

    public bool TryPlaceValve(Pipe pipe)
    {
        if (isHeld && pipe.pipeID == this.pipeID)
        {
            if (heldValveInstance != null) Destroy(heldValveInstance);
            Destroy(gameObject);
            pipe.ValvePlaced();
            return true;
        }
        return false;
    }
}