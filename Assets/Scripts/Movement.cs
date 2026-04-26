using UnityEngine;
using UnityEngine.InputSystem;
public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement Variables")]
    [SerializeField] float moveSpeed = 5;
    [SerializeField] float jumpPower = 15;
    public float jumpCutMultiplier = 0.5f;

    [Header("Grounding")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;
    [SerializeField] Vector2 groundingHitbox;

    [Header("Wall Checker")]
    [SerializeField] Transform wallCheck;
    [SerializeField] Vector2 WallCheckHitbox;

    [Header("Animator")]
    public Animator anim;

    private bool playingFootstep = false;
    public float footstepDelay = 0.5f;

    private void Awake()
    {
        if(TryGetComponent<Rigidbody2D>(out rb))
            {
                Debug.Log("Rigidbody2D Script is Attached");
            }
            else
            {
                Debug.Log("No Rigidbody2D Script Attached");
            }
    }

    private void Update() {
        if (rb.linearVelocity.y < -0.1f)
        {
            anim.SetBool("IsFalling", true);
            anim.SetBool("IsJumping", false);
        }
    }

    public void Move(float horMoveDirection)
    {
        Debug.Log(horMoveDirection);
        anim.SetFloat("Horizontal", Mathf.Abs(horMoveDirection));
        anim.SetBool("IsRunning", horMoveDirection != 0);
        

        rb.linearVelocity = new Vector2 (horMoveDirection * moveSpeed, rb.linearVelocity.y);
        if (horMoveDirection != 0 && IsGrounded() && !playingFootstep)
        {
            StartPlayingFootstep();
        }
        else if (horMoveDirection == 0 || !IsGrounded())
        {
            StopPlayingFootstep();
        }
    }

    public void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower); 
        AudioManager.Instance.PlayRandomAudioClip(gameObject.GetComponent<PlayerManager>().jumpSounds, transform);
        anim.SetBool("IsJumping", true);     
    }

    public void Fall()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        anim.SetBool("IsFalling", true);
        anim.SetBool("IsJumping", false);
    }

    public bool IsGrounded()
    {
        anim.SetBool("IsGrounded", true);
        anim.SetBool("IsFalling", false);
        anim.SetBool("IsJumping", false);
        return Physics2D.OverlapCapsule(groundCheck.position, groundingHitbox, CapsuleDirection2D.Horizontal, 0, groundLayer);
        
    }

    public bool TouchWall()
    {
        return Physics2D.OverlapBox(wallCheck.position, WallCheckHitbox, 0, groundLayer);
    }

    /* --------------- Footstep SFX --------------- */
    private void StartPlayingFootstep()
    {
        playingFootstep = true;
        InvokeRepeating(nameof(PlayFootstep), 0f, footstepDelay);
    }

    private void StopPlayingFootstep()
    {
        playingFootstep = false;
        CancelInvoke(nameof(PlayFootstep));
    }

    private void PlayFootstep()
    {
        AudioManager.Instance.PlayRandomAudioClip(gameObject.GetComponent<PlayerManager>().footstepSounds, transform);
    }
}
