using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpMultiple = 2f; // for super/candy/big bounce jumps
    [SerializeField] private float baseGravity = 1.2f;

    [Header("Grounding (optional)")]
    [SerializeField] private Transform groundCheck;            // optional: if you prefer trigger-based, leave null and call SetGrounded from another script
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer = ~0;

    [Header("Colliders (match your old sizes)")]
    [Tooltip("Small capsule used as the 'ground detection'/lower collider (old capCol[0])")]
    [SerializeField] private CapsuleCollider2D groundCapsule;
    [Tooltip("Main body capsule (old capCol[1])")]
    [SerializeField] private CapsuleCollider2D bodyCapsule;

    // State flags that other scripts can toggle
    public bool IsGrounded { get; private set; }
    public bool IsBigBounce { get; private set; } // set true by collision with BigBounce surface; cleared on exit
    public bool IsRamp { get; private set; }      // set true while on "Ramp" surfaces; cleared on exit

    // Accessors used by animation
    public float VelX => rb.linearVelocity.x;
    public bool Moving { get; private set; }
    public bool DownHeld { get; private set; }

    private Rigidbody2D rb;
    private PlayerAnimations anims;
    private PlayerAudio audioFx;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anims = GetComponent<PlayerAnimations>();     // optional
        audioFx = GetComponent<PlayerAudio>();        // optional
        rb.gravityScale = baseGravity;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // upright by default
    }

    private void Update()
    {
        // --- Input
        float moveInput = Input.GetAxisRaw("Horizontal");
        float vertInput = Input.GetAxisRaw("Vertical");
        bool jumpPressed = Input.GetButtonDown("Jump");

        Moving = !Mathf.Approximately(moveInput, 0f);
        DownHeld = vertInput < 0f;

        // --- Grounding
        // If you still use trigger-based "Ground" enter/exit, you can ignore this block and just call SetGrounded() externally.
        if (groundCheck != null)
        {
            IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // --- Roll / crouch collider logic (matches your old shapes)
        if (Moving && DownHeld)
        {
            // ROLL MODE: free rotation + chunky colliders
            rb.constraints = RigidbodyConstraints2D.None;

            if (groundCapsule != null)
            {
                groundCapsule.size = new Vector2(.85f, .85f);
                groundCapsule.offset = new Vector2(-.02f, 0f);
            }
            if (bodyCapsule != null)
            {
                bodyCapsule.size = new Vector2(.8f, .8f);
                bodyCapsule.offset = new Vector2(-.02f, 0f);
            }

            // Ramp assist (same thresholds you used)
            if (-10f < rb.linearVelocity.y && rb.linearVelocity.y < 12f && IsRamp)
            {
                rb.gravityScale = 10f;
                rb.linearVelocity = new Vector2(moveInput * moveSpeed, 1.3f * rb.linearVelocity.y);
            }
            else
            {
                rb.gravityScale = baseGravity;
            }
        }
        else
        {
            // Upright mode: lock rotation & reset rotation angle
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.rotation = 0f;
            rb.gravityScale = baseGravity;

            if (DownHeld)
            {
                // crouch squish (no lateral move)
                if (bodyCapsule != null)
                {
                    bodyCapsule.size = new Vector2(.8f, .25f);
                    bodyCapsule.offset = new Vector2(-.02f, 0f);
                }
                if (groundCapsule != null)
                {
                    groundCapsule.size = new Vector2(.6f, .3f);
                    groundCapsule.offset = new Vector2(-.02f, -.1f);
                }
            }
            else
            {
                // normal shapes
                if (bodyCapsule != null)
                {
                    bodyCapsule.size = new Vector2(.8f, .8f);
                    bodyCapsule.offset = new Vector2(-.02f, 0f);
                }
                if (groundCapsule != null)
                {
                    groundCapsule.size = new Vector2(.75f, .15f);
                    groundCapsule.offset = new Vector2(-.02f, -.35f);
                }
            }
        }

        // --- Horizontal movement (same as before)
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // --- Jump logic (preserves: super jump when Down+Jump on ground & not moving; BigBounce midair)
        if (jumpPressed)
        {
            if (IsGrounded)
            {
                if (DownHeld && !Moving)
                {
                    // crouch super-jump
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpMultiple * jumpForce);
                    audioFx?.PlayJumpBig(); // OPTIONAL: see PlayerAudio below
                }
                else
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    audioFx?.PlayJump();
                }
            }
            else if (IsBigBounce)
            {
                // midair big bounce jump
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpMultiple * jumpForce);
                audioFx?.PlayJumpBig();
            }
        }

        // --- Animator params (use your existing parameter names)
        if (anims != null)
        {
            anims.UpdateLocomotion(velX: rb.linearVelocity.x,
                                   movementBool: Moving,
                                   downArrow: DownHeld,
                                   grounded: IsGrounded);
        }
    }

    // --- External setters for other scripts (collision handler) ---
    public void SetGrounded(bool grounded) => IsGrounded = grounded;

    public void SetBigBounce(bool active)
    {
        // Your old code multiplied / divided jumpMultiple when entering/exiting.
        // That can lead to drift if called twice; instead, just flip a flag.
        IsBigBounce = active;
    }

    public void SetRamp(bool active) => IsRamp = active;
}
