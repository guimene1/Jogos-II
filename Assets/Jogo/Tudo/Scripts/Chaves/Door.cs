using UnityEngine;

public class Door : Interactable
{
    public string doorID;
    public Animator doorAnimator;
    public string openAnimationParam = "isOpen";
    public bool isLocked = true;

    [Header("Configura��es de �udio")]
    public AudioSource openAudio;
    public AudioSource closeAudio;
    public AudioSource unlockAudio;
    public AudioSource lockedAudio;

    public override string GetInteractionMessage()
    {
        if (isLocked)
        {
            return "Porta trancada";
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
        else if (isLocked)
        {
            // Tocar som de porta trancada quando o jogador tenta abrir sem chave
            if (lockedAudio != null)
            {
                lockedAudio.Play();
            }
        }
    }

    private void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("Porta destrancada: " + gameObject.name);

        // Tocar som de destrancar
        if (unlockAudio != null)
        {
            unlockAudio.Play();
        }
    }

    private void OpenCloseDoor()
    {
        if (doorAnimator != null)
        {
            bool currentState = doorAnimator.GetBool(openAnimationParam);
            doorAnimator.SetBool(openAnimationParam, !currentState);

            // Tocar o som apropriado
            if (!currentState)
            {
                // Porta est� abrindo
                if (openAudio != null)
                {
                    openAudio.Play();
                }
            }
            else
            {
                // Porta est� fechando
                if (closeAudio != null)
                {
                    closeAudio.Play();
                }
            }
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