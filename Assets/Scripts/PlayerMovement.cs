using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    CharacterController2D controller;
    public float speed = 10f;
    public float gravity = -10f;
    public Rigidbody2D rb;
    public float jumpHeight = 20f;
    public float groundCastLength = 0.5f, groundCastHeight = 0.5f;
    public float smoothedMovementFactor = 10f;
    public Animator animator;
    [SerializeField]
    public LayerMask ground;
    public float jumpDamp = 0.5f;
    private bool _isGrounded;
    private bool _facingRight;
    private bool jump;

    void Start()
    {
        controller = GetComponent<CharacterController2D>();
    }
    
    void Update()
    {
        //float that goes from -1 to 1;
        float horizontalMove = Input.GetAxis("Horizontal");
        Flip(horizontalMove);
        Animate(horizontalMove);
        Vector3 startPos = transform.position + 0.5f * (_facingRight? Vector3.right: Vector3.left);
       _isGrounded = Physics2D.OverlapArea
                (startPos + Vector3.down + Vector3.left * groundCastLength + Vector3.up * groundCastHeight, 
                startPos + Vector3.down + Vector3.down * groundCastHeight + Vector3.right * groundCastLength, ground);
        
        if (Input.GetButton("Jump"))
        {
            jump = true;
           
           if (_isGrounded)
           {
                rb.velocity = Vector2.up * jumpHeight;
           }
        }

        if(jump)
        {
            jump = _isGrounded? false: true;
            if (jump)
            {
                horizontalMove *= jumpDamp;
            }
        }

        Vector3 move = new Vector3(horizontalMove, gravity, 0);
         // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
	    move.x = Mathf.Lerp(move.x, move.x * speed, Time.deltaTime * smoothedMovementFactor);
        
        controller.move(move * Time.deltaTime);
        // if (move != Vector3.zero)
        //     transform.forward = move;  
    }

    public void increaseSpeed(float addSpeed)
    {
        this.speed += addSpeed;
        Debug.Log(this.speed);
    }
    public void increaseJump(float addJump)
    {
        this.jumpHeight += addJump;
    }

    void Animate(float horizontalMove)
    {
        if (Mathf.Abs(horizontalMove) > 0)
        {
            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Running", false);
        }
    }

    void Flip(float horizontalMove)
    {
        if (horizontalMove > 0 && _facingRight == true) 
        {
            _facingRight = !_facingRight;
            transform.eulerAngles = new Vector2(0, 0);
        }
        if (horizontalMove < 0 && _facingRight == false) 
        {
            _facingRight = !_facingRight;
            transform.eulerAngles = new Vector2(0, 180); 
        }
        
        // transform. = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
    }
}
