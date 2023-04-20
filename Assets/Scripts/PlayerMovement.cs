using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform feetCollider;
    private bool isOnWall;
    public bool isGrounded;
    public bool doubleJump;
    public bool wallJump;
    public bool wallJumpReset;


    private void Update()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");

        playerRB.velocity = new Vector2(horizontalMovement * speed, playerRB.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded == true || doubleJump == true || (wallJump == true && wallJumpReset == true)))
        {
            playerRB.velocity = new Vector2(playerRB.velocity.x, jumpForce);
            
            if(isGrounded == false)
            {
                if (wallJump == true && wallJumpReset == true)
                {
                    wallJump = false;
                    wallJumpReset = false;
                }
                else
                {
                    doubleJump = false;
                }
                
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && playerRB.velocity.y > 0f)
        {
            playerRB.velocity = new Vector2(playerRB.velocity.x, playerRB.velocity.y * 0.5f);
        }
    }

   

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            doubleJump = true;
            wallJumpReset = true;
        }

        if (collision.gameObject.tag == "Wall")
        {
            wallJump = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }

        if (collision.gameObject.tag == "Wall")
        {
            wallJump = false;
        }
    }

}
