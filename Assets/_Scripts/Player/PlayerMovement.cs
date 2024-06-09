using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    [SerializeField] public float speed = 25f;

    public static float pspeed;

    public bool IsRunning = false;
    public bool lastInputWasUp = false; // Default to up
    public bool lastInputWasDown = false;
    // Removed isAttacking as it's not used meaningfully

    public GameObject hitbox; // Reference to the hitbox object

    // Offset for hitbox position
    public Vector3 offset = Vector3.zero;
    public Vector3 upOffset = new Vector3(0f, 0.1f, 0f);
    public Vector3 downOffset = new Vector3(0f, -2.5f, 0f);
    public Vector3 leftOffset = new Vector3(-1.3f, -1.6f, 0f);
    public Vector3 rightOffset = new Vector3(1.5f, -1.6f, 0f);

    void Start()
    {
        pspeed = speed;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Deactivate the hitbox
        hitbox.SetActive(false);
        offset = rightOffset;
    }

    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        // Movement
        transform.Translate(new Vector3(horizontalInput, verticalInput, 0f) * speed * Time.deltaTime);

        // Check if the player is running
        IsRunning = (horizontalInput != 0 || verticalInput != 0);
        animator.SetBool("IsRunning", IsRunning);

        // Update hitbox position based on player's position and direction
        hitbox.transform.position = transform.position + offset;

        // Flip sprite if moving horizontally
        if (horizontalInput < 0)
        {
            spriteRenderer.flipX = true;
            lastInputWasDown = false;
            lastInputWasUp = false;
            offset = leftOffset;
        }
        else if (horizontalInput > 0)
        {
            spriteRenderer.flipX = false;
            lastInputWasDown = false;
            lastInputWasUp = false;
            offset = rightOffset;
        }

        // Set Idle animations
        animator.SetBool("IsIdle", !IsRunning || (horizontalInput == 0 && verticalInput == 0 && !lastInputWasUp));
        animator.SetBool("IsUp", lastInputWasUp && !IsRunning && horizontalInput == 0);
        animator.SetBool("IsDown", lastInputWasDown && !IsRunning && horizontalInput == 0);

        // Set RunningUp and RunningDown animations
        animator.SetBool("IsRunningUp", verticalInput > 0 && IsRunning);
        animator.SetBool("IsRunningDown", verticalInput < 0 && IsRunning);

        // Attack
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // isAttacking removed as it's not used
            animator.SetBool("IsAttacking", true);
            hitbox.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // isAttacking removed as it's not used
            animator.SetBool("IsAttacking", false);
            hitbox.SetActive(false);
        }

        // Update lastInputWasUp
        if (verticalInput > 0)
        {
            offset = upOffset;
            lastInputWasUp = true;
            lastInputWasDown = false;
        }
        else if (verticalInput < 0)
        {
            lastInputWasDown = true;
            lastInputWasUp = false;
            offset = downOffset;
        }
    }
}
