using UnityEngine;

public class Door : Interactable
{
    public string doorID;
    public Animator doorAnimator;
    public string openAnimationParam = "isOpen";
    public bool isLocked = true;

    public override string GetInteractionMessage()
    {
        if (isLocked)
        {
            return "Porta trancada - necessária chave: " + doorID;
        }
        return string.IsNullOrEmpty(customMessage) ?
            (doorAnimator.GetBool(openAnimationParam) ? "Fechar porta" : "Abrir porta") :
            customMessage;
    }

    public override void Interact()
    {
        PlayerInteraction player = FindFirstObjectByType<PlayerInteraction>();
        if (isLocked && player != null && player.TryUseKeyOnDoor(this))
        {
            UnlockDoor();
            OpenCloseDoor();
        }
        else if (!isLocked)
        {
            OpenCloseDoor();
        }
    }

    private void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("Porta destrancada: " + gameObject.name);
    }

    private void OpenCloseDoor()
    {
        if (doorAnimator != null)
        {
            bool currentState = doorAnimator.GetBool(openAnimationParam);
            doorAnimator.SetBool(openAnimationParam, !currentState);
        }
    }

    public bool CanBeOpenedWith(string keyID)
    {
        return this.doorID == keyID;
    }

    public void Unlock()
    {
        if (!isLocked) return;

        UnlockDoor();
        Debug.Log("Porta destrancada pelo puzzle.");
    }
}
