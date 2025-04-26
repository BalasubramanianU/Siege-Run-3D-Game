using UnityEngine;

public class ImprovedCharacterMover : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public Transform characterVisual;
    public Transform cameraTransform;
    public Animator animator;
    private bool wasMoving = false;

    void Start()
    {
        if (characterVisual != null)
        {
            animator = characterVisual.GetComponent<Animator>();
        }
        
        if (animator == null)
        {
            Debug.LogWarning("No Animator component found on characterVisual. Animation will not work.");
        }
    }
    
    
    void Update()
    {
        // Get input values
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down
        
        // Create movement vector based on camera direction
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        
        // Project vectors onto the horizontal plane
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        
        // Calculate move direction relative to camera orientation
        Vector3 moveDirection = (forward * -moveZ + right * -moveX).normalized;
        
        // Check if player is moving (for animation)
        bool isMoving = moveDirection.magnitude > 0.1f;
        
        if (isMoving != wasMoving)
        {
            // Update animation state
            if (animator != null)
            {
                animator.SetBool("IsWalking", isMoving);
            }
            wasMoving = isMoving;
        }
        
        // Move the character
        if (isMoving)
        {
            // Apply movement
            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);
            
            // Rotate character visual to face movement direction
            if (characterVisual != null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                characterVisual.rotation = Quaternion.Slerp(characterVisual.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}