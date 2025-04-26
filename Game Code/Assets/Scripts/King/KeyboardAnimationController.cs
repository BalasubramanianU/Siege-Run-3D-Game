using UnityEngine;

public class KeyboardAnimationController : MonoBehaviour
{
    private Animator animator;
    
    [SerializeField] private string animationBoolName = "IsAttacking";
    
    [SerializeField] private KeyCode activationKey = KeyCode.Space;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject.");
        }
    }

    void Update()
    {
        bool isKeyHeld = Input.GetKey(activationKey);
        
        if (animator != null)
        {
            animator.SetBool(animationBoolName, isKeyHeld);
        }
    }
}