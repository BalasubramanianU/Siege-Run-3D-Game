using UnityEngine;
using UnityEngine.AI;

public class TroopAI : MonoBehaviour
{
    public float attackRange = 1.5f;
    public int attackDamage = 5;
    public float attackCooldown = 1.0f;

    private Transform king;
    private Animator animator;
    private float lastAttackTime = 0f;
    private NavMeshAgent agent;

    void Start()
    {
        king = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (king == null) return;

        float distance = Vector3.Distance(transform.position, king.position);
        agent.SetDestination(king.position);

        bool inAttackRange = distance <= attackRange;
        animator.SetBool("IsAttacking", inAttackRange);
        
        if (inAttackRange)
        {
            agent.isStopped = true;

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // Damage the king
                PlayerHealth playerHealth = king.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                }
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.isStopped = false;
        }
    }
}
