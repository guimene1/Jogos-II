using UnityEngine;

[CreateAssetMenu(fileName = "NewKillerDifficulty", menuName = "Game/Enemy/Killer Difficulty")]
public class KillerDifficulty : ScriptableObject
{
    [Header("Atributos do Assassino")]
    public string difficultyName = "Normal";
    public float moveSpeed = 3.5f;
    public float chaseSpeed = 5.5f;
    public int attackDamage = 25;
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float chaseDuration = 10f;
    public float patrolWaitTime = 3f;
}
