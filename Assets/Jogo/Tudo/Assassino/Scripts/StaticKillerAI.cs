using UnityEngine;

public class StaticKillerAI : MonoBehaviour
{
    public KillerDifficulty killerDifficulty;
    public Transform player;
    public float attackCooldown = 2f;
    public float attackPauseDuration = 0.5f;

    public AudioClip attackSound;
    public AudioClip idleSound;
    [Range(0, 1)] public float soundVolume = 0.7f;

    private Animator animator;
    private AudioSource sfxSource;
    private AudioSource idleSource;

    private bool isAttacking = false;
    private float attackTimer;

    void Start()
    {
        animator = GetComponent<Animator>();

        sfxSource = gameObject.AddComponent<AudioSource>();
        idleSource = gameObject.AddComponent<AudioSource>();

        foreach (var source in new[] { sfxSource, idleSource })
        {
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = 1f;
            source.maxDistance = 15f;
        }

        if (idleSound != null)
        {
            idleSource.clip = idleSound;
            idleSource.loop = true;
            idleSource.volume = soundVolume * 0.5f;
            idleSource.Play();
        }
    }

    void Update()
    {
        if (!isAttacking)
        {
            DetectAndAttackPlayer();
        }
        else
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                EndAttack();
            }
        }
    }

    void DetectAndAttackPlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= killerDifficulty.attackRange)
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        if (isAttacking) return;

        Debug.Log("Inimigo atacou o jogador!");

        if (attackSound != null)
        {
            sfxSource.spatialBlend = 0f;
            sfxSource.PlayOneShot(attackSound, soundVolume);
            Invoke(nameof(RestoreSpatialBlend), attackSound.length);
        }

        if (player.GetComponent<PlayerHealth>() != null)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(killerDifficulty.attackDamage);
        }

        isAttacking = true;
        attackTimer = attackCooldown;
        animator.SetBool("IsAttacking", true);

        // Rotaciona para olhar o jogador
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
    }

    void RestoreSpatialBlend()
    {
        sfxSource.spatialBlend = 1f;
    }

    void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
    }
}
