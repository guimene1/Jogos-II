using UnityEngine;

public class Screw : Interactable
{
    public Screwdriver screwdriver;
    public GameObject screwModel;
    public float unscrewTime = 1.5f;

    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 360f;
    public Vector3 unscrewDirection = Vector3.up;
    public float unscrewDistance = 0.1f;

    [Header("Sound Settings")]
    public AudioClip unscrewingSound; // Som contínuo durante a remoção
    public AudioClip screwRemovedSound; // Som quando o parafuso é totalmente removido
    [Range(0, 1)] public float volume = 0.7f;

    private bool isUnscrewing = false;
    private float unscrewProgress = 0f;
    private Vector3 initialPosition;
    private AudioSource audioSource;

    void Start()
    {
        initialPosition = screwModel.transform.localPosition;

        // Adiciona AudioSource se não existir
        if (!TryGetComponent<AudioSource>(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.volume = volume;
    }

    public override string GetInteractionMessage()
    {
        if (screwdriver == null || !screwdriver.IsHeld)
            return "Precisa de chave de fenda";

        return string.IsNullOrEmpty(customMessage) ? "Remover parafuso" : customMessage;
    }

    public override void Interact()
    {
        if (screwdriver != null && screwdriver.IsHeld && !isUnscrewing)
        {
            StartUnscrewing();
        }
    }

    private void StartUnscrewing()
    {
        isUnscrewing = true;
        unscrewProgress = 0f;

        // Toca o som de rotação
        if (unscrewingSound != null)
        {
            audioSource.clip = unscrewingSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (isUnscrewing)
        {
            unscrewProgress += Time.deltaTime;

            // Rotação do parafuso
            screwModel.transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);

            // Movimento para fora
            float progressRatio = unscrewProgress / unscrewTime;
            screwModel.transform.localPosition = initialPosition + unscrewDirection * (unscrewDistance * progressRatio);

            if (unscrewProgress >= unscrewTime)
            {
                CompleteUnscrewing();
            }
        }
    }

    private void CompleteUnscrewing()
    {
        isUnscrewing = false;

        // Para o som de rotação
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Toca o som de parafuso removido
        if (screwRemovedSound != null)
        {
            audioSource.PlayOneShot(screwRemovedSound);
        }

        screwdriver.ScrewRemoved();
        Destroy(gameObject, screwRemovedSound != null ? screwRemovedSound.length : 0f); // Espera o som terminar antes de destruir
    }

    void OnDestroy()
    {
        // Limpa os sons para evitar vazamentos
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}