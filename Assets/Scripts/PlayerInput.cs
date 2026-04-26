using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInput : MonoBehaviour
{
    private Rigidbody2D rb;
    private Movement movement;
    private Health health;
    private PlayerManager pm;
    private CameraControl playerCamera;

    private float horMoveDirection;
    private bool jumpTrue; 
    private Vector3 lookOffset;
 

    [SerializeField] SpriteRenderer sprite;
 
    [Header("Look")]
    [SerializeField] float lookMaxDistance;

    [Header("Pause")]
    [SerializeField] GameObject pauseMenu;
    
 

    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement>();
        playerCamera = GetComponent<CameraControl>();
        health = GetComponent<Health>();
        pm = GetComponent<PlayerManager>();
        playerCamera = GetComponent<CameraControl>();

        pm.setMovementEnabled(true);
        Time.timeScale = 1f;
    }
 
    void FixedUpdate()
    {
        if (!pm.GetIsHurt()) movement.Move(horMoveDirection);

        playerCamera.Look(lookOffset);
    }



    /* --------------- INPUT FUNCTIONS --------------- */
    public void OnMove(InputAction.CallbackContext context)
    {
        horMoveDirection = context.ReadValue<Vector2>().x;
 
        if (horMoveDirection == 1 && pm.getMovementEnabled() == true) //flip right
        {
            sprite.flipX = false;
            pm.SetFacingRight(true);
        }
        if (horMoveDirection == -1 && pm.getMovementEnabled() == true) //flip left
        {
            sprite.flipX = true;
            pm.SetFacingRight(false);
        }
    }
 
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool grounded  = movement.IsGrounded();
            bool touchingWall = movement.TouchWall() && pm.CanWallJump();
 
            if (grounded || touchingWall)
            {
                movement.Jump();
            }
        }
 
        if (context.canceled && rb.linearVelocity.y > 0f)
        {
            movement.Fall();
        }
    }
 
    public void OnLook(InputAction.CallbackContext context)
    {
        if(context.performed && (rb.linearVelocity.x == 0f && rb.linearVelocity.y == 0f))
        {
            lookOffset = new Vector3(0f, context.ReadValue<Vector2>().y * lookMaxDistance, 0f);
        }
    }
 
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Block the attack if still in cooldown
            if (!pm.CanAttack()) return;
 
            movement.anim.SetBool("IsAttacking", true);
 
            // Cycle through attack animations by incrementing the AttackIndex parameter
            int attackIndex = movement.anim.GetInteger("AttackIndex");
            attackIndex = (attackIndex + 1) % 3;
            movement.anim.SetInteger("AttackIndex", attackIndex);
 
            pm.hit();
        }
 
        if (context.canceled)
        {
            movement.anim.SetBool("IsAttacking", false);
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (pm.getMovementEnabled() == true)
            {
                pm.setMovementEnabled(false);
                Time.timeScale = 0f;
            }
            else
            {
                pm.setMovementEnabled(true);
                Time.timeScale = 1f;
            }
            pauseMenu.SetActive(!pauseMenu.activeSelf); // Toggle pause menu visibility
        }
    }


    /* --------------- DAMAGE RESPONSE --------------- */
    public void OnHit()
    {
        pm.TriggerHurt();
    }
}
