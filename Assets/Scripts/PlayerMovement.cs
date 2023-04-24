using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] SwordPointer swordPointer;
    [SerializeField] LayerMask dashLayer;
    private bool isOnWall;
    public bool isGrounded;
    public bool doubleJump;
    public bool wallJump;
    public bool wallJumpReset;
    public float lastYPos;
    int lastWallID;


    private void Update()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");

        playerRB.velocity = new Vector2(horizontalMovement * speed, playerRB.velocity.y);

        Jump();
        Dash();
    }

    private void Jump()
    {
        /*
         * Checks if player is grounded, if double jump is active, or if the player is on a wall and the wall jump reset is true
         * If the player is not grounded, check for wall jump and then double jump and remove whichever is used
         * Jumps used in order Ground -> Wall -> Double
         */
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded == true || doubleJump == true || (wallJump == true && wallJumpReset == true)))
        {
            playerRB.velocity = new Vector2(playerRB.velocity.x, jumpForce);

            if (isGrounded == false)
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
        /*
         * Checks is space is let go, if so slows down jump velocity to give affect of pressing space longer to jump longer
         * Thanks Trey :^)
         */
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

    private void Dash()
    {
        if(Input.GetMouseButtonDown(1))
        {
            playerRB.velocity = new Vector2(0, 0);

            // Casts ray from player, goes in direction of mouse for distance 5, hits layermask 6 (dashCollide)

            RaycastHit2D hit = Physics2D.Raycast(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right, Mathf.Infinity, dashLayer);
            Debug.Log(hit.collider);
            Debug.DrawRay(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right, Color.green, 0.5f);
            Ray airDashRay = new Ray(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right * 5);


            Vector2 point;

            if (hit.collider != null) 
            {
                point = hit.point;
                //Hit something, print the tag of the object
                Debug.Log("Hitting: " + hit.collider.tag);
            } else
            {
                point = airDashRay.GetPoint(5);
            }

            float dashDistance = Mathf.Sqrt(Mathf.Pow(playerRB.transform.position.x - point.x, 2) + Mathf.Pow(playerRB.transform.position.y - point.y, 2));
            Debug.Log("Dash Distance: " + dashDistance);

            Debug.DrawRay(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right * dashDistance, Color.green, 0.5f);

            transform.position = Vector2.MoveTowards(playerRB.transform.position, point, 5);

            playerRB.velocity = new Vector2(0, 0);
        }
    }

}
