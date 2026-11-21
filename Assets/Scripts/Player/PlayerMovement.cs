using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // =========================
    // References
    // =========================
    private Rigidbody2D rb;
    private Vector2 direction;
    private PlayerControls input;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastDirection = Vector2.down;
    public Vector2 LastDirection => lastDirection;

    [SerializeField]
    private float speed = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = new PlayerControls();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
            Debug.LogError($"[PlayerMovement] Missing Rigidbody2D on {gameObject.name}.", this);
        if (animator == null)
            Debug.LogWarning($"[PlayerMovement] Missing Animator on {gameObject.name}; animations will be skipped.", this);
        if (spriteRenderer == null)
            Debug.LogWarning($"[PlayerMovement] Missing SpriteRenderer on {gameObject.name}; flipping will be skipped.", this);
    }

    private void OnEnable()
    {
        input.Gameplay.Enable();
    }

    private void OnDisable()
    {
        input.Gameplay.Disable();
    }

    private void Update()
    {
        direction = input.Gameplay.Move.ReadValue<Vector2>().normalized;
        bool isMoving = direction != Vector2.zero;

        if (isMoving)
        {
            lastDirection = direction;
        }

        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
            animator.SetFloat("moveX", direction.x);
            animator.SetFloat("moveY", direction.y);
            animator.SetFloat("idleX", lastDirection.x);
            animator.SetFloat("idleY", lastDirection.y);
        }

        if (spriteRenderer != null)
        {
            if (direction.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (direction.x < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (rb == null) return;
        rb.linearVelocity = direction * speed;
    }
}
