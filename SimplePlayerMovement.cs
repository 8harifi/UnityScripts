using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    private float HorizontalMove;
    private Rigidbody2D Rigidbody2D;
    private Vector3 Velocity = Vector3.zero;
    private bool facingRight = true;
    private bool IsGrounded;
    private bool Grounded;
    private int RemainedJumps = 3;
    const float GroundedRadius = .05f;
    private float VelocityY;

    public float MovementSpeed = 10f;
    public float JumpForce = 1000f;
    public Animator animator;
    [Range(0, .3f)] [SerializeField] float MovementSmoothing = 0.1f;
    public Transform GroundCheck;
    public LayerMask WhatIsGround;
    public int ValidJumps = 3;
    [SerializeField] private Collider2D CrouchDisableCollider;

    void Start()
    {
        // get rigidbody component
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        // set Grounded to false at the beginning of each frame
        Grounded = false;

        // Create a circle around the GroundCheck to check if there is a ground there
        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, GroundedRadius, WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                // If there was a ground nearby, player's remained jumps will be set to validjumps
                RemainedJumps = ValidJumps - 1;
                Grounded = true;
            }
        }

        // get the horizontal movement and store it in a variable
        HorizontalMove = Input.GetAxisRaw("Horizontal") * MovementSpeed;

        // create a vector3 and store the value of the movement that we want to have in it
        Vector3 targetVelocity = new Vector2(HorizontalMove * 10f, Rigidbody2D.velocity.y);
        // smooth the movement and move the character
        Rigidbody2D.velocity = Vector3.SmoothDamp(Rigidbody2D.velocity, targetVelocity, ref Velocity, MovementSmoothing);

        // a condition to check if any key for 'Jump' was pressed
        if (Input.GetButtonDown("Jump"))
        {
            // if the character is grounded then no need to check anything else, just jump
            if (Grounded)
            {
                Rigidbody2D.AddForce(new Vector2(0f, JumpForce + (VelocityY * -100)));
            }
            // otherwise (not grounded) then check if character has more than 0 remained jumps, and also decrease the value of RemainedJumps
            else if (RemainedJumps > 0)
            {
                RemainedJumps -= 1;
                Rigidbody2D.AddForce(new Vector2(0f, JumpForce + (VelocityY * -100)));
            }

        }

        // if character is moving right but it is not facing right, flip it
        if (HorizontalMove > 0 && !facingRight)
        {
            Flip();
        }
        // otherwise if character is moving left but it's facing right, flip it
        else if (HorizontalMove < 0 && facingRight)
        {
            Flip();
        }

        // if the button related to Crouch was pressed down, then disable the CrouchDisableCollider
        if (Input.GetButtonDown("Crouch"))
        {
            CrouchDisableCollider.enabled = false;
        }
        // if the button related to Crouch was up again, then enable the CrouchDisableCollider
        if (Input.GetButtonUp("Crouch"))
        {
            CrouchDisableCollider.enabled = true;
        }

    }

    // and this function, well it flips the character (rotating 180 in Y axis)
    private void Flip()
    {
        facingRight = !facingRight;

        transform.Rotate(0f, -180f, 0f);
    }
}
