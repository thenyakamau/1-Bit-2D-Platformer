using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D playerBody;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private float horizontalMovement = 0f;

    [Header("Jumping")]
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private int maxJumps = 2;
    private int jumpsRemaining = 0;

    [Header("WallCheck")]
    [SerializeField] private Transform WallCheckPosition;
    [SerializeField] private Vector2 WallCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private LayerMask wallLayer;

    [Header("WallMovement")]
    [SerializeField] private float wallSlideSpeed = 2f;
    private bool isWallSliding = false;

    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Gravity")]
    [SerializeField] private float baseGravity = 2f;
    [SerializeField] private float maxFallSpeed = 18f;
    [SerializeField] private float fallSpeedMultiplier = 2f;
    private bool isGrounded = true;

    private bool isFacingRight = true;

    // Start is called b efore the first frame update
    private void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        playerBody.velocity = new Vector2(horizontalMovement * moveSpeed, playerBody.velocity.y);
        GroundCheck();
        Gravity();
        Flip();
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        var directionX = playerBody.velocity.x;
        var directiony = playerBody.velocity.y;

        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                // Hold jump button for full jump
                playerBody.velocity = new Vector2(directionX, jumpPower);
                jumpsRemaining--;
            }
            else if (context.canceled)
            {
                // Tap jump button for a half jump
                playerBody.velocity = new Vector2(directionX, directiony / 2);
                jumpsRemaining--;
            }

           
        }
    }

    private void Flip()
    {
        bool checkAction = isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0;
         if (checkAction)
        {
            isFacingRight = !isFacingRight;
            Vector3 lScale = transform.localScale;
            lScale.x *= -1f;
            transform.localScale = lScale;
        }
    }

    private  void WallSlide()
    {

    }

    private void Gravity()
    {
        var directionX = playerBody.velocity.x;
        var directiony = playerBody.velocity.y;

        if (directiony < 0)
        {
            playerBody.gravityScale = baseGravity * fallSpeedMultiplier; // Increases fall speed
            playerBody.velocity = new Vector2(directionX, Mathf.Max(directiony, -maxFallSpeed));
        }else
        {
            playerBody.gravityScale = baseGravity;
        }
    }

    private void GroundCheck()
    {
        bool checkGround = Physics2D.OverlapBox(groundCheckPosition.position, groundCheckSize, 0, groundLayer);
        if (checkGround)
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else isGrounded = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPosition.position, groundCheckSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(WallCheckPosition.position, WallCheckSize);
    }
}
