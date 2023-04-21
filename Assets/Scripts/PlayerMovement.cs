using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform swordTransform;
    private bool isOnWall;
    public bool isGrounded;
    public bool doubleJump;
    public bool wallJump;
    public bool wallJumpReset;
    int lastWallID;


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

        if(Input.GetMouseButtonDown(1))
        {
            StartCoroutine(Dash());
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

            if(lastWallID != collision.collider.GetInstanceID())
            {
                wallJumpReset = true;
            }

            lastWallID = collision.collider.GetInstanceID();
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

    IEnumerator Dash()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        playerRB.velocity = new Vector2(0, 0);

        for (int i = 0; i < 5; i++) 
        {
            transform.position = transform.position = Vector2.MoveTowards(transform.position, mousePosition, 0.5f);
            yield return new WaitForSeconds(0.02f);
        }

        playerRB.velocity = new Vector2(0, 0);

    }


   
}
