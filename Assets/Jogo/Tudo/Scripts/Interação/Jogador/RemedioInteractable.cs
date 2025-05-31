using UnityEngine;

public class RemedioInteractable : Interactable
{
    public int healAmount = 25; // Quantidade de vida a recuperar
    public AudioClip somRemedio; // Som ao usar o remédio
    public GameObject efeitoVisual; // Partícula ou efeito visual (opcional)

    private PlayerHealth playerHealth;
    private AudioSource audioSource;

    private void Start()
    {
        customMessage = "Usar remédio";
        playerHealth = FindObjectOfType<PlayerHealth>();
        
        // Adiciona um AudioSource se não existir
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public override void Interact()
    {
        base.Interact();

        if (playerHealth != null)
        {
            // Toca o som
            if (somRemedio != null)
            {
                audioSource.PlayOneShot(somRemedio);
            }

            // Ativa efeito visual (se existir)
            if (efeitoVisual != null)
            {
                Instantiate(efeitoVisual, transform.position, Quaternion.identity);
            }

            playerHealth.Heal(healAmount);
            
            // Desativa o collider para evitar interações repetidas
            GetComponent<Collider>().enabled = false;
            
            // Destroi após o som terminar (se houver som)
            if (somRemedio != null)
            {
                Destroy(gameObject, somRemedio.length);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}