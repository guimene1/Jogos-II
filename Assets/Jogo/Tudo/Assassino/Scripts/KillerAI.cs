using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class KillerAI : MonoBehaviour
{
    public KillerDifficulty killerDifficulty;
    public Transform[] patrolPoints;
    public Transform player;
    public float attackCooldown = 2f;
    public float attackPauseDuration = 0.5f;


    [HideInInspector] public bool isInSafeZone = false;

    public AudioClip attackSound;
    public AudioClip[] footstepSounds;
    public AudioClip idleSound;
    [Range(0, 1)] public float soundVolume = 0.7f;
    public float footstepSoundDelay = 1f;

    private NavMeshAgent agent;
    private Animator animator;

    private AudioSource sfxSource; // Para ataques/passos
    private AudioSource idleSource; // Para som idle

    private bool isChasing;
    private float chaseTimer;
    private float attackTimer;
    private bool isAttacking;
    private Transform currentPatrolPoint;
    private float footstepSoundTimer;
    private bool wasMoving;
    private int lastFootstepIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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

        idleSource.loop = true;
        idleSource.clip = idleSound;
        idleSource.volume = soundVolume * 0.5f;
        idleSource.Play();

        isChasing = false;
        isAttacking = false;
        attackTimer = 0f;
        footstepSoundTimer = 0f;
        wasMoving = false;

        agent.speed = killerDifficulty.moveSpeed;
        SetNextPatrolPoint();
    }

    void Update()
    {

        if (isInSafeZone)
{
    isChasing = false;
    isAttacking = false;
    attackTimer = 0f;
    agent.speed = killerDifficulty.moveSpeed;

    Patrol(); // volta a patrulhar normalmente

    return;
}


        bool isMoving = agent.velocity.magnitude > 0.1f && !isAttacking;
        animator.SetBool("IsMoving", isMoving);

        HandleMovementSounds(isMoving);

        float currentSpeed = agent.velocity.magnitude;
        float normalizedSpeed = currentSpeed / killerDifficulty.moveSpeed;

        if (isChasing)
        {
            normalizedSpeed = currentSpeed / killerDifficulty.chaseSpeed;
            ChasePlayer();
        }
        else
        {
            Patrol();
            DetectPlayer();
        }

        animator.SetFloat("Speed", normalizedSpeed);

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                EndAttack();
            }
        }
    }

    void HandleMovementSounds(bool isMoving)
    {
        if (isMoving)
        {
            if (idleSource.isPlaying) idleSource.Stop();

            float speedFactor = agent.velocity.magnitude / agent.speed;
            float currentDelay = Mathf.Lerp(footstepSoundDelay, footstepSoundDelay * 0.7f, speedFactor);

            if (footstepSoundTimer <= 0 && footstepSounds != null && footstepSounds.Length > 0)
            {
                int randomIndex;
                do
                {
                    randomIndex = Random.Range(0, footstepSounds.Length);
                } while (randomIndex == lastFootstepIndex && footstepSounds.Length > 1);

                if (!sfxSource.isPlaying && !isAttacking)
                {
                    sfxSource.PlayOneShot(footstepSounds[randomIndex], soundVolume);
                    lastFootstepIndex = randomIndex;
                }

                footstepSoundTimer = currentDelay;
            }
            else
            {
                footstepSoundTimer -= Time.deltaTime;
            }

            wasMoving = true;
        }
        else if (wasMoving)
        {
            if (idleSound != null && !idleSource.isPlaying)
            {
                idleSource.Play();
            }

            wasMoving = false;
            footstepSoundTimer = 0f;
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
        agent.isStopped = false;
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f && agent.velocity.sqrMagnitude < 0.01f)
        {
            if (!IsInvoking("SetNextPatrolPoint"))
            {
                Invoke("SetNextPatrolPoint", killerDifficulty.patrolWaitTime);
            }
        }
    }

    void SetNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        Transform nextPoint;
        do
        {
            nextPoint = patrolPoints[Random.Range(0, patrolPoints.Length)];
        } while (nextPoint == currentPatrolPoint);

        currentPatrolPoint = nextPoint;
        agent.SetDestination(currentPatrolPoint.position);
    }

    void DetectPlayer()
{
    if (isInSafeZone) return;

    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    if (distanceToPlayer <= killerDifficulty.detectionRange)
    {
        StartChase();
    }
}

    void StartChase()
    {
        isChasing = true;
        chaseTimer = killerDifficulty.chaseDuration;
        agent.speed = killerDifficulty.chaseSpeed;
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= killerDifficulty.attackRange && !isAttacking)
        {
            AttackPlayer();
        }

        chaseTimer -= Time.deltaTime;
        if (chaseTimer <= 0 || distanceToPlayer > killerDifficulty.detectionRange * 1.5f)
        {
            StopChase();
        }
    }

    public void StopChase()
    {
        isChasing = false;
        agent.speed = killerDifficulty.moveSpeed;
        SetNextPatrolPoint();
    }




    void AttackPlayer()
    {
        if (isAttacking || isInSafeZone) return;

        Debug.Log("Inimigo atacou o jogador!");

        // Reproduzir som de ataque como 2D temporariamente
        if (attackSound != null)
        {
            sfxSource.spatialBlend = 0f; // som 2D
            sfxSource.PlayOneShot(attackSound, soundVolume);
            Invoke(nameof(RestoreSpatialBlend), attackSound.length);
        }

        if (player.GetComponent<PlayerHealth>() != null)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(killerDifficulty.attackDamage);
        }

        isAttacking = true;
        animator.SetBool("IsAttacking", true);
        attackTimer = attackCooldown;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);

        Invoke("ResumeAfterAttack", attackPauseDuration);
    }

    void RestoreSpatialBlend()
    {
        sfxSource.spatialBlend = 1f; // volta para som 3D
    }

    void ResumeAfterAttack()
    {
        if (!isAttacking) return;

        if (attackTimer <= 0)
        {
            EndAttack();
        }
    }


}