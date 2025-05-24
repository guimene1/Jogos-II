using UnityEngine;

public class Animatable : MonoBehaviour
{
    public Animator objectAnimator;
    public string animationBoolParam;
    public string customMessage;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    [Range(0, 1)] public float volume = 1f;

    public string GetInteractionMessage()
    {
        return string.IsNullOrEmpty(customMessage) ? "Interagir com " + gameObject.name : customMessage;
    }

    public virtual void Interact()
    {
        if (objectAnimator != null)
        {
            bool currentState = objectAnimator.GetBool(animationBoolParam);
            objectAnimator.SetBool(animationBoolParam, !currentState);

            // Toca o som correspondente
            PlaySound(!currentState);

            Debug.Log($"Estado da animação alterado para {(!currentState)} em {gameObject.name}");
        }
        else
        {
            Debug.LogWarning("Animator não encontrado no objeto!");
        }
    }

    private void PlaySound(bool isOpening)
    {
        if (audioSource == null) return;

        AudioClip clipToPlay = isOpening ? openSound : closeSound;
        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay, volume);
        }
    }
}