using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class KillerAI : MonoBehaviour
{
    public KillerDifficulty killerDifficulty;
    public Transform[] patrolPoints;
    public Transform player;
    public float attackCooldown = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private bool isChasing;
    private float chaseTimer;
    private float attackTimer;
    private bool isAttacking;
    private Transform currentPatrolPoint;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        isChasing = false;
        isAttacking = false;
        attackTimer = 0f;
        agent.speed = killerDifficulty.moveSpeed;

        SetNextPatrolPoint();
    }

    void Update()
    {
        // Controla a animação baseada no movimento
        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("IsMoving", isMoving);

        // Ajusta a velocidade da animação baseada na velocidade atual
        float currentSpeed = agent.velocity.magnitude;
        float normalizedSpeed = currentSpeed / killerDifficulty.moveSpeed; // Normaliza para velocidade de patrulha

        if (isChasing)
        {
            normalizedSpeed = currentSpeed / killerDifficulty.chaseSpeed; // Ajusta para velocidade de perseguição
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
                isAttacking = false;
            }
        }
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
        }
        while (nextPoint == currentPatrolPoint);

        currentPatrolPoint = nextPoint;
        agent.SetDestination(currentPatrolPoint.position);

        Debug.Log("Novo ponto de patrulha: " + currentPatrolPoint.position);
    }

    void DetectPlayer()
    {
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

    void StopChase()
    {
        isChasing = false;
        agent.speed = killerDifficulty.moveSpeed;
        SetNextPatrolPoint();
    }

    void AttackPlayer()
    {
        Debug.Log("Inimigo atacou o jogador!");
        player.GetComponent<PlayerHealth>().TakeDamage(killerDifficulty.attackDamage);

        isAttacking = true;
        attackTimer = attackCooldown;
    }
}
