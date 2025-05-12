using UnityEngine;

public class Screwdriver : Interactable
{
    [Header("Screwdriver Settings")]
    public GameObject screwdriverModel;
    public int screwsRequiredToDisappear = 4;

    [Header("Positioning")]
    public Transform targetPosition;
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;
    public float scaleCorrection = 1f;

    private GameObject heldScrewdriverInstance;
    private Vector3 originalSize;
    private int screwsRemovedCount = 0;
    public bool IsHeld { get; private set; } = false;

    void Awake()
    {
        originalSize = GetObjectWorldSize(screwdriverModel);
    }

    public override string GetInteractionMessage()
    {
        return string.IsNullOrEmpty(customMessage) ? "Pegar chave de fenda" : customMessage;
    }

    public override void Interact()
    {
        if (!IsHeld) PickUpScrewdriver();
    }

    private void PickUpScrewdriver()
    {
        if (targetPosition == null)
        {
            Debug.LogError("Assign an empty Transform to 'targetPosition' in the Inspector!");
            return;
        }

        heldScrewdriverInstance = Instantiate(screwdriverModel);
        float sizeCorrectionFactor = CalculateCorrectionFactor();
        Vector3 correctedScale = screwdriverModel.transform.localScale * sizeCorrectionFactor * scaleCorrection;

        heldScrewdriverInstance.transform.SetParent(targetPosition, false);
        heldScrewdriverInstance.transform.localPosition = positionOffset;
        heldScrewdriverInstance.transform.localRotation = Quaternion.Euler(rotationOffset);
        heldScrewdriverInstance.transform.localScale = correctedScale;

        gameObject.SetActive(false);
        IsHeld = true;
    }

    public void ScrewRemoved()
    {
        screwsRemovedCount++;
        
        if (screwsRemovedCount >= screwsRequiredToDisappear)
        {
            if (heldScrewdriverInstance != null)
                Destroy(heldScrewdriverInstance);
            
            Destroy(gameObject);
        }
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
}