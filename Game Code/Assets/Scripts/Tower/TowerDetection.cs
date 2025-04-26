using UnityEngine;
using System.Collections;

public class TowerDetection : MonoBehaviour
{
    public float detectionRadius = 5f;
    public string targetTag = "Player";
    public float attackCooldown = 1.5f;
    public Transform firePoint;
    
    private float lastAttackTime;
    private Transform target;
    private bool targetInRange = false;
    private bool hasInitialized = false;
    
    void Start()
    {
        // Initialize attack timer
        lastAttackTime = Time.time; 
        targetInRange = false;
        target = null;
        
        Debug.Log("Tower initialized with detection radius: " + detectionRadius);
        hasInitialized = true;
    }
    
    void Update()
    {
        if (!hasInitialized) return;
        
        FindTarget();
        
        if (targetInRange && target != null)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                FireAtTarget();
                lastAttackTime = Time.time;
                Debug.Log("Tower fired at target: " + target.name);
            }
        }
    }
    
    void FindTarget()
    {
        targetInRange = false;
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(targetTag))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                
                if (distance <= detectionRadius)
                {
                    target = hitCollider.transform;
                    targetInRange = true;
                    
                    Debug.DrawLine(transform.position, target.position, Color.red);
                    
                    break;
                }
            }
        }
        
        if (!targetInRange)
        {
            target = null;
        }
    }
    
    void FireAtTarget()
    {
        if (firePoint != null && target != null)
        {
            ParticleSystem arrowEffect = firePoint.GetComponent<ParticleSystem>();
            
            if (arrowEffect != null)
            {
                Vector3 targetPosition = target.position + new Vector3(0, 1.0f, 0); // Aim at upper body
                Vector3 direction = (targetPosition - firePoint.position).normalized;
                
                firePoint.LookAt(targetPosition);
                
                var mainModule = arrowEffect.main;
                mainModule.startSpeedMultiplier = 15f; 
                
                var shapeModule = arrowEffect.shape;
                shapeModule.shapeType = ParticleSystemShapeType.Cone;
                shapeModule.angle = 3f; 
                
                var emissionModule = arrowEffect.emission;
                emissionModule.rateOverTime = 0; 
                
                ParticleSystem.Burst burst = new ParticleSystem.Burst(0.0f, 3); 
                emissionModule.SetBurst(0, burst);
                
                // Play the particle effect
                arrowEffect.Clear(); 
                arrowEffect.Play();
                
                // Deal damage after a short delay
                StartCoroutine(DealDamageAfterDelay(0.5f, 10));
            }
            else
            {
                Debug.LogError("No ParticleSystem found on firePoint!");
            }
        }
    }
    
    IEnumerator DealDamageAfterDelay(float delay, int damage)
    {
        yield return new WaitForSeconds(delay);
        
        // Deal damage if target is still in range
        if (target != null && targetInRange)
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Dealt " + damage + " damage to " + target.name);
            }
        }
    }
    
    void OnDrawGizmos()
    {
        // Draw the detection radius in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}