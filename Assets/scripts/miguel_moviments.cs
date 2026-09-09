using UnityEngine;
using UnityEngine.InputSystem;

public class miguel_moviments : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator; // Referencia al Animator
    private float horizontal;
    private bool jumpRequested;
    private bool isFacingRight = true;
    private bool isGrounded;

    [Header("Movimiento")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Detección de Suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Obtenemos el componente Animator
    }

    void Update()
    {
        horizontal = 0f;

        if (Keyboard.current != null)
        {
            // Movimiento horizontal
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                horizontal = -1f;
            }
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                horizontal = 1f;
            }

            // Capturamos el salto solo si está en el suelo
            if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
            {
                jumpRequested = true;
            }
        }

        // Actualizar la animación según el movimiento horizontal
        if (animator != null)
        {
            animator.SetBool("running", horizontal != 0f);
        }

        // Determinar si debemos girar el sprite
        if (horizontal > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontal < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        // Verificar si está tocando el suelo
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // Movimiento Horizontal
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        // Aplicar salto
        if (jumpRequested)
        {
            // Reseteamos la velocidad vertical antes de aplicar el impulso para un salto consistente
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpRequested = false;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar el rango de detección del suelo en el editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}