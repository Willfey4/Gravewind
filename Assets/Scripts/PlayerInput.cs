using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private Rigidbody2D rb;
    private Movement movement;
    private Health health;
    private PlayerManager pm;
    private CameraControl playerCamera;

    [SerializeField] SpriteRenderer sprite;

    [Header("Look")]
    [SerializeField] float lookMaxDistance;


    float horMoveDirection;
    bool jumpTrue;

    Vector3 lookOffset;

    void Awake()
    {
        if(TryGetComponent<Rigidbody2D>(out rb))
        {
            Debug.Log("Rigidbody2D Script is Attached");
        }
        else
        {
            Debug.Log("No Rigidbody2D Script Attached");
        }


        if(TryGetComponent<Movement>(out movement))
        {
            Debug.Log("Movement Script is Attached");
        }
        else
        {
            Debug.Log("No Movement Script Attached");
        }


        if(TryGetComponent<CameraControl>(out playerCamera))
        {
            Debug.Log("Camera Script is Attached");
        }
        else
        {
            Debug.Log("No Camera Script Attached");
        }

        if(TryGetComponent<Health>(out health))
        {
            Debug.Log("Health Script is Attached");
        }
        else
        {
            Debug.Log("No Health Script Attached");
        }

        if(TryGetComponent<PlayerManager>(out pm))
        {
            Debug.Log("PlayerManager Script is Attached");
        }
        else
        {
            Debug.Log("No PlayerManager Script Attached");
        }
    }

    void FixedUpdate()
    {
        movement.Move(horMoveDirection);
        playerCamera.Look(lookOffset);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        horMoveDirection = context.ReadValue<Vector2>().x;
        if(horMoveDirection == 1 && pm.getMovementEnabled() == true)
        {
            sprite.flipX = false;
        }
        if(horMoveDirection == -1 && pm.getMovementEnabled() == true)
        {
            sprite.flipX = true;
        }

        Debug.Log("OnMove is called");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed && (movement.IsGrounded() || movement.TouchWall()))
        { 
            Debug.Log("OnJump is performed");
            movement.Jump();
        }

        if(context.canceled && rb.linearVelocity.y > 0f)
        { 
            Debug.Log("OnJump is canceled");
            movement.Fall();
        }
    }

    public void OnLook(InputAction.CallbackContext context) 
    {
        if(context.performed && (rb.linearVelocity.x == 0f && rb.linearVelocity.y == 0f)) 
        { 
            lookOffset = new Vector3 (0f, context.ReadValue<Vector2>().y * lookMaxDistance, 0f);
        }
    }


    public void onAttack(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            movement.anim.SetBool("IsAttacking", true);

            int attackIndex = movement.anim.GetInteger("AttackIndex");
            attackIndex = (attackIndex + 1) % 3; 
            GetComponent<AudioSource>().Play();
            movement.anim.SetInteger("AttackIndex", attackIndex);

            pm.hit();

            Debug.Log("OnAttack is performed");
        }

        if(context.canceled)
        { 
            movement.anim.SetBool("IsAttacking", false);
            Debug.Log("OnAttack is canceled");
        }
        
    }
}
