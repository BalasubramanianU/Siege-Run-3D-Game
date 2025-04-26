using UnityEngine;

public class KingAttackDetector : MonoBehaviour
{
    public float attackRadius = 2f;
    public int attackDamage = 10;
    public LayerMask towerLayer;
    public LayerMask troopLayer;
    public LayerMask gateLayer;
    public float attackCooldown = 1.0f; 
    private float lastAttackTime = 0f;
    public float soundDelay = 0.7f;

    
    private Animator animator;
    private bool canDealDamage = false;
    private AudioSource[] audioSources;
    private AudioSource swordSwingSound; 
    
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSources = GetComponents<AudioSource>();
        swordSwingSound = audioSources[0]; 
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            
            if (animator == null)
            {
                Debug.LogError("Animator not found on this GameObject or its children!");
            }
        }
    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time >= lastAttackTime + attackCooldown)
        {
            DealDamageToNearbyTargets();
            lastAttackTime = Time.time;
                Invoke(nameof(PlaySwordSwingSound), soundDelay);
    }
}

void PlaySwordSwingSound()
{
    swordSwingSound.Play();
}

void DealDamageToNearbyTargets()
{
    // Damage towers in radius
    Collider[] towers = Physics.OverlapSphere(transform.position, attackRadius, towerLayer);
    foreach (var hitCollider in towers)
    {
        TowerHealth towerHealth = hitCollider.GetComponent<TowerHealth>();
        if (towerHealth != null)
        {
            towerHealth.TakeDamage(attackDamage);
            Debug.Log("King attacked tower: " + hitCollider.name);
        }
    }

    // Damage troops in radius
    Collider[] troops = Physics.OverlapSphere(transform.position, attackRadius, troopLayer);
    foreach (var hitCollider in troops)
    {
        EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(attackDamage);
            Debug.Log("King attacked troop: " + hitCollider.name);
        }
    }

    // Damage gates in radius
    Collider[] gates = Physics.OverlapSphere(transform.position, attackRadius, gateLayer);
    foreach (var hitCollider in gates)
    {
        GateHealth gateHealth = hitCollider.GetComponent<GateHealth>();
        if (gateHealth != null)
        {
            gateHealth.TakeDamage(attackDamage);
            Debug.Log("King attacked gate: " + hitCollider.name);
        }
    }

}

    // Draw attack radius in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}